Shader "Unlit/Raymarch"
{
    Properties
    {

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        Cull Front

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

            #define MAX_STEPS 128
            #define SURF_DIST 0.001
            #define MAX_DIST 64.0
            #include "RaymarchingUtils.cginc"
            
            float GetDist(float3 ro)
            {
                float s1 = sdSphere(ro, float3(0,-.3,0), 0.2);
                float y = sin(_Time.y * 3) * 0.2;
                float s2 = sdSphere(ro, float3(0, y, 0), 0.15);
                return sdUnionSmooth(s2, s1, 0.05);
            }
            float3 GetNormal(float3 p)
            {
                float d = GetDist(p);
                float2 e = float2(0.01, 0.0);
                float3 n = d - float3(
                    GetDist(p-e.xyy),
                    GetDist(p-e.yxy),
                    GetDist(p-e.yyx));
                return normalize(n);
            }

            float RayMarch(float3 ro, float3 rd)
            {
                float d = 0.0;
                float dS;
                for (int i = 0; i < MAX_STEPS; i++)
                {
                    float3 p = ro + rd * d;
                    dS = GetDist(p);
                    d += dS;
                    if (d>MAX_DIST || dS<SURF_DIST) break;
                }
                return d;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                
                fixed4 col = float4(1,1,1,1);
                float3 ro = _WorldSpaceCameraPos;
                ro = mul(unity_WorldToObject, ro);
                float3 localPos = mul(unity_WorldToObject, i.worldPos);
                float3 rd = normalize(localPos - ro);
                float d = RayMarch(ro, rd);
                bool hasHit = d < MAX_DIST;
                clip(hasHit - 0.5);

                float3 normal = GetNormal(ro + rd * d);
                float3 lightDir = normalize(float3(.5, .5, 0));
                float lambert = saturate(dot(normal, lightDir));
                col.rgb = lerp(float3(.5,.5,.9), float3(1,1,1), lambert);
                return col;
            }
            ENDCG
        }
    }
}
