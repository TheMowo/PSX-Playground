Shader "Custom/GhostShell"
{
    Properties
    {
        [MainTexture]
        _MainTex("Glyph Atlas", 2D) = "white" {}

        _GridSize("Grid Size", Float) = 90
        _MutationRate("Mutation Rate", Float) = 8
        _Brightness("Brightness", Float) = 2
        _ScanSpeed("Scan Speed", Float) = 1
        _FlickerStrength("Flicker Strength", Range(0,1)) = 0.15

        _Tint("Tint", Color) = (0.3,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Blend One One
            Cull Off
            ZWrite Off

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _GridSize;
            float _MutationRate;
            float _Brightness;
            float _ScanSpeed;
            float _FlickerStrength;

            float4 _Tint;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;

                output.positionCS =
                    TransformObjectToHClip(
                        input.positionOS.xyz);

                output.uv = input.uv;

                return output;
            }

            float Hash(float2 p)
            {
                return frac(
                    sin(dot(
                        p,
                        float2(127.1,311.7)))
                    * 43758.5453123);
            }

            float4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;

                //----------------------------------
                // Character Grid
                //----------------------------------

                float2 cell =
                    floor(uv * _GridSize);

                float2 localUV =
                    frac(uv * _GridSize);

                //----------------------------------
                // Character Mutation
                //----------------------------------

                float mutationFrame =
                    floor(
                        _Time.y *
                        _MutationRate);

                float glyphIndex =
                    floor(
                        Hash(
                            cell +
                            mutationFrame)
                        * 256);

                float2 atlasCell;

                atlasCell.x =
                    fmod(glyphIndex,16);

                atlasCell.y =
                    floor(glyphIndex / 16);

                float2 glyphUV =
                    (localUV + atlasCell)
                    / 16.0;

                float glyph =
                    SAMPLE_TEXTURE2D(
                        _MainTex,
                        sampler_MainTex,
                        glyphUV).r;

                //----------------------------------
                // Horizontal Scanner
                //----------------------------------

                float scanner =
                    exp(
                        -pow(
                            frac(
                                uv.y -
                                _Time.y *
                                _ScanSpeed)
                            * 6,
                            2));

                //----------------------------------
                // CRT Scanlines
                //----------------------------------

                float scanlines =
                    0.85 +
                    0.15 *
                    sin(
                        uv.y *
                        1800);

                //----------------------------------
                // Flicker
                //----------------------------------

                float flicker =
                    lerp(
                        1.0 -
                        _FlickerStrength,

                        1.0 +
                        _FlickerStrength,

                        Hash(
                            cell +
                            floor(_Time.y * 20)));

                //----------------------------------
                // Final Brightness
                //----------------------------------

                float brightness =
                    _Brightness *
                    scanlines *
                    flicker;

                brightness +=
                    scanner * 3.0;

                //----------------------------------
                // Color
                //----------------------------------

                float3 color =
                    glyph *
                    _Tint.rgb *
                    brightness;

                return float4(
                    color,
                    glyph);
            }

            ENDHLSL
        }
    }
}