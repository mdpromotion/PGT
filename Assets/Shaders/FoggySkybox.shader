Shader "Skybox/FoggySkybox"
{
    Properties
    {
        [Header(Sky Gradient Day)]
        _SkyTop     ("Sky Top Color",    Color) = (0.25, 0.45, 0.85, 1)
        _SkyHorizon ("Sky Horizon Color",Color) = (0.75, 0.80, 0.85, 1)
        _SkyBottom  ("Sky Bottom Color", Color) = (0.30, 0.30, 0.30, 1)
        _HorizonSharpness ("Horizon Sharpness", Range(0.5, 8)) = 3

        [Header(Sky Gradient Night)]
        _NightSkyTop     ("Night Sky Top Color",    Color) = (0.02, 0.03, 0.08, 1)
        _NightSkyHorizon ("Night Sky Horizon Color",Color) = (0.05, 0.06, 0.12, 1)
        _NightSkyBottom  ("Night Sky Bottom Color", Color) = (0.01, 0.01, 0.02, 1)

        [Header(Day Night Transition)]
        _DayNightSharpness ("Day/Night Transition Sharpness", Range(1, 32)) = 8

        [Header(Fog Blend)]
        _FogSkyBlend ("Fog Influence On Sky", Range(0,1)) = 1
        _FogSkyPower ("Fog Sky Falloff Power", Range(0.1, 8)) = 2

        [Header(Sun)]
        _SunColor      ("Sun Color", Color) = (1.0, 0.95, 0.85, 1)
        _SunSize       ("Sun Size", Range(0.0001, 0.1)) = 0.02
        _SunFalloff    ("Sun Edge Falloff", Range(1, 512)) = 128
        _SunGlowColor  ("Sun Glow Color", Color) = (1.0, 0.8, 0.5, 1)
        _SunGlowSize   ("Sun Glow Size", Range(0.5, 32)) = 4
        _SunGlowIntensity ("Sun Glow Intensity", Range(0, 4)) = 1

        [Header(Moon)]
        _MoonColor   ("Moon Color", Color) = (0.85, 0.87, 0.95, 1)
        _MoonSize    ("Moon Size", Range(0.0001, 0.1)) = 0.015
        _MoonFalloff ("Moon Edge Falloff", Range(1, 512)) = 256

        [Header(Stars)]
        _StarDensity   ("Star Density", Range(50, 800)) = 250
        _StarBrightness("Star Brightness", Range(0, 5)) = 1.5
        _StarTwinkleSpeed ("Star Twinkle Speed", Range(0, 10)) = 2
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
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

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

            float4 _NightSkyTop;
            float4 _NightSkyHorizon;
            float4 _NightSkyBottom;

            float  _DayNightSharpness;

            float  _FogSkyBlend;
            float  _FogSkyPower;

            float4 _SunColor;
            float  _SunSize;
            float  _SunFalloff;
            float4 _SunGlowColor;
            float  _SunGlowSize;
            float  _SunGlowIntensity;

            float4 _MoonColor;
            float  _MoonSize;
            float  _MoonFalloff;

            float  _StarDensity;
            float  _StarBrightness;
            float  _StarTwinkleSpeed;

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                OUT.viewDirWS = normalize(TransformObjectToWorldDir(IN.positionOS.xyz));
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            // simple hash for stars
            float Hash21(float2 p)
            {
                p = frac(p * float2(234.34, 435.345));
                p += dot(p, p + 34.23);
                return frac(p.x * p.y);
            }

            half3 GradientSky(float h, float3 top, float3 horizon, float3 bottom, float sharpness)
            {
                half3 col;
                if (h > 0)
                {
                    float t = pow(saturate(h), 1.0 / max(sharpness, 0.001));
                    col = lerp(horizon, top, t);
                }
                else
                {
                    float t = pow(saturate(-h), 1.0 / max(sharpness, 0.001));
                    col = lerp(horizon, bottom, t);
                }
                return col;
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                float3 viewDir = normalize(IN.viewDirWS);
                float h = viewDir.y;

                // === Main light (Directional Light "Sun") direction ===
                Light mainLight = GetMainLight();
                float3 sunDir = normalize(mainLight.direction); // points FROM surface TOWARD the light

                // Sun height above horizon, used for day/night blend
                float sunHeight = sunDir.y;
                float dayFactor = saturate(sunHeight * _DayNightSharpness * 0.5 + 0.5);
                // sharper transition around horizon
                dayFactor = smoothstep(0.0, 1.0, saturate((sunHeight + 0.15) * _DayNightSharpness));

                // === Sky gradients ===
                half3 daySky   = GradientSky(h, _SkyTop.rgb, _SkyHorizon.rgb, _SkyBottom.rgb, _HorizonSharpness);
                half3 nightSky = GradientSky(h, _NightSkyTop.rgb, _NightSkyHorizon.rgb, _NightSkyBottom.rgb, _HorizonSharpness);
                half3 skyCol = lerp(nightSky, daySky, dayFactor);

                // === Sun disk + glow ===
                float sunDot = saturate(dot(viewDir, sunDir));
                float sunDisk = pow(sunDot, _SunFalloff / max(_SunSize, 0.0001));
                float sunGlow = pow(sunDot, _SunGlowSize) * _SunGlowIntensity;

                half3 sunContribution = _SunColor.rgb * sunDisk + _SunGlowColor.rgb * sunGlow;
                sunContribution *= dayFactor > 0.02 ? 1.0 : dayFactor * 50.0; // fade sun quickly below horizon
                sunContribution = max(sunContribution, 0);

                // === Moon (opposite direction of the sun) ===
                float3 moonDir = -sunDir;
                float moonDot = saturate(dot(viewDir, moonDir));
                float moonDisk = pow(moonDot, _MoonFalloff / max(_MoonSize, 0.0001));
                half3 moonContribution = _MoonColor.rgb * moonDisk * (1.0 - dayFactor);

                // === Stars (only visible at night, above horizon) ===
                half3 starContribution = 0;
                if (h > 0)
                {
                    float2 starUV = viewDir.xz / (viewDir.y + 1.0) * _StarDensity;
                    float2 cell = floor(starUV);
                    float2 f = frac(starUV);
                    float starRand = Hash21(cell);
                    float starMask = step(0.995, starRand);

                    float twinkle = 0.5 + 0.5 * sin(_Time.y * _StarTwinkleSpeed + starRand * 100.0);
                    float dist = length(f - 0.5);
                    float starShape = smoothstep(0.15, 0.0, dist);

                    starContribution = starMask * starShape * twinkle * _StarBrightness * (1.0 - dayFactor);
                }

                half3 celestialCol = skyCol + sunContribution + moonContribution + starContribution;

                // === Fog blend ===
                half3 fogColor = unity_FogColor.rgb;
                float fogAmount = 1.0 - saturate(abs(h));
                fogAmount = pow(fogAmount, _FogSkyPower) * _FogSkyBlend;

                // Don't let fog eat the sun disk itself too much (keeps it looking crisp near horizon)
                fogAmount *= saturate(1.0 - sunDisk);

                half3 finalColor = lerp(celestialCol, fogColor, fogAmount);

                return half4(finalColor, 1);
            }
            ENDHLSL
        }
    }
}