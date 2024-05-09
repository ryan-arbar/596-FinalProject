Shader "Custom/URPStandardEmissiveShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Normal ("Normal Map", 2D) = "bump" {}
        _MaskMap ("Mask Map", 2D) = "white" {}
        _Emission ("Emission", 2D) = "black" {}
        _EmissionColor ("Emission Color", Color) = (1, 1, 1, 1)
        _EmissionIntensity ("Emission Intensity", Float) = 1.0
        _EmissionGlow ("Glow", Float) = 1.0
        _EmissionGlowDuration ("Glow Duration", Float) = 5.0
        _ScrollingMask ("Scrolling Mask", 2D) = "white" {}
        _ScrollX ("Scroll Speed X", Float) = 0.1
        _ScrollY ("Scroll Speed Y", Float) = 0.1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : NORMAL;
                float3 positionWS : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS).xyz;
                return OUT;
            }

            TEXTURE2D(_MainTex);
            TEXTURE2D(_Emission);
            TEXTURE2D(_ScrollingMask);
            SAMPLER(sampler_MainTex);
            SAMPLER(sampler_Emission);
            SAMPLER(sampler_ScrollingMask);
            float4 _Color;
            float4 _EmissionColor;
            float _EmissionIntensity;
            float _EmissionGlow;
            float _EmissionGlowDuration;
            float _ScrollX, _ScrollY;

            float4 frag(Varyings IN) : SV_Target
            {
                float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Color;
                float2 scrollingUV = IN.uv + float2(_ScrollX, _ScrollY) * _Time.y;
                float4 emission = SAMPLE_TEXTURE2D(_Emission, sampler_Emission, scrollingUV);
                float glowFactor = sin(_Time.y * 2 * PI / _EmissionGlowDuration) * 0.5 + 0.5;
                color.rgb += emission.rgb * _EmissionColor.rgb * (_EmissionIntensity + _EmissionGlow * glowFactor);
                return color;
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
