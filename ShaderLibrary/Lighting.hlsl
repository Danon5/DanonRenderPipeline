#ifndef DRP_LIGHTING_INCLUDED
#define DRP_LIGHTING_INCLUDED

#include "Surface.hlsl"
#include "Light.hlsl"

float3 IncomingLight(Surface surf, Light light) {
    return saturate(dot(surf.n, light.dir)) * light.col;
}

float3 CalcLighting(Surface surf) {
    return IncomingLight(surf, GetDirectionalLight()) * surf.col;
}

#endif