Shader "DRP/Unlit"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BaseCol("Color", Color) = (1, 1, 1, 1)
        [Toggle(_CLIPPING)] _Clipping ("Alpha Clipping", Float) = 0
        _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend", Float) = 0
        [Enum(Off, 0, On, 1)] _ZWrite ("Z Write", Float) = 1
    }
    SubShader
    {
        Pass
        {
            Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]
            
            HLSLPROGRAM
            #pragma multi_compile_instancing
            #pragma shader_feature _CLIPPING
            #pragma vertex Vert
            #pragma fragment Frag
            #include "UnlitPass.hlsl"
            ENDHLSL
        }
    }
}
