Shader "Custom/RetroInteractionZone"
{
    Properties
    {
        _Color ("Color", Color) = (0,1,1,1)

        _ScanlineDensity ("Scanline Density", Float) = 80
        _ScanlineSpeed ("Scanline Speed", Float) = 5

        _PulseSpeed ("Pulse Speed", Float) = 0.25
        _PulseWidth ("Pulse Width", Float) = 50

        _EmissionStrength ("Emission Strength", Float) = 2

        _CapFadeStart ("Cap Fade Start", Range(0,1)) = 0.7
        _CapFadeEnd ("Cap Fade End", Range(0,1)) = 0.9

        _FlickerStrength ("Flicker Strength", Range(0,0.2)) = 0.05
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Blend SrcAlpha One
        ZWrite Off
        Cull Off

        Pass
        {
            Name "Forward"

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalOS   : TEXCOORD0;
                float2 uv         : TEXCOORD1;
            };

            float4 _Color;

            float _ScanlineDensity;
            float _ScanlineSpeed;

            float _PulseSpeed;
            float _PulseWidth;

            float _EmissionStrength;

            float _CapFadeStart;
            float _CapFadeEnd;

            float _FlickerStrength;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                OUT.positionCS =
                    TransformObjectToHClip(IN.positionOS.xyz);

                OUT.normalOS = normalize(IN.normalOS);
                OUT.uv = IN.uv;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                //----------------------------------
                // Hide cylinder caps
                //----------------------------------

                float sideMask =
                    1.0 -
                    smoothstep(
                        _CapFadeStart,
                        _CapFadeEnd,
                        abs(IN.normalOS.y)
                    );

                //----------------------------------
                // Vertical fade
                //----------------------------------

                float verticalFade =
                    saturate(1.0 - uv.y);

                verticalFade = pow(verticalFade, 1.5);

                //----------------------------------
                // Moving scanlines
                //----------------------------------

                float scan =
                    sin(
                        uv.y * _ScanlineDensity
                        - _Time.y * _ScanlineSpeed
                    ) * 0.5 + 0.5;

                //----------------------------------
                // Upward moving energy pulse
                //----------------------------------

                float pulsePos =
                    frac(_Time.y * _PulseSpeed);

                float pulse =
                    exp(
                        -_PulseWidth *
                        abs(uv.y - pulsePos)
                    );

                //----------------------------------
                // Subtle hologram flicker
                //----------------------------------

                float flicker =
                    1.0 +
                    sin(_Time.y * 7.31) *
                    _FlickerStrength;

                //----------------------------------
                // Alpha
                //----------------------------------

                float alpha =
                    verticalFade *
                    lerp(0.8, 1.0, scan) *
                    sideMask;

                //----------------------------------
                // Emission
                //----------------------------------

                float3 emission =
                    _Color.rgb *
                    flicker *
                    (1.0 + pulse * _EmissionStrength);

                emission *= sideMask;

                return float4(emission, alpha);
            }

            ENDHLSL
        }
    }
}