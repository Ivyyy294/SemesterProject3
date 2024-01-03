Shader "Unlit/RaymarchTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BubblesTex ("Bubbles", 3D) = "white" {}
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
            // make fog work
            #pragma multi_compile_fog

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
                float3 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler3D _BubblesTex;

            #define MAX_STEPS 128
            #define SURF_DIST 0.001
            #define MAX_DIST 64.0
            #include "RaymarchingUtils.cginc"
            
            float GetDist(float3 ro)
            {
                // float bubbles = tex3D(_BubblesTex, ro + float3(0, _Time.y * 0.1, 0)).r - 0.3;
                // float bubbles = tex2D(_MainTex, ro.xz).r - 0.3;
                // return sdIntersect(bubbles, sdSphere(ro, float3(0,0,0), 0.5));
                
                float s1 = sdSphere(ro, float3(0,-.3,0), 0.2);
                float y = sin(_Time.y * 3) * 0.2;
                float s2 = sdSphere(ro, float3(0, y, 0), 0.15);
                return sdUnionSmooth(s2, s1, 0.05);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.worldPos = v.vertex.xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                float3 ro = _WorldSpaceCameraPos;
                float3 rd = normalize(i.worldPos - ro);

                RaymarchResult result;
                RAYMARCH(GetDist, result, ro, rd, DEF_MAX_STEPS, DEF_MAX_DIST, 0.01);
                
                clip(result.IsHit - 0.5);
                
                fixed4 col = tex2D(_MainTex, i.uv);
                
                return float4(result.Normal, 1);
            }
            ENDCG
        }
    }
}

