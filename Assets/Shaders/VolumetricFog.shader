Shader "CustomEffects/Volumetric Fog"
{
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType"="Lit" }
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "FogRenderPass"

            HLSLPROGRAM

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ _LIGHT_LAYERS
            #pragma multi_compile _ _FORWARD_PLUS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
                
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // The Blit.hlsl file provides the vertex shader (Vert),
            // the input structure (Attributes), and the output structure (Varyings)
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
        
            float2 _Resolution;
            float _Fov;
            float4x4 _CameraMatrix; // Inverse view matrix

            int _StepCount;
            float _Density;
            float4 _SigmaS;
            float4 _SigmaA;
            float4 _SigmaT;
            float _LightScale;

            float3 GetRay(float2 texcoord)
            {
	            float2 xy = (texcoord * 2 - 1) * (2 * tan(0.5 * _Fov));
                xy.x *= _Resolution.x / _Resolution.y;
                return normalize(mul(_CameraMatrix, float4(xy, 1, 0)).xyz);
            }

            float3 SampleSceneRadiance(float2 texcoord)
            {
                return SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, texcoord).rgb;
            }

            // Phase function
            // https://www.pbr-book.org/3ed-2018/Volume_Scattering/Phase_Functions
            float HenyeyGreenstein(float g, float cosTheta) {
	            return (1.0 / (4.0 * 3.1415926)) * ((1.0 - g * g) / pow(max(0.0, 1.0 + g * g - 2.0 * g * cosTheta), 1.5));
            }

            float3 MarchRay(float2 texcoord, inout float3 totalTransmittance)
            {
	            float3 radiance = float3(0, 0, 0);
                float3 rayDir = GetRay(texcoord);
                float traversalDepth = LinearEyeDepth(SampleSceneDepth(texcoord), _ZBufferParams);

	            float stepSize = traversalDepth / _StepCount;

	            float3 samplePoint = _WorldSpaceCameraPos;

                float cosTheta = dot(rayDir, _MainLightPosition.xyz);
	            float phaseFunction = lerp(HenyeyGreenstein(-0.3, cosTheta), HenyeyGreenstein(0.3, cosTheta), 0.7);

                float scaledDensity = _Density * 0.001;

                for(int i = 0; i < _StepCount; i++)
                {
                    float3 densitySigmaT = scaledDensity * _SigmaT.rgb;

			        float3 transmittance = exp(-stepSize * densitySigmaT);

                    Light light = GetMainLight(TransformWorldToShadowCoord(samplePoint), samplePoint, half4(1, 1, 1, 1));

			        float3 inscatter = _SigmaS.rgb * scaledDensity * light.color * light.shadowAttenuation * _LightScale * phaseFunction;

			        radiance += totalTransmittance * (inscatter - inscatter * transmittance) / densitySigmaT;
			        totalTransmittance *= transmittance;

			        if(totalTransmittance.x * totalTransmittance.y * totalTransmittance.z < 0.0001)
			        {
				        totalTransmittance = float3(0, 0, 0);
				        break;
			        }

		            samplePoint += rayDir * stepSize;
                }

                return radiance;
            }
    
            float4 ComputeFog (Varyings input) : SV_Target
            {
                float3 sceneRadiance = SampleSceneRadiance(input.texcoord);

                if (_StepCount == 0)
                    return float4(sceneRadiance, 1);
	
                float3 totalTransmittance = float3(1, 1, 1);

                float3 fogRadiance = MarchRay(input.texcoord, totalTransmittance);

                return float4(sceneRadiance * totalTransmittance + fogRadiance, 1);
            }

            #pragma vertex Vert
            #pragma fragment ComputeFog
            
            ENDHLSL
        }
        
        Pass
        {
            Name "FogCompositeRenderPass"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // The Blit.hlsl file provides the vertex shader (Vert),
            // the input structure (Attributes), and the output structure (Varyings)
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float4 CompositeFog (Varyings input) : SV_Target
            {
                return float4(SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord).rgb, 1);
            }
            
            #pragma vertex Vert
            #pragma fragment CompositeFog
            
            ENDHLSL
        }
    }
}