Shader "Unlit/SpriteEraseDisplay"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _MaskTex ("Mask", 2D) = "black" {}
        _PaintTex ("Paint", 2D) = "black" {}
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _MaskTex;
            sampler2D _PaintTex;

            struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
            struct v2f { float4 pos:SV_POSITION; float2 uv:TEXCOORD0; };

            v2f vert(appdata v) { v2f o; o.pos = UnityObjectToClipPos(v.vertex); o.uv = v.uv; return o; }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 baseCol = tex2D(_MainTex, i.uv);
                float mask = tex2D(_MaskTex, i.uv).r;     // 1=지움
                fixed4 paint = tex2D(_PaintTex, i.uv);    // 그린 색(알파 포함)

                // 1) 지운 영역은 투명
                baseCol.a *= (1.0 - mask);

                // 2) 지워지지 않은 영역에 paint가 있으면 덮어쓰기(알파 블렌딩)
                // paint.a가 0이면 변화 없음
                baseCol.rgb = lerp(baseCol.rgb, paint.rgb, paint.a);
                // base alpha는 유지(지우개가 만든 투명도 우선)
                return baseCol;
            }
            ENDCG
        }
    }
}
