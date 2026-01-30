Shader "Hidden/EraseStamp"
{
    Properties
    {
        _MainTex ("OldMask", 2D) = "black" {}
        _CenterUV ("CenterUV", Vector) = (0.5,0.5,0,0)
        _Radius ("Radius", Float) = 0.05
        _Hardness ("Hardness", Float) = 0.8

        _BaseTex ("BaseTex", 2D) = "white" {}
        _AlphaThreshold ("AlphaThreshold", Float) = 0.05
        _UseAlphaLimit ("UseAlphaLimit", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }

        Pass
        {
            ZTest Always
            ZWrite Off
            Cull Off
            Blend Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _CenterUV;
            float _Radius;
            float _Hardness;

            sampler2D _BaseTex;
            float _AlphaThreshold;
            float _UseAlphaLimit;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv;
                return o;
            }

            float BrushValue(float2 uv)
            {
                float d = distance(uv, _CenterUV.xy);

                float inner = _Radius * _Hardness;
                float t = saturate((d - inner) / max(1e-6, (_Radius - inner)));

                return 1.0 - t;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float oldMask = tex2D(_MainTex, i.uv).r;

                // (선택) 스프라이트의 알파가 없는 곳엔 기록 금지
                if (_UseAlphaLimit > 0.5)
                {
                    float baseA = tex2D(_BaseTex, i.uv).a;
                    if (baseA < _AlphaThreshold)
                        return fixed4(oldMask, oldMask, oldMask, 1);
                }

                float add = BrushValue(i.uv);
                float m = max(oldMask, add);
                return fixed4(m, m, m, 1);
            }
            ENDCG
        }
    }
}
