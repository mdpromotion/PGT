Shader "ProceduralWorld/Water"
{
    Properties
    {
        _MaskHeightTex ("Mask/Height/Bank", 2D) = "black" {}
        _Color ("Water Color", Color) = (0.005, 0.04, 0.055, 0.9)

        _MaskThreshold ("Mask Cutoff", Range(0,1)) = 0.05

        _EdgeFade ("Edge Fade", Range(0.01,1)) = 0.2
        _EdgeDepth ("Edge Depth", Range(0,0.05)) = 0.01

        _HeightScale ("Height Scale", Float) = 600
        _Opacity ("Water Opacity", Range(0,1)) = 0.85
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

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

            fixed4 _Color;
            float _MaskThreshold;
            float _EdgeFade;
            float _EdgeDepth;
            float _HeightScale;
            float _Opacity;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float mask : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;

                float3 sample = tex2Dlod(_MaskHeightTex, float4(v.uv, 0, 0)).rgb;

                float mask = sample.r;
                float waterHeight = sample.g;

                float center = smoothstep(0.0, _EdgeFade, mask);

                float finalHeight = waterHeight;
                
                finalHeight -= (1.0 - center) * _EdgeDepth;

                float4 displaced = v.vertex;
                displaced.y = finalHeight * _HeightScale;

                o.pos = UnityObjectToClipPos(displaced);
                o.uv = v.uv;
                o.mask = mask;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                clip(i.mask - _MaskThreshold);

                fixed4 color = _Color;
                color.a = _Opacity;

                return color;
            }

            ENDCG
        }
    }
}