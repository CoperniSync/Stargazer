Shader "Custom/StarShader"
{
    Properties
    {
        _BaseColor ("Base Star Color", Color) = (1,1,1,1)
        _BrightnessMultiplier ("Brightness Multiplier", Range(0.5, 5)) = 3.0
        _GlowStrength ("Glow Strength", Range(0, 2)) = 0.4
        _CoreSize ("Core Size", Range(0.05, 0.5)) = 0.4
        _SpikeStrength ("Spike Strength", Range(0, 1)) = 0.1
        _ZoomFactor("Zoom Factor", Float) = 1.0
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100
        
        Blend One One  // additive blending?
        ZWrite Off
        Cull Off

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
                float2 uv : TEXCOORD0;
                uint instanceID : SV_InstanceID;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float visible : TEXCOORD1;
            };

            StructuredBuffer<float3> positionBuffer;
            StructuredBuffer<float3> scaleBuffer;
            StructuredBuffer<float4> colorBuffer;  
            StructuredBuffer<float> visibilityBuffer;
            
            float4 _BaseColor;
            float _BrightnessMultiplier;
            float _GlowStrength;
            float _CoreSize;
            float _SpikeStrength;
            float _ZoomFactor;

            v2f vert (appdata v)
            {
                v2f o;
                
                float3 worldPos = positionBuffer[v.instanceID];
                float3 scale = scaleBuffer[v.instanceID];
                float4 starColor = colorBuffer[v.instanceID];
                float visible = visibilityBuffer[v.instanceID];
                
                float3 camUp = UNITY_MATRIX_V[1].xyz;
                float3 camRight = UNITY_MATRIX_V[0].xyz;
                
                // Apply scale to vertex
                float3 scaledVertex = v.vertex.xyz * scale;
                
                // Billboard transformation
                float3 vertexWorld = worldPos 
                    + camRight * scaledVertex.x 
                    + camUp * scaledVertex.y;
                

                
                 
                // Transform to clip space
                o.vertex = UnityWorldToClipPos(float4(vertexWorld, 1.0));
                o.uv = v.uv;
                o.color = starColor;
                o.visible = visible;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (i.visible < 0.5) discard;
    
                if (i.color.a > 10.0) discard;
    
                float2 centered = i.uv * 2.0 - 1.0;
                float dist = length(centered);
    
                float coreSize = _CoreSize / (1.0 + (_ZoomFactor - 1.0) * 0.3);
                float core = 1.0 - smoothstep(0.0, coreSize, dist);
                core = pow(core, 0.3);
    
                float disk = smoothstep(_CoreSize * 0.8, _CoreSize * 0.5, dist);
    
                float glowBoost = 1.0 + (_ZoomFactor - 1.0) * 0.3;
                float glow = exp(-dist * 5.0) * _GlowStrength * glowBoost;

                // spikes are taken from a shadertoy example. 
                float angle = atan2(centered.y, centered.x);
                float spikes = pow(abs(cos(angle * 2.0)), 25.0);
                spikes *= smoothstep(0.5, 0.25, dist) * smoothstep(0.0, 0.1, dist);
    
                float spikeBoost = 1.0 + (_ZoomFactor - 1.0) * 0.5;
                spikes *= _SpikeStrength * spikeBoost;

                float brightness = disk * 3.0 + core * 2.0 + glow + spikes * 0.3;
                brightness *= i.color.a; 
                brightness *= _BrightnessMultiplier;
    
                fixed4 col = i.color * _BaseColor;
                col.rgb *= brightness;
    
                col.rgb = saturate(col.rgb);
    
                float coreBoost = smoothstep(_CoreSize * 0.6, 0.0, dist);
                col.rgb += coreBoost * i.color.rgb * 0.5;
    
                col.rgb = min(col.rgb, float3(3.0, 3.0, 3.0));
    
                col.a = saturate(brightness);
    
                if (brightness < 0.005) discard;

                return col;
            }
            ENDCG
        }
    }
    
    Fallback "Unlit/Transparent"
}