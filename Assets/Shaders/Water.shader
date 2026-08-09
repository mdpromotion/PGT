Shader "ProceduralWorld/Water"
{
    Properties
    {
        _MaskHeightTex ("Mask/Height/Bank", 2D) = "black" {}
        _Color ("Water Color (Deep)", Color) = (0.005, 0.04, 0.055, 0.9)
        _ShallowColor ("Water Color (Shallow)", Color) = (0.1, 0.5, 0.5, 1)
        _Opacity ("Water Opacity", Range(0,1)) = 0.85
        _MaskExpand ("Mask Expand", Range(0,4)) = 1
        _EdgeSoftness ("Edge Softness", Range(0,1)) = 0.08

        [Header(Depth)]
        _DepthMaxDistance ("Depth Color Max Distance", Float) = 3
        _AlphaFadeDistance ("Alpha Fade Distance", Float) = 0.5

        [Header(Lighting)]
        _MinBrightness ("Min Brightness (night floor)", Range(0,1)) = 0.15
        _Shininess ("Specular Sharpness", Range(1,256)) = 64
        _SpecularStrength ("Specular Strength", Range(0,2)) = 0.6

        [Header(Waves)]
        _WaveSpeed ("Wave Speed", Range(0,5)) = 1.0
        _WaveHeight ("Wave Height", Range(0,1)) = 0.08
        _WaveFrequency ("Wave Frequency", Range(0,10)) = 1.5
        _WaveScale2 ("Secondary Wave Scale", Range(0,10)) = 2.7

        [Header(Reflection)]
        _ReflectionStrength ("Reflection Strength", Range(0,1)) = 0.5
        _FresnelPower ("Fresnel Power", Range(0.5,8)) = 3
        _Smoothness ("Surface Smoothness", Range(0,1)) = 0.85
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            TEXTURE2D(_MaskHeightTex);
            SAMPLER(sampler_MaskHeightTex);
            float4 _MaskHeightTex_TexelSize;

            half4 _Color;
            half4 _ShallowColor;
            float _Opacity;
            float _MaskExpand;
            float _EdgeSoftness;
            float _MinBrightness;
            float _Shininess;
            float _SpecularStrength;

            float _DepthMaxDistance;
            float _AlphaFadeDistance;

            float _WaveSpeed;
            float _WaveHeight;
            float _WaveFrequency;
            float _WaveScale2;

            float _ReflectionStrength;
            float _FresnelPower;
            float _Smoothness;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 posWS : TEXCOORD2;
                float fogCoord : TEXCOORD3;
                float4 screenPos : TEXCOORD4;
            };

            float WaveHeightAt(float2 posXZ, float time)
            {
                float w1 = sin(posXZ.x * _WaveFrequency + time * _WaveSpeed)
                         * cos(posXZ.y * _WaveFrequency * 0.8 - time * _WaveSpeed * 0.7);

                float w2 = sin((posXZ.x + posXZ.y) * _WaveScale2 - time * _WaveSpeed * 1.3) * 0.5;

                return (w1 + w2) * _WaveHeight;
            }

            v2f vert(appdata v)
            {
                v2f o;

                VertexPositionInputs posInputs = GetVertexPositionInputs(v.vertex.xyz);
                float3 worldPos = posInputs.positionWS;
                float t = _Time.y;

                float h = WaveHeightAt(worldPos.xz, t);
                worldPos.y += h;

                float eps = 0.15;
                float hX = WaveHeightAt(worldPos.xz + float2(eps, 0), t);
                float hZ = WaveHeightAt(worldPos.xz + float2(0, eps), t);

                float3 tangentX = normalize(float3(eps, hX - h, 0));
                float3 tangentZ = normalize(float3(0, hZ - h, eps));
                float3 waveNormal = normalize(cross(tangentZ, tangentX));

                VertexNormalInputs normInputs = GetVertexNormalInputs(v.normal);
                float3 baseN = normInputs.normalWS;
                float3 finalNormal = normalize(lerp(baseN, waveNormal, 0.85));

                o.pos = TransformWorldToHClip(worldPos);
                o.posWS = worldPos;
                o.normalWS = finalNormal;
                o.uv = v.uv;
                o.fogCoord = ComputeFogFactor(o.pos.z);
                o.screenPos = ComputeScreenPos(o.pos);

                return o;
            }

            float SampleMask(float2 uv)
            {
                return SAMPLE_TEXTURE2D(_MaskHeightTex, sampler_MaskHeightTex, uv).r;
            }

            float SampleDilatedMask(float2 uv)
            {
                float2 t = _MaskHeightTex_TexelSize.xy * _MaskExpand;

                float m = SampleMask(uv);

                m = max(m, SampleMask(uv + float2( t.x, 0)));
                m = max(m, SampleMask(uv + float2(-t.x, 0)));
                m = max(m, SampleMask(uv + float2(0,  t.y)));
                m = max(m, SampleMask(uv + float2(0, -t.y)));

                m = max(m, SampleMask(uv + float2( t.x,  t.y)));
                m = max(m, SampleMask(uv + float2(-t.x,  t.y)));
                m = max(m, SampleMask(uv + float2( t.x, -t.y)));
                m = max(m, SampleMask(uv + float2(-t.x, -t.y)));

                return saturate(m);
            }

            half4 frag(v2f i) : SV_Target
            {
                float mask = SampleDilatedMask(i.uv);
                float edge = smoothstep(0.0, _EdgeSoftness, mask);

                float3 N = normalize(i.normalWS);
                float3 V = normalize(GetWorldSpaceViewDir(i.posWS));

                Light mainLight = GetMainLight();
                float3 L = normalize(mainLight.direction);

                float NdotL = dot(N, L) * 0.5 + 0.5;
                float diffuse = NdotL * NdotL;

                float3 H = normalize(L + V);
                float spec = pow(saturate(dot(N, H)), _Shininess) * _SpecularStrength;

                float3 ambient = SampleSH(N);

                float3 lighting = mainLight.color * diffuse + ambient;
                lighting = max(lighting, _MinBrightness);
                
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                float rawDepth = SampleSceneDepth(screenUV);
                float sceneEyeDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
                float surfaceEyeDepth = i.screenPos.w;
                float depthDifference = max(sceneEyeDepth - surfaceEyeDepth, 0);

                float depthColorFactor = saturate(depthDifference / _DepthMaxDistance);
                half3 waterColor = lerp(_ShallowColor.rgb, _Color.rgb, depthColorFactor);

                float depthAlphaFactor = saturate(depthDifference / _AlphaFadeDistance);
                float baseAlpha = lerp(_ShallowColor.a, _Color.a, depthColorFactor) * _Opacity;

                float3 reflectDir = reflect(-V, N);
                float roughness = 1.0 - _Smoothness;
                half3 reflection = GlossyEnvironmentReflection(reflectDir, i.posWS, roughness, 1.0);

                float fresnel = pow(1.0 - saturate(dot(N, V)), _FresnelPower);
                float reflAmount = saturate(fresnel * _ReflectionStrength);

                half3 baseWater = waterColor * lighting;
                half3 finalColor = lerp(baseWater, reflection, reflAmount) + spec * mainLight.color;

                finalColor = MixFog(finalColor, i.fogCoord);

                half4 outColor;
                outColor.rgb = finalColor;
                outColor.a = edge * baseAlpha * depthAlphaFactor;

                return outColor;
            }
            ENDHLSL
        }
    }
}