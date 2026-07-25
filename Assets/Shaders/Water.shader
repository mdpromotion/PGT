Shader "ProceduralWorld/Water"
{
    Properties
    {
        _MaskHeightTex ("Mask/Height/Bank", 2D) = "black" {}
        _Color ("Water Color", Color) = (0.005, 0.04, 0.055, 0.9)
        _EdgeFadeStart ("Edge Fade Start", Range(0,1)) = 0.02
        _EdgeFadeEnd ("Edge Fade End", Range(0,1)) = 0.5
        _Opacity ("Water Opacity", Range(0,1)) = 0.85
        _RimColor ("Bank Foam Color", Color) = (0.8, 0.9, 0.95, 1)
        _RimWidth ("Bank Foam Width", Range(0,1)) = 0.08
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
            float _EdgeFadeStart;
            float _EdgeFadeEnd;
            float _Opacity;
            fixed4 _RimColor;
            float _RimWidth;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float mask = tex2D(_MaskHeightTex, i.uv).r;
                
                float edge = smoothstep(_EdgeFadeStart, _EdgeFadeEnd, mask);
                clip(edge - 0.001);
                
                float rim = smoothstep(0.0, _RimWidth, edge) *
                            (1.0 - smoothstep(_RimWidth, _RimWidth * 2.0, edge));

                fixed4 color = _Color;
                color.rgb = lerp(color.rgb, _RimColor.rgb, rim);
                color.a *= _Opacity * edge;

                return color;
            }
            ENDCG
        }
    }
}