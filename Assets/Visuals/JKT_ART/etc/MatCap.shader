Shader "Universal Render Pipeline/MatCap"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _MatCap ("MatCap (RGB)", 2D) = "white" {}
        [Toggle(MATCAP_ACCURATE)] _MatCapAccurate ("Accurate Calculation", Int) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        Pass
        {
            Name "MatCap"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local _MATCAP_ACCURATE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata_tangent
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv_bump : TEXCOORD1;
                float3 viewNormal : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);
            TEXTURE2D(_MatCap);
            SAMPLER(sampler_MatCap);

            float4 _MainTex_ST;
            float4 _BumpMap_ST;

            v2f vert (appdata_tangent v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv_bump = TRANSFORM_TEX(v.uv, _BumpMap);
                o.viewNormal = TransformObjectToWorldNormal(v.normal);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half3 normal = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, i.uv_bump));

                float3 viewNormal = normalize(i.viewNormal);
                float2 mcUV = viewNormal.xy * 0.5 + 0.5;

                half4 matcap = SAMPLE_TEXTURE2D(_MatCap, sampler_MatCap, mcUV);
                return tex * matcap;
            }
            ENDHLSL
        }
    }
}
