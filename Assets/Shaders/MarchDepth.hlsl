void MarchDepth_float(float3 rayOrigin, float3 rayDir, out float2 hit)
{
    float depth = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(rayOrigin.xy), _ZBufferParams);

    hit = rayOrigin.xy + rayDir.xy * (depth - rayOrigin.z) * 0.0001;
}