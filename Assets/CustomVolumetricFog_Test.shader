Shader "Assets/VolumetricFogTest"
{
    Properties
    {
        fog_base_color("Fog Base Color", Color) = (1, 1, 1, 1)

        max_distance("Max Distance", float) = 100
        step_size("Step Size", Range(0.01, 1)) = 1
        density_multiplier("Density Multiplier", Range(0,10)) = 1

        noise_offset("Noise Offset", float) = 0
        light_scattering("Light Scattering", float) = 0
        light_contribution("Light Contribution", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float4 fog_base_color;
            float4 light_contribution;
            float  max_distance;
            float  step_size;
            float  density_multiplier;
            float  noise_offset;
            float  light_scattering;

            float get_density()
            {
                return density_multiplier;
            }

            float henyey_geenstein(float angle, float scattering)
            {
                return (1.0 - angle * angle) / 4.0 * PI * pow(1.0 + scattering * scattering - (2.0 * scattering) * angle, 1.5f);
            }

            float getLightScattering(float3 raydir, Light mainLight)
            {
                return henyey_geenstein(dot(raydir, mainLight.direction), light_scattering);
            }

            half4 frag(Varyings IN) : SV_Target
            {
                //default values
                float4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, IN.texcoord);
                float  depth = SampleSceneDepth(IN.texcoord);
                float3 worldPos = ComputeWorldSpacePosition(IN.texcoord, depth, UNITY_MATRIX_I_VP);

                //ray data
                float3 rayStart = _WorldSpaceCameraPos;
                float3 viewDir = worldPos - _WorldSpaceCameraPos;
                float  viewDist = length(viewDir);
                float3 rayDir = normalize(viewDir);

                float distLimit = min(viewDist, max_distance);

                //adding noise to break out banding
                float2 pixelCoords = IN.texcoord * _BlitTexture_TexelSize.zw;
                int    frameindex = _Time.y / max(unity_DeltaTime.x, HALF_EPS);
                float  noiseValue = InterleavedGradientNoise(pixelCoords, frameindex);
                float  distTraveled = noiseValue * noise_offset;

                //adding shadows
                half4 shadowCoord = TransformWorldToShadowCoord(worldPos);
                Light mainLight = GetMainLight(shadowCoord, worldPos, 1.0);
                half  shadow = mainLight.shadowAttenuation;

                //raymarching loop
                float  transmittance = 1;
                float4 outputColor = fog_base_color * shadow;

                while(distTraveled < distLimit)
                {
                    float3 rayPos = rayStart + rayDir * distTraveled;
                    float  density = get_density();
                    if(density > 0)
                    {
                        //for each light, computes shadows
                        Light mainLight = GetMainLight(TransformWorldToShadowCoord(rayPos));
                        outputColor.rgb += mainLight.color.rgb * light_contribution.rgb *
                            density * step_size * mainLight.shadowAttenuation ;

                        transmittance *= exp(-density * step_size);
                    }
                    distTraveled += step_size;
                }

                return lerp(col, outputColor, 1.0 - saturate(transmittance));
            }
            ENDHLSL
        }
    }
}