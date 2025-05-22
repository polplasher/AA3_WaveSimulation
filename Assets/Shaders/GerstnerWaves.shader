Shader "Custom/GerstnerWaves"
{
    Properties
    {
        _Color ("Base Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _SpecularColor ("Specular Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _Glossiness ("Smoothness", Range(8.0, 256)) = 20
        _FresnelPower ("Fresnel Power", Range(1.0, 10.0)) = 5.0
        _WaterTexture ("Water Texture", 2D) = "white" {}
        _TextureScale ("Texture Scale", Float) = 5.0
        _Amplitude ("Amplitude (A)", Float) = 1.0
        _WaveLength ("Wave Length (L)", Float) = 10.0
        _Direction ("Direction", Vector) = (1.0, 0.0, 0.0, 0.0)
        _Speed ("Speed (v)", Float) = 1.0
        _Phase ("Phase (φ)", Float) = 0.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "Queue"="Transparent"
        }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            fixed4 _Color;
            fixed4 _SpecularColor;
            float _Glossiness;
            float _FresnelPower;
            sampler2D _WaterTexture;
            float _TextureScale;

            float _Amplitude;
            float _WaveLength;
            float4 _Direction;
            float _Speed;
            float _Phase;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 normal : NORMAL;
                float3 viewDir : TEXCOORD2;
                UNITY_FOG_COORDS(3)
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;

                // Vertex position in world space
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                // Normalize direction
                float2 dir = normalize(_Direction.xy);

                // Calculate x term
                float x = worldPos.x * dir.x + worldPos.z * dir.y;

                // Implementation of formula: A sin(2π/L(x − vt) + φ)
                float k = 2.0 * UNITY_PI / _WaveLength; // Wave number (2π/L)
                float f = k * (x - _Speed * _Time.y) + _Phase;
                float displacement = _Amplitude * sin(f);

                // Apply displacement only to Y (height)
                worldPos.y += displacement;

                // Transform back to local space
                float4 localPos = mul(unity_WorldToObject, float4(worldPos, 1.0));

                // Calculate more precise normals for waves
                float3 normal = v.normal;
                normal.x = -dir.x * k * _Amplitude * cos(f);
                normal.z = -dir.y * k * _Amplitude * cos(f);
                normal.y = 1.0;
                normal = normalize(normal);

                // Final output
                o.vertex = UnityObjectToClipPos(localPos);
                o.uv = v.uv;
                o.worldPos = worldPos;
                o.normal = UnityObjectToWorldNormal(normal);
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);

                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Sample animated texture
                float2 uv = i.worldPos.xz / _TextureScale;
                float2 uv1 = uv + _Time.y * float2(0.03, 0.04);
                float2 uv2 = uv * 0.8 - _Time.y * float2(0.04, 0.03);
                float3 texColor = tex2D(_WaterTexture, uv1).rgb * 0.6 + tex2D(_WaterTexture, uv2).rgb * 0.4;

                // Basic lighting calculation (Lambert)
                float3 normal = normalize(i.normal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float ndotl = max(0, dot(normal, lightDir));
                float3 diffuse = ndotl * _LightColor0.rgb;

                // Specular calculation (Blinn-Phong)
                float3 halfVector = normalize(lightDir + i.viewDir);
                float ndoth = max(0, dot(normal, halfVector));
                float3 specular = pow(ndoth, _Glossiness) * _SpecularColor.rgb * _LightColor0.rgb;

                // Fresnel effect (more reflective at grazing angles)
                float fresnel = pow(1.0 - max(0.0, dot(normal, i.viewDir)), _FresnelPower);

                // Final combined color
                float3 finalColor = lerp(_Color.rgb * texColor * diffuse, _SpecularColor.rgb, fresnel) + specular;

                fixed4 col = fixed4(finalColor, 1.0);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}