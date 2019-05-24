Shader "Nature/OceanAdvanced"
{
	Properties
	{
		_BaseColor ("Base color", COLOR)  = ( .54, .95, .99, 0.5)
		_WaterColor ("Water color", COLOR)  = ( .54, .95, .99, 0.5)
		_ReflectionColor ("Reflection color", COLOR)  = ( .54, .95, .99, 0.5)
		_SpecularColor ("Specular color", COLOR)  = ( .72, .72, .72, 1)
		[NoScaleOffset] _Foam ("Foam texture", 2D) = "white" {}
		[HideInInspector] world_light_dir("", VECTOR) = (0.0, 1.0, 0.8, 0.0)
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	
	uniform float4 _BaseColor;
	uniform float4 _WaterColor;
	uniform float4 _ReflectionColor;
	uniform float4 _SpecularColor;
	
	#define NB_WAVE 5
	float4 waves_p[NB_WAVE];
	float4 waves_d[NB_WAVE];
	
	#define NB_INTERACTIONS 64
	#define WAVE_DURATION 4.0
	#define WAVE_SPEED 3.0
	#define MAX_WAVE_AMP 0.5
	float4 interactions[NB_INTERACTIONS];

	uniform float4 world_light_dir;
	uniform float4 sun_color;

	uniform sampler2D _Foam;
	uniform sampler2D _RefractionTex;
	uniform sampler2D _CameraDepthTexture;
	uniform sampler2D _CameraDepthNormalsTexture;
	
	#define PI 3.14159234


	float evaluateWave(float4 wave_param, float4 wave_dir, float2 pos, float t)
	{
	  return wave_param.y * sin( dot(wave_dir.xy, pos) * wave_param.x + t * wave_param.z);
	}

	float get_water_height(float3 p)
	{
		float height = 0.0;
		for(int i = 0; i < NB_WAVE; i++)
			height += evaluateWave(waves_p[i], waves_d[i], p.xz, _Time.y);
		return height;
	}

	float3 get_water_normal(float3 a)
	{
		const float eps = 0.01;
		float3 b = a + float3(eps, 0.0, 0.0);
		float3 c = a + float3(0.0, 0.0, eps);

		a.y = get_water_height(a);
		b.y = get_water_height(b);
		c.y = get_water_height(c);
		
		float3 n = normalize(cross(c - a, b - a));
		return n;
	}

	float hash(float2 p )
	{
		float h = dot(p,float2(127.1,311.7));	
		return frac(sin(h)*43758.5453123);
	}
	
	float noise2(in float2 p )
	{
		float2 i = floor(p);
		float2 f = frac(p);	
		float2 u = f*f*(3.0-2.0*f);
		return -1.0+2.0 * lerp( lerp( hash( i + float2(0.0,0.0) ), 
						 hash( i + float2(1.0,0.0) ), u.x),
					lerp( hash( i + float2(0.0,1.0) ), 
						 hash( i + float2(1.0,1.0) ), u.x), u.y);
	}
	
	float sea_octave(float2 uv, float choppy)
	{
		uv += noise2(uv);        
		float2 wv = 1.0 - abs(sin(uv));
		float2 swv = abs(cos(uv));    
		wv = lerp(wv,swv,wv);
		return pow(1.0 - pow(wv.x * wv.y,0.65),choppy);
	}

	float map_detailed(float3 p)
	{
		float freq = 0.16;
		float amp = 0.6;
		float choppy = 4.0;
		float2 uv = p.xz;
		uv.x *= 0.75;
		
		float d, h = 0.0;
		for(int i = 0; i < 5; i++)
		{
			d = sea_octave((uv + _Time.yy) * freq, choppy);
			d += sea_octave((uv - _Time.yy) * freq, choppy);
			h += d * amp;
			uv = float2(uv.x * 1.6 + 1.2 * uv.y, uv.x * -1.2 + 1.6 * uv.y);
			freq *= 1.9;
			amp *= 0.22;
			choppy = lerp(choppy,1.0,0.2);
		}
		
		for (int j = 0; j < NB_INTERACTIONS; j++)
		{
			half dist = distance(p.xz, interactions[j].xy);
			half elapsed = (_Time.y - interactions[j].w);
			half computed_distance = elapsed * WAVE_SPEED;
			half power = 1.0 - saturate(pow(abs(computed_distance - dist), 2.0) * 0.3);
			power *= 1.0 - saturate(elapsed / WAVE_DURATION);
			dist += 2.0;
			p.y += power * interactions[j].z;
		}
		
		return p.y - h;
	}
	
	float map(float3 p)
	{
		float freq = 0.16;
		float amp = 0.6;
		float choppy = 4.0;
		float2 uv = p.xz;
		uv.x *= 0.75;
		
		float d, h = 0.0;
		for(int i = 0; i < 3; i++)
		{
			d = sea_octave((uv + _Time.yy) * freq, choppy);
			d += sea_octave((uv - _Time.yy) * freq, choppy);
			h += d * amp;
			uv = float2(uv.x * 1.6 + 1.2 * uv.y, uv.x * -1.2 + 1.6 * uv.y);
			freq *= 1.9;
			amp *= 0.22;
			choppy = lerp(choppy,1.0,0.2);
		}
		return p.y - h;
	}
	
	float3 get_detailed_normal(float3 a, float eps)
	{
		float3 b = a + float3(eps, 0.0, 0.0);
		float3 c = a + float3(0.0, 0.0, eps);
		
		a.y = map_detailed(a);
		b.y = map_detailed(b);
		c.y = map_detailed(c);
		
		float3 n = normalize(cross(b - a, c - a));
		n.y *= -1;
		return n;
	}

	float3 getSkyColor(float3 e)
	{
		e.y = max(e.y, 0.0);
		return float3(pow(1.0 - e.y,2.0), 1.0 - e.y, 0.6 + (1.0 - e.y) * 0.4);
	}
	
	float diffuse(float3 n,float3 l,float p)
	{
		return pow(dot(n,l) * 0.4 + 0.6,p);
	}
	
	float specular(float3 n,float3 l,float3 e,float s)
	{    
		float nrm = (s + 8.0) / (3.1415 * 8.0);
		return pow(max(dot(reflect(e,n),l),0.0),s) * nrm;
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

	struct appdata
	{
		float4 vertex : POSITION;
	};
	
	struct v2f
	{
		float4 pos : SV_POSITION;
		float3 normal : NORMAL;
		float3 world_position : TEXCOORD0;
		float4 ref : TEXCOORD1;
		float4 grabPassPos : TEXCOORD2;
		UNITY_FOG_COORDS(5)
	};
	
	v2f vert(appdata v)
	{
		v2f o;
		
		float4 world_position = mul(unity_ObjectToWorld, v.vertex);
		world_position.y = get_water_height(world_position.xyz);
		
		float interactive = 0.0;
		for (int i = 0; i < NB_INTERACTIONS; i++)
		{
			half dist = distance(world_position.xz, interactions[i].xy);
			half elapsed = (_Time.y - interactions[i].w);
			half computed_distance = elapsed * WAVE_SPEED;
			half power = 1.0 - saturate(pow(abs(computed_distance - dist), 2.0) * 0.3);
			power *= 1.0 - saturate(elapsed / WAVE_DURATION);
			dist += 2.0;
			interactive += power * interactions[i].z;
		}
		world_position.y += clamp(interactive, -MAX_WAVE_AMP, MAX_WAVE_AMP);
		
		o.world_position = world_position;
		o.normal = get_water_normal(world_position.xyz);
		o.pos = mul(UNITY_MATRIX_VP,  world_position);
		
		ComputeScreenAndGrabPassPos(o.pos, o.ref, o.grabPassPos);
		UNITY_TRANSFER_FOG(o,o.pos);
		
		return o;
	}

	#define REFLECTION
	#define FOAM
	#define SPECULAR
	#define REFRACTION
	
	half4 frag( v2f i ) : SV_Target
	{
		float3 eye_vector = i.world_position.xyz - _WorldSpaceCameraPos;
		float eye_distance = length(eye_vector);
		float3 eye = normalize(eye_vector);
		
		half3 detail_normal = get_detailed_normal(i.world_position, 0.01 * pow(length(eye_vector), 0.8));
		half3 vertex_normal = i.normal;
		half3 normal = normalize(detail_normal * 1.0 + vertex_normal * 0.0);
		
		float depth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.ref)).r);
		half4 distort = half4(i.normal.xz * 0.5 + detail_normal.xz * 0.6, 0.0, 0.0);
		distort *=  pow(saturate((depth - i.ref.z) * 0.3), 2.0);
		
		float sceneZ = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.ref + distort * 0.2)).r);
		float objectZ = i.ref.z;
		float depthFactor = 1.0 - saturate(abs(sceneZ - objectZ) / 4.0);
		
		float3 light_direction = normalize(world_light_dir);
		half4 baseColor;
		baseColor.a = 1.0;
		
		//baseColor.rgb = getSeaColor(i.world_position, normal, normalize(light_direction), normalize(eye_vector), eye_vector);
		
		#ifdef REFLECTION
			float3 l = normalize(light_direction);
			
			float fresnel = clamp(1.0 - pow(dot(normal,-eye), 1.0), 0.0, 1.0);
			fresnel = pow(fresnel,3.0) * 0.65;
			fresnel = pow(fresnel, 0.8) * 0.8;
			
			float3 reflected = getSkyColor(reflect(eye,normal)) * _ReflectionColor;
			
			/* SSR test */
			#ifdef SSR
				half4 refl = i.ref + distort;
				float3 decodedNormal;
				float decodedDepth;
				DecodeDepthNormal( tex2D( _CameraDepthNormalsTexture, i.uv), decodedDepth, normal);
				float4 pixelPosition = float4(i.cameraRay * decodedDepth, 0.0);
				//float3 reflected = tex2Dproj(_RefractionTex, UNITY_PROJ_COORD(pixelPosition)) * _ReflectionColor; 
			#endif

			
			float3 refracted = _BaseColor + diffuse(normal, l, 80.0) * _WaterColor * 0.12; 
			
			baseColor.rgb = lerp( refracted, reflected, fresnel);
		#endif
		
		#ifdef REFRACTION
			half3 refraction = tex2Dproj(_RefractionTex, UNITY_PROJ_COORD(i.grabPassPos + distort));
			refraction *= lerp(_ReflectionColor, _BaseColor, 1.0 - depthFactor * 2.0);
			baseColor.a = 1.0 - (saturate(depthFactor * 2.0) * saturate(1.2 - 0.6));
		#endif
		
		
		/* Possible depth color */
		float atten = max(1.0 - dot(eye, eye) * 0.001, 0.0);
		baseColor.rgb += _WaterColor * (i.world_position.y + 0.6) * 0.18 * atten;
			
		#ifdef SPECULAR
			float spec = specular(vertex_normal * 0.2 + detail_normal * 0.8, l, eye, 60.0);
			baseColor.rgb += sun_color * spec;
		#endif
			
		
		#ifdef FOAM
			half3 foamColor = tex2D(_Foam, i.world_position.xz * 0.1 + _Time.xx * 0.5 + distort).rgb;
			//baseColor.rgb += foamColor * smoothstep(0.2, 1.5, depthFactor);
			baseColor.a += foamColor.r * 0.05;
			
			float wave_foam = clamp(smoothstep(-0.1, 1.0, 1.0 - detail_normal.y), 0.0, 0.5);
			baseColor.a += wave_foam;
			baseColor = saturate(baseColor);
			baseColor.rgb += foamColor * clamp(wave_foam + smoothstep(0.2, 1.5, depthFactor), 0.0, 0.5);
		#endif
		
		UNITY_APPLY_FOG(i.fogCoord, baseColor);

		baseColor = saturate(baseColor);
		baseColor.rgb = lerp(refraction.rgb, baseColor.rgb, clamp(baseColor.a, 0.0, 0.8));
		baseColor.a = 1.0;
		return saturate(baseColor);
	}
	
	ENDCG
	Subshader
	{
		Tags {"RenderType"="Transparent" "Queue"="Transparent"}
		Lod 300
		//ColorMask RGBA
		
		GrabPass { "_RefractionTex" }
		
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			//ZTest LEqual
			//ZWrite Off
			//Cull Off
		
			CGPROGRAM
		
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_fog_exp2
			#pragma fragmentoption ARB_precision_hint_fastest
		
			ENDCG
		}
	}
	Fallback "Transparent/Diffuse"
}
