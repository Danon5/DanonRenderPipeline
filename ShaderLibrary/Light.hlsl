#ifndef DRP_LIGHT_INCLUDED
#define DRP_LIGHT_INCLUDED

#include <HLSLSupport.cginc>

CBUFFER_START(_DRPLight)
    float3 _DirLightColor;
    float3 _DirLightDir;
CBUFFER_END

struct Light {
    float3 pos;
    float3 dir;
    float3 col;
};

Light GetDirectionalLight() {
    Light light;
    light.pos = 0;
    light.dir = _DirLightDir;
    light.col = _DirLightColor;
    return light;
}

#endif