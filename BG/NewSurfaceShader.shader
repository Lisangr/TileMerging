Shader "Custom/SmoothGradientShader"
{
    Properties
    {
        _Color1 ("Color 1", Color) = (1,0,0,1)
        _Color2 ("Color 2", Color) = (0,0,1,1)
        _Blend ("Blend Factor", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 _Color1; // ������ ���� ���������
            fixed4 _Color2; // ������ ���� ���������
            float _Blend;   // ����������� ����������

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex.xz * 0.5 + 0.5; // ��������� UV
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // �������� �� ������ UV � ������������ ����������
                fixed4 gradientColor = lerp(_Color1, _Color2, i.uv.y);
                return gradientColor;
            }
            ENDCG
        }
    }
}
