Shader "DayNightSystem/TransitionSkybox"
{
    Properties
    {
        _Transition ("Transition", Range(0, 1)) = 0
        _DayTexture ("Day Texture", Cube) = "white" {}
        _NightTexture ("Night Texture", Cube) = "black" {}
        _StarTexture ("Star Texture", Cube) = "black" {}
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _Exposure ("Exposure", Range(0, 15)) = 1
        [Space]
        _StarIntensity ("Star Intensity", Range(0, 10)) = 1
        _StarMinTransition ("Star Min Transition", Range(0,1)) = 0.9
        _StarMaxTransition ("Star Max Transition", Range(0,1)) = 1.0
        _StarColor ("Star Color", Color) = (1, 1, 1, 1)
        _StarVisibility ("Star Visibility", Range(0.005, 0.1)) = 0.1
        [Space]
        _TwinkleSpeed ("Twinkle Speed", Range(0, 5)) = 1
        _TwinkleIntensity ("Twinkle Intensity", Range(0, 5)) = 1
        [Space]
        _FogColor("Fog Color", Color) = (0.5, 0.5, 0.5, 0.5)
        _FogStart("Fog Start", Range(0, 1)) = 0
        _FogEnd("Fog End", Range(0, 1)) = 0.4
        _FogDensity("Fog Density", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Background" "RenderType"="Background"
        }
        Cull Off ZWrite Off Lighting Off

        Pass
        {
            Fog
            {
                Mode Off
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            samplerCUBE _DayTexture;
            samplerCUBE _NightTexture;
            samplerCUBE _StarTexture;

            float _Transition;
            fixed4 _Tint;
            float _Exposure;
            float _StarIntensity;
            float _StarMinTransition;
            float _StarMaxTransition;
            fixed4 _StarColor;
            float _StarVisibility;
            float _TwinkleSpeed;
            float _TwinkleIntensity;

            fixed4 _FogColor;
            float _FogStart;
            float _FogEnd;
            float _FogDensity;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 skyboxCoord : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };

            // Generates a pseudo-random value based on a 3D coordinate
            float rand(float3 co)
            {
                return frac(sin(dot(co, float3(12.9898, 78.233, 45.164))) * 43758.5453);
            }

            v2f vert(appdata v)
            {
                // Transforms vertices and calculates skybox coordinates
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.skyboxCoord = v.vertex * _StarVisibility;
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Normalize the skybox coordinates
                float3 worldPos = normalize(i.skyboxCoord.xyz);

                // Sample day, night, and star textures
                float3 dayColor = texCUBE(_DayTexture, worldPos);
                float3 nightColor = texCUBE(_NightTexture, worldPos);
                float3 starColor = texCUBE(_StarTexture, worldPos) * _StarColor.rgb;

                // Calculate twinkling effect for stars
                float noise = rand(worldPos);
                float twinkle = 1.0 + _TwinkleIntensity * sin(_TwinkleSpeed * _Time.y * noise * 6.28);
                starColor *= twinkle * _StarIntensity;

                // Blend day and night textures based on the transition value
                float3 baseColor = lerp(dayColor, nightColor, _Transition);

                // Adjust star visibility based on configurable min/max transition range
                float starVisibility = saturate(
                    (_Transition - _StarMinTransition) / max(1e-5, (_StarMaxTransition - _StarMinTransition)));
                float3 finalColor = baseColor + starColor * starVisibility;

                // Apply tint and exposure to the final color
                finalColor.rgb *= _Tint.rgb;
                finalColor.rgb *= _Exposure;

                // Apply Unity's fog effect
                UNITY_APPLY_FOG(i.fogCoord, finalColor);

                // Custom fog blending based on distance
                float fogAmount = 1.0 - saturate(abs(i.skyboxCoord.y) - _FogStart) / (_FogEnd - _FogStart);
                fogAmount = pow(fogAmount, 1.0 / _FogDensity);
                finalColor.rgb = lerp(finalColor.rgb, _FogColor.rgb, fogAmount);

                return float4(finalColor, 1.0);
            }
            ENDCG
        }
    }
    Fallback Off
}