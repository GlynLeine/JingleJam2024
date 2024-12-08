using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;
using static Unity.Burst.Intrinsics.X86.Avx;
using static UnityEditor.TerrainTools.MeshUtils;
using static UnityEngine.Rendering.RenderGraphModule.Util.RenderGraphUtils;

public class VolumetricFogRenderPass : ScriptableRenderPass
{
    private VolumetricFogSettings defaultSettings;
    private Material material;
    private RenderTextureDescriptor sceneColorTextureDescriptor;
    private RenderTextureDescriptor fogTextureDescriptor;

    private static readonly int resolutionId = Shader.PropertyToID("_Resolution");
    private static readonly int fovId = Shader.PropertyToID("_Fov");
    private static readonly int cameraMatrixId = Shader.PropertyToID("_CameraMatrix");
    private static readonly int stepCountId = Shader.PropertyToID("_StepCount");
    private static readonly int fogDensityId = Shader.PropertyToID("_Density");
    private static readonly int scatteringCoefficientsId = Shader.PropertyToID("_SigmaS");
    private static readonly int absorptionCoefficientsId = Shader.PropertyToID("_SigmaA");
    private static readonly int extinctionCoefficientsId = Shader.PropertyToID("_SigmaT");
    private static readonly int lightScaleId = Shader.PropertyToID("_LightScale");
    private static readonly int fogTextureId = Shader.PropertyToID("_FogTexture");

    private const string fogTextureName = "_FogTexture";
    private const string sceneColorTextureName = "_SceneColor";
    private const string fogPassName = "FogRenderPass";
    private const string compositePassName = "FogCompositeRenderPass";

    class CompositePassData
    {
        public int sourceTexturePropertyID;
        public TextureHandle source;
        public int fogDataTexturePropertyID;
        public TextureHandle fogData;
        public TextureHandle destination;
        public Vector2 scale;
        public Vector2 offset;
        public Material material;
        public int shaderPass;
        public MaterialPropertyBlock propertyBlock;
        public int sourceSlice;
        public int destinationSlice;
        public int numSlices;
        public int sourceMip;
        public int destinationMip;
        public int numMips;
        public FullScreenGeometryType geometry;
        public int sourceSlicePropertyID;
        public int sourceMipPropertyID;
        public int scaleBiasPropertyID;
    }

    static MaterialPropertyBlock s_PropertyBlock = new MaterialPropertyBlock();
    static void CompositeMaterialRenderFunc(CompositePassData data, UnsafeGraphContext context)
    {
        Vector4 blitScaleBias;
        blitScaleBias.x = data.scale.x;
        blitScaleBias.y = data.scale.y;
        blitScaleBias.z = data.offset.x;
        blitScaleBias.w = data.offset.y;

        CommandBuffer unsafeCmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);

        if (data.propertyBlock == null) data.propertyBlock = s_PropertyBlock;

        data.propertyBlock.SetTexture(data.fogDataTexturePropertyID, data.fogData);

        data.propertyBlock.SetTexture(data.sourceTexturePropertyID, data.source);
        if (data.sourceSlice == -1)
            data.propertyBlock.SetInt(data.sourceSlicePropertyID, 0);
        if (data.sourceMip == -1)
            data.propertyBlock.SetInt(data.sourceMipPropertyID, 0);
        data.propertyBlock.SetVector(data.scaleBiasPropertyID, blitScaleBias);

