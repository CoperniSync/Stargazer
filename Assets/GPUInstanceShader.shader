Shader "Custom/GPUInstanceShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
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
            #pragma multi_compile_instancing
            #pragma target 4.5

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                uint instanceID : SV_InstanceID;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            StructuredBuffer<float3> positionBuffer;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                
                float3 worldPos = positionBuffer[v.instanceID];
                
                float3 vertexWorld = v.vertex.xyz + worldPos;
                
                o.vertex = UnityWorldToClipPos(float4(vertexWorld, 1.0));
                
                float hue = frac(v.instanceID * 0.618033988749895);
                o.color = float4(hue, 1.0 - hue, 0.5, 1.0);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.color * _Color;
            }
            ENDCG
        }
    }
}