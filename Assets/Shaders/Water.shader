Shader "ProceduralWorld/Water"
{
    Properties
    {
        _MaskHeightTex ("Mask/Height/Bank", 2D) = "black" {}
        _Color ("Water Color", Color) = (0.005, 0.04, 0.055, 0.9)
        _MaskThreshold ("Mask Cutoff", Range(0,1)) = 0.05
        _EdgeFade ("Edge Fade", Range(0.001,1)) = 0.2
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
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 sample = tex2D(_MaskHeightTex, i.uv).rgb;

                float mask = sample.r;

                clip(mask - 0.001);

                float edge = smoothstep(0.0, max(_EdgeFade, 0.0001f), mask);

                fixed4 color = _Color;
                color.a *= _Opacity * edge;

                return color;
            }
            ENDCG
        }
    }
}