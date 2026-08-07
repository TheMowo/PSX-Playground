Shader "Hidden/CRT_URP"
{
    Properties
    {
        _MainTex("Source", 2D) = "white" {}

        _HardScan("HardScan", Range(-16, -8)) = -8
        _HardPix("HardPix", Range(-4, -2)) = -3
        _MaskDark("MaskDark", Range(0, 2)) = 0.5
        _MaskLight("MaskLight", Range(0, 2)) = 1.5
        _Warp("Warp", Vector) = (0.03125, 0.04166, 0, 0)
        _ResScale("ResolutionScale", Range(1, 16)) = 4
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Name "CRTPostProcess"
            ZTest Always Cull Off ZWrite Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);
                o.uv = v.uv;
                return o;
            }

            // URP texture macros
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float _HardScan;
            float _HardPix;
            float _MaskDark;
            float _MaskLight;
            float4 _Warp;
            float _ResScale;

            // Gaussian helpers
            float Gaus(float pos, float scale) { return exp2(scale * pos * pos); }

            float2 Dist(float2 pos, float2 res)
            {
                pos = pos * res;
                return -((pos - floor(pos)) - float2(0.5, 0.5));
            }

            float3 Fetch(float2 pos, float2 off, float2 res)
            {
                pos = floor(pos * res + off) / res;
                return SAMPLE_TEXTURE2D_LOD(_MainTex, sampler_MainTex, pos, 0).rgb;
            }

            float3 Horz3(float2 pos, float off, float2 res)
            {
                float3 b = Fetch(pos, float2(-1.0, off), res);
                float3 c = Fetch(pos, float2(0.0, off), res);
                float3 d = Fetch(pos, float2(1.0, off), res);
                float dst = Dist(pos, res).x;
                float wb = Gaus(dst - 1.0, _HardPix);
                float wc = Gaus(dst + 0.0, _HardPix);
                float wd = Gaus(dst + 1.0, _HardPix);
                return (b*wb + c*wc + d*wd) / (wb + wc + wd);
            }

            float3 Horz5(float2 pos, float off, float2 res)
            {
                float3 a = Fetch(pos, float2(-2.0, off), res);
                float3 b = Fetch(pos, float2(-1.0, off), res);
                float3 c = Fetch(pos, float2(0.0, off), res);
                float3 d = Fetch(pos, float2(1.0, off), res);
                float3 e = Fetch(pos, float2(2.0, off), res);
                float dst = Dist(pos, res).x;
                float wa = Gaus(dst - 2.0, _HardPix);
                float wb = Gaus(dst - 1.0, _HardPix);
                float wc = Gaus(dst + 0.0, _HardPix);
                float wd = Gaus(dst + 1.0, _HardPix);
                float we = Gaus(dst + 2.0, _HardPix);
                return (a*wa + b*wb + c*wc + d*wd + e*we) / (wa + wb + wc + wd + we);
            }

            float Scan(float2 pos, float off, float2 res)
            {
                float dst = Dist(pos, res).y;
                return Gaus(dst + off, _HardScan);
            }

            float3 Tri(float2 pos, float2 res)
            {
                float3 a = Horz3(pos, -1.0, res);
                float3 b = Horz5(pos, 0.0, res);
                float3 c = Horz3(pos, 1.0, res);
                float wa = Scan(pos, -1.0, res);
                float wb = Scan(pos, 0.0, res);
                float wc = Scan(pos, 1.0, res);
                return a*wa + b*wb + c*wc;
            }

            float2 Warp(float2 pos)
            {
                pos = pos * 2.0 - 1.0;
                pos *= float2(1.0 + (pos.y*pos.y)*_Warp.x, 1.0 + (pos.x*pos.x)*_Warp.y);
                return pos * 0.5 + 0.5;
            }

            float3 Mask(float2 pos)
            {
                pos.x += pos.y * 3.0;
                float3 mask = float3(_MaskDark, _MaskDark, _MaskDark);
                pos.x = frac(pos.x / 6.0);
                if (pos.x < 0.333) mask.r = _MaskLight;
                else if (pos.x < 0.666) mask.g = _MaskLight;
                else mask.b = _MaskLight;
                return mask;
            }

            half4 Frag(Varyings i) : SV_Target
            {
                float2 res = _ScreenParams.xy / _ResScale;
                float2 pos = Warp(i.uv);
                float3 col = Tri(pos, res) * Mask(i.uv * _ScreenParams);
                return half4(col, 1.0);
            }
            ENDHLSL
        }
    }
}
