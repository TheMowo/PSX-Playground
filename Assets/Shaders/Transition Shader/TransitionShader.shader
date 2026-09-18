Shader "Custom/TransitionShader"
{
    Properties
    {
        _Cutoff("Cutoff", Range(0, 1)) = 0.0
        _TransitionTex("Transition Texture", 2D) = "white" {}
        _Color("Transition Color", Color) = (0, 0, 0, 1)
        _EdgeSoftness("Edge Softness", Range(0, 0.2)) = 0.01
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "TransitionPass"

            ZWrite Off
            ZTest Always
            Cull Off

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            // Transition texture
            TEXTURE2D(_TransitionTex);
            SAMPLER(sampler_TransitionTex);

            // Material properties
            CBUFFER_START(UnityPerMaterial)
                float _Cutoff;
                float4 _Color;
                float _EdgeSoftness;
            CBUFFER_END

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord.xy;

                // Current camera image
                half4 screen = SAMPLE_TEXTURE2D_X(
                    _BlitTexture,
                    sampler_LinearClamp,
                    uv
                );

                // Transition mask
                half4 transition = SAMPLE_TEXTURE2D(
                    _TransitionTex,
                    sampler_TransitionTex,
                    uv
                );

                // Invert mask so white -> transition color
                float maskValue = 1.0 - transition.r;

                // Prevent smoothstep from collapsing
                float softness = max(_EdgeSoftness, 0.0001);

                // Smooth cutoff
                float sceneMask = smoothstep(
                    _Cutoff - softness,
                    _Cutoff + softness,
                    maskValue
                );

                // Blend between transition color and screen
                return lerp(_Color, screen, sceneMask);
            }

            ENDHLSL
        }
    }
}
