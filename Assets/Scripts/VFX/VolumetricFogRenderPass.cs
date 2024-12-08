using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class VolumetricFogRenderPass : ScriptableRenderPass
{
    private VolumetricFogSettings defaultSettings;
    private Material material;
    private RenderTextureDescriptor fogTextureDescriptor;

    private static readonly int mainLightShadowmapTextureId = Shader.PropertyToID("_MainLightShadowmapTexture");

    private static readonly int resolutionId = Shader.PropertyToID("_Resolution");
    private static readonly int fovId = Shader.PropertyToID("_Fov");
    private static readonly int cameraMatrixId = Shader.PropertyToID("_CameraMatrix");
    private static readonly int stepCountId = Shader.PropertyToID("_StepCount");
    private static readonly int fogDensityId = Shader.PropertyToID("_Density");
    private static readonly int scatteringCoefficientsId = Shader.PropertyToID("_SigmaS");
    private static readonly int absorptionCoefficientsId = Shader.PropertyToID("_SigmaA");
    private static readonly int extinctionCoefficientsId = Shader.PropertyToID("_SigmaT");
    private static readonly int lightScaleId = Shader.PropertyToID("_LightScale");

    private const string fogTextureName = "_FogTexture";
    private const string fogPassName = "FogRenderPass";
    private const string compositePassName = "FogCompositeRenderPass";

    public VolumetricFogRenderPass(Material material, VolumetricFogSettings defaultSettings)
    {
        this.material = material;
        this.defaultSettings = defaultSettings;

        fogTextureDescriptor = new RenderTextureDescriptor(Screen.width, Screen.height,
        RenderTextureFormat.Default, 0);
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

        TextureHandle srcCamColor = resourceData.activeColorTexture;
        TextureHandle dst = UniversalRenderer.CreateRenderGraphTexture(renderGraph, fogTextureDescriptor, fogTextureName, false);

        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

        // The following line ensures that the render pass doesn't blit
        // from the back buffer.
        if (resourceData.isActiveTargetBackBuffer)
            return;

        // Set the fog texture size to be the same as the camera target size.
        fogTextureDescriptor.width = cameraData.cameraTargetDescriptor.width;
        fogTextureDescriptor.height = cameraData.cameraTargetDescriptor.height;
        fogTextureDescriptor.depthBufferBits = 0;

        // Update the fog settings in the material
        UpdateSettings(renderGraph, cameraData, resourceData);

        // This check is to avoid an error from the material preview in the scene
        if (!srcCamColor.IsValid() || !dst.IsValid())
            return;

        RenderGraphUtils.BlitMaterialParameters fogPassParams = new(srcCamColor, dst, material, 0);
        renderGraph.AddBlitPass(fogPassParams, fogPassName);

        RenderGraphUtils.BlitMaterialParameters compositePassParams = new(dst, srcCamColor, material, 1);
        renderGraph.AddBlitPass(compositePassParams, compositePassName);
    }

    private void UpdateSettings(RenderGraph renderGraph, UniversalCameraData cameraData, UniversalResourceData resourceData)
    {
        if (material == null) return;

        // Use the Volume settings or the default settings if no Volume is set.
        var volumeComponent =
            VolumeManager.instance.stack.GetComponent<VolumetricFogVolumeComponent>();
        material.SetVector(resolutionId, new Vector4(cameraData.cameraTargetDescriptor.width, cameraData.cameraTargetDescriptor.height));
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
