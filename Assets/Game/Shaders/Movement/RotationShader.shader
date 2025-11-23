Shader "Custom/GPURotation3D_NoDeform"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _Color;
            float _Rotation;

            // Pure Y-axis rotation matrix (orthogonal)
            float3x3 RotateY(float angle)
            {
                float c = cos(angle);
                float s = sin(angle);

                return float3x3(
                    c, 0, -s,
                    0, 1,  0,
                    s, 0,  c
                );
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normalWS : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;

                float3x3 rot = RotateY(_Rotation);

                // Rotate locally
                float3 localPos = mul(rot, v.vertex.xyz);
                float3 localNormal = mul(rot, v.normal);

                // Apply Unity's standard transforms afterward
                float4 worldPos = mul(unity_ObjectToWorld, float4(localPos, 1.0));
                o.pos = mul(UNITY_MATRIX_VP, worldPos);

                // Correct world normal transform
                o.normalWS = mul((float3x3)unity_ObjectToWorld, localNormal);

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                // Simple diffuse light to see 3D shape
                float3 lightDir = normalize(float3(0.3, 1.0, 0.3));
                float nDotL = saturate(dot(normalize(i.normalWS), lightDir));

                return _Color * (0.25 + nDotL * 0.75);
            }

            ENDCG
        }
    }
}
