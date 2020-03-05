Shader "Custom/GhostShader"
{
    Properties 
    {
        _Outline("Outline", Range(0.5, 2)) = 1
        _Color ("Main Color", Color) = (1,1,1,1)
    }
    
    SubShader 
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 200

        CGPROGRAM
        #pragma surface surf NoLighting alpha vertex:vert fragment frag

        float _Outline;
        fixed4 _Color;

        struct Input
        {
            float2 uv_MainTex;
            fixed viewIntensity;
        };

        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
        {
            fixed4 c;
            c.rgb = s.Albedo;
            c.a = s.Alpha;
            return c;
        }

        void vert (inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input,o);

            float3 viewDir = ObjSpaceViewDir(v.vertex);
            o.viewIntensity = _Outline - saturate(dot(normalize(viewDir), v.normal));
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a * IN.viewIntensity;
        }
        ENDCG
        }

        Fallback "Transparent/VertexLit"
}