        for (int currSlice = 0; currSlice < data.numSlices; currSlice++)
        {
            for (int currMip = 0; currMip < data.numMips; currMip++)
            {
                if (data.sourceSlice != -1)
                    data.propertyBlock.SetInt(data.sourceSlicePropertyID, data.sourceSlice + currSlice);
                if (data.sourceMip != -1)
                    data.propertyBlock.SetInt(data.sourceMipPropertyID, data.sourceMip + currMip);

                context.cmd.SetRenderTarget(data.destination, data.destinationMip + currMip, CubemapFace.Unknown, data.destinationSlice + currSlice);
                switch (data.geometry)
                {
                    case FullScreenGeometryType.Mesh:
                        unsafeCmd.DrawMesh(VolumetricFogRenderFeature.s_QuadMesh, Matrix4x4.identity, data.material, 0, data.shaderPass, data.propertyBlock);
                        break;
                    case FullScreenGeometryType.ProceduralQuad:
                        if (SystemInfo.graphicsShaderLevel < 30)
                        {
                            unsafeCmd.DrawMesh(VolumetricFogRenderFeature.s_QuadMesh, Matrix4x4.identity, data.material, 0, data.shaderPass, data.propertyBlock);
                        }
                        else
                        {
                            unsafeCmd.DrawProcedural(Matrix4x4.identity, data.material, data.shaderPass, MeshTopology.Quads, 4, 1, data.propertyBlock);
                        }
                        break;
                    case FullScreenGeometryType.ProceduralTriangle:
                        if (SystemInfo.graphicsShaderLevel < 30)
                        {
                            unsafeCmd.DrawMesh(VolumetricFogRenderFeature.s_TriangleMesh, Matrix4x4.identity, data.material, 0, data.shaderPass, data.propertyBlock);
                        }
                        else
                        {
                            unsafeCmd.DrawProcedural(Matrix4x4.identity, data.material, data.shaderPass, MeshTopology.Triangles, 3, 1, data.propertyBlock);
                        }
                        break;
                }
            }
        }
    }

    public VolumetricFogRenderPass(Material material, VolumetricFogSettings defaultSettings)
    {
        this.material = material;
        this.defaultSettings = defaultSettings;

        fogTextureDescriptor = new RenderTextureDescriptor(Screen.width, Screen.height,
        RenderTextureFormat.ARGBFloat, 0);
        sceneColorTextureDescriptor = new RenderTextureDescriptor(Screen.width, Screen.height,
        RenderTextureFormat.Default, 0);
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

        // The following line ensures that the render pass doesn't blit
        // from the back buffer.
        if (resourceData.isActiveTargetBackBuffer)
            return;

        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>(); ;

        // Update the fog settings in the material
        UpdateSettings(renderGraph, cameraData, resourceData);

        TextureHandle srcCamColor = resourceData.activeColorTexture;
        TextureHandle sceneColorTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, sceneColorTextureDescriptor, sceneColorTextureName, false);
        TextureHandle fogTexture = UniversalRenderer.CreateRenderGraphTexture(renderGraph, fogTextureDescriptor, fogTextureName, false);

        // This check is to avoid an error from the material preview in the scene
        if (!srcCamColor.IsValid() || !sceneColorTexture.IsValid() || !fogTexture.IsValid())
            return;

        renderGraph.AddCopyPass(srcCamColor, sceneColorTexture);

        RenderGraphUtils.BlitMaterialParameters fogPassParams = new(srcCamColor, fogTexture, material, 0);
        renderGraph.AddBlitPass(fogPassParams, fogPassName);

        RenderGraphUtils.BlitMaterialParameters compositePassParams = new(sceneColorTexture, srcCamColor, material, 1);
        using (var builder = renderGraph.AddUnsafePass<CompositePassData>(compositePassName, out var passData))
        {
            passData.sourceTexturePropertyID = compositePassParams.sourceTexturePropertyID;
            passData.source = compositePassParams.source;
            passData.fogDataTexturePropertyID = fogTextureId;
            passData.fogData = fogTexture;
            passData.destination = compositePassParams.destination;
            passData.scale = compositePassParams.scale;
            passData.offset = compositePassParams.offset;
            passData.material = compositePassParams.material;
            passData.shaderPass = compositePassParams.shaderPass;
            passData.propertyBlock = compositePassParams.propertyBlock;
            passData.sourceSlice = compositePassParams.sourceSlice;
            passData.destinationSlice = compositePassParams.destinationSlice;
            passData.numSlices = compositePassParams.numSlices;
            passData.sourceMip = compositePassParams.sourceMip;
            passData.destinationMip = compositePassParams.destinationMip;
            passData.numMips = compositePassParams.numMips;
            passData.geometry = compositePassParams.geometry;
            passData.sourceSlicePropertyID = compositePassParams.sourceSlicePropertyID;
            passData.sourceMipPropertyID = compositePassParams.sourceMipPropertyID;
            passData.scaleBiasPropertyID = compositePassParams.scaleBiasPropertyID;

            builder.UseTexture(fogTexture, AccessFlags.Read);
            builder.UseTexture(compositePassParams.source, AccessFlags.Read);
            builder.UseTexture(compositePassParams.destination, AccessFlags.Write);
            builder.SetRenderFunc((CompositePassData data, UnsafeGraphContext context) => CompositeMaterialRenderFunc(data, context));
        }
    }

    private void UpdateSettings(RenderGraph renderGraph, UniversalCameraData cameraData, UniversalResourceData resourceData)
    {
        if (material == null) return;

        // Use the Volume settings or the default settings if no Volume is set.
        var volumeComponent = VolumeManager.instance.stack.GetComponent<VolumetricFogVolumeComponent>();

        float fogResolutionScale = volumeComponent.resolutionScale.overrideState ? volumeComponent.resolutionScale.value : defaultSettings.resolutionScale;
        Vector2Int fogResolution = new Vector2Int(
            Mathf.NextPowerOfTwo(Mathf.FloorToInt(fogResolutionScale * cameraData.cameraTargetDescriptor.width)),
            Mathf.NextPowerOfTwo(Mathf.FloorToInt(fogResolutionScale * cameraData.cameraTargetDescriptor.height))
            );

        // Set the fog texture size to be the same as the camera target size.
        fogTextureDescriptor.width = fogResolution.x;
        fogTextureDescriptor.height = fogResolution.y;
        fogTextureDescriptor.depthBufferBits = 0;

        material.SetVector(resolutionId, new Vector4(
            cameraData.cameraTargetDescriptor.width,
            cameraData.cameraTargetDescriptor.height,
            fogResolution.x,
            fogResolution.y
            ));

        material.SetFloat(fovId, cameraData.camera.fieldOfView * Mathf.Deg2Rad);
        material.SetMatrix(cameraMatrixId, cameraData.camera.transform.localToWorldMatrix);

        int stepCount = volumeComponent.stepCount.overrideState ? volumeComponent.stepCount.value : defaultSettings.stepCount;
        material.SetInt(stepCountId, stepCount);

        float fogDensity = volumeComponent.fogDensity.overrideState ? volumeComponent.fogDensity.value : defaultSettings.fogDensity;
        material.SetFloat(fogDensityId, fogDensity);

        Color scatteringCoefficients = volumeComponent.scatteringCoefficients.overrideState ?
            volumeComponent.scatteringCoefficients.value : defaultSettings.scatteringCoefficients;
        material.SetColor(scatteringCoefficientsId, scatteringCoefficients);

        Color absorptionCoefficients = volumeComponent.absorptionCoefficients.overrideState ?
            volumeComponent.absorptionCoefficients.value : defaultSettings.absorptionCoefficients;
        material.SetColor(absorptionCoefficientsId, absorptionCoefficients);

        Color extinctionCoefficients = scatteringCoefficients + absorptionCoefficients;
        material.SetColor(extinctionCoefficientsId, extinctionCoefficients);

        float lightScale = volumeComponent.lightScale.overrideState ? volumeComponent.lightScale.value : defaultSettings.lightScale;
        material.SetFloat(lightScaleId, lightScale);
    }
}
