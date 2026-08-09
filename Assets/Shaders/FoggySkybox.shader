Shader "Skybox/FoggySkybox"
{
    Properties
    {
        [Header(Sky Gradient)]
        _SkyTop     ("Sky Top Color",    Color) = (0.25, 0.45, 0.85, 1)
        _SkyHorizon ("Sky Horizon Color",Color) = (0.75, 0.80, 0.85, 1)
        _SkyBottom  ("Sky Bottom Color", Color) = (0.30, 0.30, 0.30, 1)
        _HorizonSharpness ("Horizon Sharpness", Range(0.5, 8)) = 3

        [Header(Fog Blend)]
        _FogSkyBlend ("Fog Influence On Sky", Range(0,1)) = 1
        _FogSkyPower ("Fog Sky Falloff Power", Range(0.1, 8)) = 2
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" "RenderPipeline"="UniversalPipeline" }
        Cull Off ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 viewDirWS  : TEXCOORD0;
            };

            float4 _SkyTop;
            float4 _SkyHorizon;
            float4 _SkyBottom;
            float  _HorizonSharpness;
            float  _FogSkyBlend;
            float  _FogSkyPower;

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                OUT.viewDirWS = normalize(TransformObjectToWorldDir(IN.positionOS.xyz));
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                float h = normalize(IN.viewDirWS).y;
                
                half3 skyCol;
                if (h > 0)
                {
                    float t = pow(saturate(h), 1.0 / max(_HorizonSharpness, 0.001));
                    skyCol = lerp(_SkyHorizon.rgb, _SkyTop.rgb, t);
                }
                else
                {
                    float t = pow(saturate(-h), 1.0 / max(_HorizonSharpness, 0.001));
                    skyCol = lerp(_SkyHorizon.rgb, _SkyBottom.rgb, t);
                }
                
                half3 fogColor = unity_FogColor.rgb;
                
                float fogAmount = 1.0 - saturate(abs(h));
                fogAmount = pow(fogAmount, _FogSkyPower) * _FogSkyBlend;

                half3 finalColor = lerp(skyCol, fogColor, fogAmount);

                return half4(finalColor, 1);
            }
            ENDHLSL
        }
    }
}
