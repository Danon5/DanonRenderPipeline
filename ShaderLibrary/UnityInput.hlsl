#ifndef DRP_UNITY_INPUT_INCLUDED
#define DRP_UNITY_INPUT_INCLUDED

float4x4 unity_ObjectToWorld;
float4x4 unity_WorldToObject;
float4x4 unity_PrevObjectToWorld;
float4x4 unity_PrevWorldToObject;
float4x4 unity_ViewToWorld;
real4 unity_WorldTransformParams;

float4x4 unity_MatrixVP;
float4x4 unity_MatrixV;
float4x4 glstate_matrix_projection;

#endif