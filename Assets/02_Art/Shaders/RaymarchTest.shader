Shader "Custom/RaymarchTest"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
//        Cull Front

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        // void vert(inout appdata_full v, out Input o)
        // {
        //     UNITY_INITIALIZE_OUTPUT(Input, o);
        //     o.worldPos = v.vertex.xyz;
        // }
        
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

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

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 ro = _WorldSpaceCameraPos;
            float3 rd = normalize(IN.worldPos - ro);

            float d = RayMarch(ro, rd);
            bool hasHit = d < MAX_DIST;
            clip(hasHit - 0.5);

            float3 normal = GetNormal(ro + rd * d);
            o.Albedo = normal;
            o.Metallic = 0;
            o.Smoothness = 0.5;
            o.Alpha = 0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
