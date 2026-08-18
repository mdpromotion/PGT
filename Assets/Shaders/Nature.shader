Shader "Custom/Grass/Nature"
{
    Properties
    {
        [MainTexture] _BaseMap ("Grass Texture", 2D) = "white" {}

        [Header(Phase Colors)]
        _NightColor ("Night Color", Color) = (0.05, 0.05, 0.10, 1)
        _DawnColor  ("Dawn Color", Color) = (0.85, 0.55, 0.35, 1)
        _NoonColor  ("Noon Color", Color) = (1.0, 1.0, 1.0, 1)
        _DuskColor  ("Dusk Color", Color) = (0.75, 0.40, 0.30, 1)

        [Header(Phase Keyframes)]
        _NightTime ("Night Time", Range(0,1)) = 0.0
        _DawnTime  ("Dawn Time", Range(0,1)) = 0.25
        _NoonTime  ("Noon Time", Range(0,1)) = 0.5
        _DuskTime  ("Dusk Time", Range(0,1)) = 0.75

        [Header(Real Light Influence)]
        [Toggle] _TintByLightColor ("Multiply by actual Light Color", Float) = 1

        [Header(Shadows)]
        [Toggle] _ReceiveShadows ("Receive Shadows", Float) = 1
        _ShadowStrength ("Shadow Strength", Range(0,1)) = 1.0

        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "TransparentCutout"
            "Queue" = "AlphaTest"
            "RenderPipeline" = "UniversalPipeline"
        }

        Cull Off
        ZWrite On

        Pass
        {
            Name "Grass"

            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM

            #pragma target 2.0
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing
            #pragma multi_compile_fog

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_SCREEN

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float _GlobalTimeOfDay;

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;

                float4 _NightColor;
                float4 _DawnColor;
                float4 _NoonColor;
                float4 _DuskColor;

                float _NightTime;
                float _DawnTime;
                float _NoonTime;
                float _DuskTime;

                float _TintByLightColor;

                float _ReceiveShadows;
                float _ShadowStrength;

                float _Cutoff;
            CBUFFER_END

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                half fogFactor : TEXCOORD1;
                float4 shadowCoord : TEXCOORD2;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                VertexPositionInputs positionInputs =
                    GetVertexPositionInputs(input.positionOS);

                output.positionCS = positionInputs.positionCS;

                output.uv = TRANSFORM_TEX(
                    input.uv,
                    _BaseMap
                );

                output.fogFactor =
                    ComputeFogFactor(positionInputs.positionCS.z);

                output.shadowCoord =
                    GetShadowCoord(positionInputs);

                return output;
            }

            float InverseLerp(float a, float b, float v)
            {
                return saturate(
                    (v - a) / max(b - a, 1e-5)
                );
            }

            half3 EvaluateColorByTimeOfDay(float t)
            {
                t = frac(t);

                if (t < _NightTime)
                {
                    return _NightColor.rgb;
                }
                else if (t < _DawnTime)
                {
                    float f = InverseLerp(
                        _NightTime,
                        _DawnTime,
                        t
                    );

                    return lerp(
                        _NightColor.rgb,
                        _DawnColor.rgb,
                        f
                    );
                }
                else if (t < _NoonTime)
                {
                    float f = InverseLerp(
                        _DawnTime,
                        _NoonTime,
                        t
                    );

                    return lerp(
                        _DawnColor.rgb,
                        _NoonColor.rgb,
                        f
                    );
                }
                else if (t < _DuskTime)
                {
                    float f = InverseLerp(
                        _NoonTime,
                        _DuskTime,
                        t
                    );

                    return lerp(
                        _NoonColor.rgb,
                        _DuskColor.rgb,
                        f
                    );
                }
                else
                {
                    float wrappedNightTime =
                        _NightTime + 1.0;

                    float f = InverseLerp(
                        _DuskTime,
                        wrappedNightTime,
                        t
                    );

                    return lerp(
                        _DuskColor.rgb,
                        _NightColor.rgb,
                        f
                    );
                }
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);

                half4 tex = SAMPLE_TEXTURE2D(
                    _BaseMap,
                    sampler_BaseMap,
                    input.uv
                );

                clip(tex.a - _Cutoff);

                half3 phaseColor =
                    EvaluateColorByTimeOfDay(
                        _GlobalTimeOfDay
                    );

                half3 finalColor =
                    tex.rgb * phaseColor;

                Light mainLight;

                #if defined(_MAIN_LIGHT_SHADOWS) || \
                    defined(_MAIN_LIGHT_SHADOWS_CASCADE) || \
                    defined(_MAIN_LIGHT_SHADOWS_SCREEN)

                    mainLight = GetMainLight(
                        input.shadowCoord
                    );

                #else

                    mainLight = GetMainLight();

                #endif

                if (_TintByLightColor > 0.5)
                {
                    finalColor *= mainLight.color;
                }

                if (_ReceiveShadows > 0.5)
                {
                    half shadowAttenuation =
                        mainLight.shadowAttenuation;

                    shadowAttenuation = lerp(
                        1.0,
                        shadowAttenuation,
                        _ShadowStrength
                    );

                    finalColor *= shadowAttenuation;
                }

                finalColor = MixFog(
                    finalColor,
                    input.fogFactor
                );

                return half4(
                    finalColor,
                    1.0
                );
            }

            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"

            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Off

            HLSLPROGRAM

            #pragma target 2.0
            #pragma vertex ShadowVert
            #pragma fragment ShadowFrag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float _Cutoff;
            CBUFFER_END

            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings ShadowVert(Attributes input)
            {
                Varyings output;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                VertexPositionInputs positionInputs =
                    GetVertexPositionInputs(input.positionOS);

                output.positionCS = positionInputs.positionCS;

                output.uv = TRANSFORM_TEX(
                    input.uv,
                    _BaseMap
                );

                return output;
            }

            half4 ShadowFrag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);

                half alpha = SAMPLE_TEXTURE2D(
                    _BaseMap,
                    sampler_BaseMap,
                    input.uv
                ).a;

                clip(alpha - _Cutoff);

                return 0;
            }

            ENDHLSL
        }
    }
}