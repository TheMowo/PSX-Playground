Shader "Custom/MatrixRain"
{
    Properties
    {
        [MainTexture]
        _MainTex ("Glyph Atlas", 2D) = "white" {}

        _GridSize ("Grid Size", Float) = 80
        _TrailLength ("Trail Length", Float) = 18
        _Brightness ("Brightness", Float) = 3
        _Speed ("Speed", Float) = 1
        _Tint ("Tint", Color) = (0,1,0,1)
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
            Name "MatrixRain"

            Blend One One
            ZWrite Off
            Cull Off

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _GridSize;
            float _TrailLength;
            float _Brightness;
            float _Speed;

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
                    sin(dot(p, float2(12.9898, 78.233)))
                    * 43758.5453);
            }

            float4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.uv;

                //-------------------------------------
                // Character Grid Cell
                //-------------------------------------

                float2 cell =
                    floor(uv * _GridSize);

                //-------------------------------------
                // Stream Movement
                //-------------------------------------

                float columnRandom =
                    Hash(float2(cell.x, 0.0));

                float speed =
                    lerp(
                        0.5,
                        2.0,
                        columnRandom);

                float streamPos =
                    frac(
                        _Time.y *
                        _Speed *
                        speed);

                float head =
                    streamPos * _GridSize;

                float distanceFromHead =
                    head - cell.y;

                distanceFromHead =
                    fmod(
                        distanceFromHead + _GridSize,
                        _GridSize);

                //-------------------------------------
                // Fade Trail
                //-------------------------------------

                float trail =
                    saturate(
                        1.0 -
                        distanceFromHead /
                        _TrailLength);

                //-------------------------------------
                // Random Glyph Selection
                //-------------------------------------

                float glyphIndex =
                    floor(
                        Hash(
                            cell +
                            floor(_Time.y * 8.0))
                        * 256.0);

                float2 atlasCell;

                atlasCell.x =
                    fmod(glyphIndex, 16.0);

                atlasCell.y =
                    floor(glyphIndex / 16.0);

                //-------------------------------------
                // Glyph Sampling
                //-------------------------------------

                float2 glyphUV =
                    frac(uv * _GridSize);

                glyphUV =
                    (glyphUV + atlasCell)
                    / 16.0;

                float glyph =
                    SAMPLE_TEXTURE2D(
                        _MainTex,
                        sampler_MainTex,
                        glyphUV).r;

                //-------------------------------------
                // Brightness
                //-------------------------------------

                float glow =
                    pow(trail, 2.0)
                    * _Brightness;

                float3 color =
                    glyph *
                    trail *
                    _Tint.rgb *
                    glow;

                //-------------------------------------
                // Bright Stream Head
                //-------------------------------------

                if (distanceFromHead < 1.0)
                {
                    color +=
                        glyph *
                        _Tint.rgb *
                        4.0;
                }

                return float4(
                    color,
                    trail);
            }

            ENDHLSL
        }
    }

    FallBack Off
}