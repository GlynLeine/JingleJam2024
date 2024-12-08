using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[Serializable]
public class VolumetricFogSettings
{
    public int stepCount = 50;
    public float fogDensity = 64f;
    public Color scatteringCoefficients = Color.white;
    public Color absorptionCoefficients = Color.black;
    public float lightScale = 1f;
}

public class VolumetricFogRenderFeature : ScriptableRendererFeature
{
    [SerializeField] private VolumetricFogSettings settings;
    [SerializeField] private Shader shader;
    private Material material;
    private VolumetricFogRenderPass renderPass;

    public override void Create()
    {
        if (shader == null)
        {
            return;
        }

        material = new Material(shader);
        renderPass = new VolumetricFogRenderPass(material, settings);

        renderPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderPass == null)
        {
            return;
        }

        if (renderingData.cameraData.cameraType == CameraType.Game || renderingData.cameraData.cameraType == CameraType.SceneView)
        {
            renderer.EnqueuePass(renderPass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (Application.isPlaying)
        {
            Destroy(material);
        }
        else
        {
            DestroyImmediate(material);
        }
    }
}
