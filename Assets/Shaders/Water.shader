Shader "ProceduralWorld/Water"
{
    Properties
    {
        _MaskHeightTex ("Mask/Height/Bank", 2D) = "black" {}
        _Color ("Water Color", Color) = (0.005, 0.04, 0.055, 0.9)
        _Opacity ("Water Opacity", Range(0,1)) = 0.85
        _MaskExpand ("Mask Expand", Range(0,4)) = 1
        _EdgeSoftness ("Edge Softness", Range(0,1)) = 0.08
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MaskHeightTex;
            float4 _MaskHeightTex_TexelSize;

            fixed4 _Color;
            float _Opacity;
            float _MaskExpand;
            float _EdgeSoftness;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float SampleMask(float2 uv)
            {
                return tex2D(_MaskHeightTex, uv).r;
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

            fixed4 frag(v2f i) : SV_Target
            {
                float mask = SampleDilatedMask(i.uv);

                float edge = smoothstep(0.0, _EdgeSoftness, mask);

                fixed4 color = _Color;
                color.a = edge * _Opacity;

                return color;
            }
            ENDCG
        }
    }
}