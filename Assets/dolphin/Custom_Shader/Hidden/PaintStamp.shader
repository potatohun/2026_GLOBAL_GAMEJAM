Shader "Hidden/PaintStamp"
{
    Properties
    {
        _MainTex ("OldPaint", 2D) = "black" {}
        _EraseMask ("EraseMask", 2D) = "black" {}

        _CenterUV ("CenterUV", Vector) = (0.5,0.5,0,0)
        _Radius ("Radius", Float) = 0.05
        _Hardness ("Hardness", Float) = 0.8

        _BrushColor ("BrushColor", Color) = (1,0,0,1)
        _Opacity ("Opacity", Float) = 1
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

            sampler2D _MainTex;     // 기존 paintRT
            sampler2D _EraseMask;   // maskRT (1=지움)

            float4 _CenterUV;
            float _Radius;
            float _Hardness;

            fixed4 _BrushColor;
            float _Opacity;

            struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
            struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; };

            v2f vert(appdata v) { v2f o; o.pos = UnityObjectToClipPos(v.vertex); o.uv = v.uv; return o; }

            float BrushValue(float2 uv)
            {
                float d = distance(uv, _CenterUV.xy);
                float inner = _Radius * _Hardness;
                float t = saturate((d - inner) / max(1e-6, (_Radius - inner)));
                return 1.0 - t; // 1 -> 0
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 oldPaint = tex2D(_MainTex, i.uv);

                // 지워진 영역이면(마스크가 1에 가까우면) 절대 그리지 않음
                float erased = tex2D(_EraseMask, i.uv).r; // 0~1
                if (erased > 0.5)
                    return oldPaint;

                float b = BrushValue(i.uv) * saturate(_Opacity);
                if (b <= 0.0001) return oldPaint;

                // 누적: oldPaint 위에 brushColor를 알파 블렌딩처럼 덮기
                fixed4 col = _BrushColor;
                col.a *= b;

                // “그림판” 느낌: 알파 누적 + 색상 덮기
                fixed a = 1 - (1 - oldPaint.a) * (1 - col.a);
                fixed3 rgb = (oldPaint.rgb * oldPaint.a + col.rgb * col.a) / max(1e-5, (oldPaint.a + col.a));
                return fixed4(rgb, a);
            }
            ENDCG
        }
    }
}
