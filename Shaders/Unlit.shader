Shader "DRP/Unlit"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BaseColor("Color", Color) = (1., 1., 1., 1.)
    }
    SubShader
    {
        Pass
        {
            HLSLPROGRAM
            #pragma multi_compile_instancing
            #pragma vertex Vert
            #pragma fragment Frag
            #include "UnlitPass.hlsl"
            ENDHLSL
        }
    }
}
