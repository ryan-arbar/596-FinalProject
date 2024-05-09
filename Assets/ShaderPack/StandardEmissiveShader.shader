Shader "Custom/StandardEmissiveShader"
{
    Properties
    {

        [Header(Albedo)][Space][MainTexture]
        _Color ("Color", Color) = (1,1,1,1)
        // controls current color
        _MainTex ("Albedo (RGB), Alpha(A)", 2D) = "white" {}
        // tutorial uses variable name "_Albedo", changing it helps my personal understanding 
        
        [Header(Details)][Space][NoScaleOffset][Normal]
        _Normal ("Normal(RGB)", 2D) = "bump" {}
        // handles the bumps and bends of a 3D material
        [NoScaleOffset]
        _MaskMap ("Mask Map (Metallic, Occlusion, Detail Mask, Smoothness)", 2D) = "black" {} 
        // not all of these names are necessary for functionality but help aesthetically
    
        [Header(Emission)][Space][NoScaleOffset]
        // the fun additions to our shader!
        _Emission("Emission", 2D) = "black" {}
        _EmissionColor("Color", Color) = (1,1,1,1) // aka white
        _EmissionIntensity("Intensity", Float) = 1.0
        _EmissionGlow("Glow", Float) = 1.0 // how bright to glow
        _EmissionGlowDuration("GlowDuration", Float) = 5.0 // duration = 5 seconds how long to pulse


        [Header(Mask)][Space]
        // this will help with pulse
        _ScrollingMask("Scrolling Mask", 2D) = "white" {}
        _ScrollX("Scroll Speed(X)", Float) = 0.2
        _ScrollY("Scroll Speed(Y)", Float) = 0.2
    }   
    SubShader
    {
        Tags 
        { 
            "Queue" = "Geometry"
            "RenderType"="Opaque" 
        }
        
        CGPROGRAM
        
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        #include "UnityPBSLighting.cginc"
        #pragma surface surf Standard
        #pragma exclude_renderers gles

        sampler2D _MainTex;
        sampler2D _Normal;
        sampler2D _MaskMap;
        fixed4 _Color;

        sampler2D _Emission;
        float4 _EmissionColor;
        float _EmissionIntensity;
        float _EmissionGlow;
        float _EmissionGlowDuration;

        

        struct Input
        {
            float2 uv_MainTex;
        };

        
        

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 albedo = tex2D(_MainTex, IN.uv_MainTex);
            fixed3 normal = UnpackScaleNormal(tex2D(_Normal, IN.uv_MainTex), 1);
            fixed4 mask = tex2D(_MaskMap, IN.uv_MainTex);
            
            fixed4 emission = tex2D(_Emission, IN.uv_MainTex);



            o.Albedo = albedo.rgb * _Color;
            o.Alpha = albedo.a;
            o.Normal = normal;
            // our mask handles these variables in the repective spots
            o.Metallic = mask.r; 
            o.Occlusion = mask.g;
            // o.DetailMask = mask.b; // this would go here but we don't need it
            o.Smoothness = mask.a;

            o.Emission = emission.rgb * _EmissionColor * (_EmissionIntensity + abs(frac(_Time.y * (1/_EmissionGlowDuration)) - 0.5) * _EmissionGlow); 
            // o.Emission holds the result of our changes


        }
        ENDCG
        Pass
        {
            
            Tags{"RenderType" = "Fade"}
            LOD 200
            ZWrite On
            Blend DstColor Zero

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
            };

            sampler2D _ScrollingMask;
            float4 _ScrollingMask_ST;

            float _ScrollX;
            float _ScrollY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _ScrollingMask);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                i.uv.x += _Time.y * _ScrollX;
                i.uv.y += _Time.y * _ScrollY;
                // sample the texture
                fixed4 col = tex2D(_ScrollingMask, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
