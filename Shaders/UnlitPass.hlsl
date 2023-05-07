#ifndef DRP_UNLIT_PASS_INCLUDED
#define DRP_UNLIT_PASS_INCLUDED

#include "Assets/Plugins/DanonRenderPipeline/ShaderLibrary/Common.hlsl"

struct Attributes {
    float3 positionOS : POSITION;
    float2 uv0 : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct VertToFrag {
    float4 positionCS : SV_POSITION;
    float2 uv0 : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

TEXTURE2D(_MainTex);
SAMPLER(sampler_MainTex);

UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
    UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
    UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
    UNITY_DEFINE_INSTANCED_PROP(float, _Cutoff)
UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

VertToFrag Vert(Attributes input) {
    VertToFrag output;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    float3 positionWS = TransformObjectToWorld(input.positionOS);
    output.positionCS = TransformWorldToHClip(positionWS);
    float4 unityST = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _MainTex_ST);
    output.uv0 = input.uv0 * unityST.xy + unityST.zw;
    return output;
}

float4 Frag(VertToFrag input) : SV_TARGET {
    UNITY_SETUP_INSTANCE_ID(input);
    float4 texCol = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv0);
    float4 baseCol = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseColor);
    float4 col = texCol * baseCol;
    #ifdef _CLIPPING
    clip(col.a - UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Cutoff));
    #endif
    return col;
}

#endif
