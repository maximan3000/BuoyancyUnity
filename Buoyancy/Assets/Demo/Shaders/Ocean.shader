// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Nature/Ocean"
{
	Properties
	{
		_FresnelScale ("FresnelScale", Range (0.15, 4.0)) = 0.75

		_BaseColor ("Base color", COLOR)  = ( .54, .95, .99, 0.5)
		_ReflectionColor ("Reflection color", COLOR)  = ( .54, .95, .99, 0.5)
		_SpecularColor ("Specular color", COLOR)  = ( .72, .72, .72, 1)
		
		_BumpMap ("Normals ", 2D) = "bump" {}
		_BumpTiling ("Bump Tiling", Vector) = (1.0 ,1.0, -2.0, 3.0)
		_BumpDirection ("Bump Direction & Speed", Vector) = (1.0 ,1.0, -1.0, 1.0)
		
		_WorldLightDir ("Specular light direction", Vector) = (0.0, 0.1, -0.5, 0.0)
		_Shininess ("Shininess", Range (2.0, 500.0)) = 200.0
		
		_DistortParams ("Distortions (Bump waves, Reflection, Fresnel power, Fresnel bias)", Vector) = (1.0 ,1.0, 2.0, 1.15)
		
		_WaveLengthInverse("Wave Length", Float) = 10.0
		_Intensity("Intensity", Float) = 4.0
		_Periode("Periode", Float) = 1.0
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	#include "WaterToolBox.cginc"
	
	struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};
	
	struct v2f
	{
		float4 pos : SV_POSITION;
		float3 viewInterpolator : TEXCOORD0;
		float4 bumpCoords : TEXCOORD1;
		float4 depth : TEXCOORD2;
		float4 grabPassPos : TEXCOORD3;
		float4 screenPos : TEXCOORD4;
		UNITY_FOG_COORDS(5)
	};
	
	float4 _BaseColor;
	float4 _ReflectionColor;
	float4 _SpecularColor;
	
	
	sampler2D _BumpMap;
	sampler2D _RefractionTex;
	
	uniform float4 _BumpDirection;
	uniform float4 _BumpTiling;
	uniform float4 _DistortParams;
	
	uniform float4 _WorldLightDir;
	uniform float _Shininess;
	uniform float _FresnelScale;
	
	uniform float _WaveLengthInverse;
	uniform	float _Intensity;
	uniform	float _Periode;
	
	uniform sampler2D _CameraDepthTexture;
	
	#define FRESNEL_POWER _DistortParams.z
	#define FRESNEL_BIAS _DistortParams.w
	#define PER_PIXEL_DISPLACE _DistortParams.x
	
	v2f vert(appdata_full v)
	{
		v2f o;
		half3 worldSpaceVertex = mul(unity_ObjectToWorld, v.vertex).xyz;
		half2 tileableUv = worldSpaceVertex.xz;

		o.bumpCoords.xyzw = (tileableUv.xyxy + _Time.xxxx * _BumpDirection.xyzw) * _BumpTiling.xyzw;
		o.viewInterpolator.xyz = worldSpaceVertex - _WorldSpaceCameraPos;
		
		v.vertex.y = waterHeight(_WaveLengthInverse, _Intensity, _Periode, worldSpaceVertex.xz) - worldSpaceVertex.y;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.depth = ComputeScreenPos(o.pos);
		ComputeScreenAndGrabPassPos(o.pos, o.screenPos, o.grabPassPos);
		UNITY_TRANSFER_FOG(o,o.pos);
		return o;

	}

	half4 frag( v2f i ) : SV_Target
	{
		half3 worldNormal = PerPixelNormal(_BumpMap, i.bumpCoords, half3(0,1,0), PER_PIXEL_DISPLACE);
		half3 viewVector = normalize(i.viewInterpolator.xyz);

		half3 reflectVector = normalize(reflect(viewVector, worldNormal));
		half3 h = normalize ((_WorldLightDir.xyz) + viewVector.xyz);
		float nh = max (0, dot (worldNormal, -h));
		float spec = max(0.0,pow (nh, _Shininess));
		
		half4 distortOffset = half4(worldNormal.xz * 0.24 * 10.0, 0, 0);
		half4 grabWithOffset = i.grabPassPos + distortOffset;
		half4 rtRefractionsNoDistort = tex2Dproj(_RefractionTex, UNITY_PROJ_COORD(i.grabPassPos));
		half refrFix = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(grabWithOffset));
		half4 rtRefractions = tex2Dproj(_RefractionTex, UNITY_PROJ_COORD(grabWithOffset));
		
		worldNormal.xz *= _FresnelScale;
		half refl2Refr = Fresnel(viewVector, worldNormal, FRESNEL_BIAS, FRESNEL_POWER);
		float depthT = LinearEyeDepth(tex2Dproj(_CameraDepthTexture,
                                                         UNITY_PROJ_COORD(i.depth)).r);
		float depth = depthT * 0.02 + 0.5 * refl2Refr;											 
		depth = depth > 500 ? 0.3 : depth;

		half4 baseColor = _BaseColor;
		half4 reflectionColor = _ReflectionColor;
		baseColor = lerp (lerp (rtRefractions, baseColor, baseColor.a), reflectionColor, refl2Refr);
		baseColor = baseColor + spec * _SpecularColor;
		baseColor.rgb += spec * _SpecularColor.rgb;

		UNITY_APPLY_FOG(i.fogCoord, baseColor);
		return baseColor;
	}
	
	ENDCG
	Subshader
	{
		Tags {"RenderType"="Transparent" "Queue"="Transparent"}
		
		Lod 200
		ColorMask RGB
		
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest LEqual
			//ZWrite Off
			Cull Off
		
			CGPROGRAM
		
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
		
			ENDCG
		}
	}

	Fallback "Transparent/Diffuse"
}
