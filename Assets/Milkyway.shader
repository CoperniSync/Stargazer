Shader "Custom/MilkyWayShader"
{
    Properties
    {
        _MainTex ("Milky Way Texture", 2D) = "black" {}
        _Brightness ("Brightness", Range(0, 1)) = 0.3
        _Contrast ("Contrast", Range(0.5, 2)) = 1.2
        _TintColor ("Tint Color", Color) = (0.8, 0.88, 1, 1)
        _UseExtinction ("Use Extinction", Float) = 1
        _ExtinctionCoeff ("Extinction Coefficient", Range(0.1, 0.6)) = 0.25
    }
    
    SubShader
    {
        Tags { "Queue"="Background+1" "RenderType"="Transparent" }
        
        // Render after skybox but before stars
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Front  // We're inside the sphere, cull front faces
        
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
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
            };
            
            sampler2D _MainTex;
            float _Brightness;
            float _Contrast;
            float4 _TintColor;
            float _UseExtinction;
            float _ExtinctionCoeff;
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                // Sample texture
                fixed4 tex = tex2D(_MainTex, i.uv);
                
                // Apply contrast
                float3 color = pow(tex.rgb, _Contrast);
                
                // Apply tint
                color *= _TintColor.rgb;
                
                // Calculate altitude for extinction
                float3 viewDir = normalize(i.worldPos - _WorldSpaceCameraPos);
                float altitude = viewDir.y;  // Y is up, so this is sin(altitude)
                
                // Extinction dimming near horizon
                float extinction = 1.0;
                if (_UseExtinction > 0.5)
                {
                    if (altitude >= 0)
                    {
                        // More dimming at lower altitudes
                        float altDimming = saturate(altitude);  // cos(90-alt) = sin(alt)
                        altDimming = pow(altDimming, 0.5);  // Soften the falloff
                        
                        // Extinction based on coefficient
                        float minExt = 0.1;
                        float maxExt = 0.6;
                        float extDimming = 1.0 - (_ExtinctionCoeff - minExt) / (maxExt - minExt);
                        
                        extinction = altDimming * extDimming;
                    }
                    else
                    {
                        // Below horizon - fade out
                        extinction = saturate(1.0 + altitude * 5.0);
                    }
                }
                
                // Final alpha
                float alpha = _Brightness * extinction * tex.a;
                
                // Threshold to avoid rendering invisible pixels
                if (alpha < 0.01) discard;
                
                return fixed4(color * _Brightness * extinction, alpha);
            }
            ENDCG
        }
    }
}
