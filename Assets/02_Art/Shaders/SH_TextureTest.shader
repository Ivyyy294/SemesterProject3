Shader "Unlit/TextureTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _3DTex ("3D Texture", 3D) = "white" {}
        _Threshold ("Threshold", Float) = 0.5
        _Tile ("Tile", Float) = 1
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler3D _3DTex;
            float _Threshold;
            float _Tile;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv * _Tile);
                fixed4 col3D = tex3D(_3DTex, float3(i.uv * _Tile, _Time.y * 0.1 - 0.0032));
                // col3D += tex3D(_3DTex, float3(i.uv * _Tile * 2 + 0.1, _Time.y * 0.1 + 0.112)) * .5;
                // col3D += tex3D(_3DTex, float3(i.uv * _Tile * 4 - 0.4, _Time.y * 0.1 + 34.124)) * .25;
                // col3D += tex3D(_3DTex, float3(i.uv * _Tile * 8 + 4.51, _Time.y * 0.1)) * .125;
                // col3D *= .6;
                // col3D = col3D * 2 - 1;
                col = tex2D(_MainTex, (i.uv * _Tile));
                float t = col.r > _Threshold? 1.0:0.0;
                float s = col3D.r > _Threshold? 1:0;
                col.rgb = 1 - col3D.rgb;
                return float4(col.rgb, 1);
            }
            ENDCG
        }
    }
}
