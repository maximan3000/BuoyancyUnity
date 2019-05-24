#ifndef WATER_TOOLBOX
#define WATER_TOOLBOX

#include "UnityCG.cginc"


inline half3 PerPixelNormal(sampler2D bumpMap, half4 coords, half3 vertexNormal, half bumpStrength) 
{
	half3 bump = (UnpackNormal(tex2D(bumpMap, coords.xy)) + UnpackNormal(tex2D(bumpMap, coords.zw))) * 0.5;
	half3 worldNormal = vertexNormal + bump.xxy * bumpStrength * half3(1,0,1);
	return normalize(worldNormal);
} 

inline half Fresnel(half3 viewVector, half3 worldNormal, half bias, half power)
{
	half facing =  clamp(1.0-max(dot(-viewVector, worldNormal), 0.0), 0.0,1.0);	
	half refl2Refr = saturate(bias+(1.0-bias) * pow(facing,power));	
	return refl2Refr;	
}

inline void ComputeScreenAndGrabPassPos (float4 pos, out float4 screenPos, out float4 grabPassPos) 
{
	#if UNITY_UV_STARTS_AT_TOP
		float scale = -1.0;
	#else
		float scale = 1.0f;
	#endif
	
	screenPos = ComputeScreenPos(pos); 
	grabPassPos.xy = ( float2( pos.x, pos.y*scale ) + pos.w ) * 0.5;
	grabPassPos.zw = pos.zw;
}

inline float waterHeight(float _WaveLengthInverse, float _Intensity, float _Periode, half2 p)
{
	p *= _WaveLengthInverse;
	float v = (cos(_Time.y * _Periode + p.x) + sin(_Time.y * _Periode + p.y)) * _Intensity * 0.25;
	return v;
}






inline float4 WaterHeightAt(float4 pos)
{
	pos.y = 5;
	return pos;
}


















#endif