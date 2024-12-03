#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

float SampleDepth(float2 texcoords)
{
    if (unity_OrthoParams.w == 1.0)
    {
        return LinearEyeDepth(ComputeWorldSpacePosition(texcoords, SampleSceneDepth(texcoords), UNITY_MATRIX_I_VP), UNITY_MATRIX_V) * _ProjectionParams.x;
    }
    else
    {
        return LinearEyeDepth(SampleSceneDepth(texcoords), _ZBufferParams) * _ProjectionParams.x;
    }
}

float2 ToScreenSpacePos(float3 viewSpace)
{
    float4 result = mul(UNITY_MATRIX_P, float4(viewSpace, 1.0));
    result.xy = result.xy / result.w;
#if UNITY_UV_STARTS_AT_TOP
    result.y = -result.y;
#endif
    return result.xy * 0.5 + 0.5;
}

void MarchDepth_float(float3 rayOrigin, float3 rayDir, out float2 hit)
{
    const float marchDist = 5.0;
    const float stepSize = 0.25;
    
    float2 currentTexCoords = ToScreenSpacePos(rayOrigin);
    float2 nextTexCoords = currentTexCoords;
    float sampleDepth = SampleDepth(currentTexCoords);

    float hitDist = 0.0;
    float prevDepthDist = abs(rayOrigin.z - sampleDepth);
    for(float i = stepSize; i < marchDist; i += stepSize)
    {
        float3 rayPos = rayOrigin + rayDir * i;
        nextTexCoords = ToScreenSpacePos(rayPos);
        sampleDepth = SampleDepth(nextTexCoords);
        float depthDist = abs(rayPos.z - sampleDepth);
        if(depthDist < prevDepthDist)
        {
            currentTexCoords = nextTexCoords;
            hitDist = i;
            prevDepthDist = depthDist;
        }
        else
        {
            break;
        }
    }

    hit = currentTexCoords;
}