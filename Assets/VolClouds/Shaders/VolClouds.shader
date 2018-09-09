// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "LemonSpawn/VolumetricClouds" {

	Properties{
		_H("Height (from, to)", Vector) = (500, 1200,0,0)
		_CloudScale("Scale", Range(0, 1)) = 0.3
		_CloudColor("Color", Color) = (1, 1, 1, 1.0)
		_CloudDistance("Distance", Range(0,0.25)) = 0.03
		_MaxDetail("Max Detail (performance speed)", Range(0,1)) = 0.33
		_CloudSubtract("Subtract", Range(0,1)) = 0.6
		_CloudScattering("Scattering",Range(1,3)) = 1.5
		_CloudHeightScatter("Height Scattering",Range(0.5,6)) = 1.5
		_CloudAlpha("Density", Range(0,1)) = 0.6
		_CloudHardness("Hardness", Range(0,1)) = 0.8
		_CloudBrightness("Brightness", Range(0,2)) = 1.4
		_SunGlare("Sun glare",Range(0, 2)) = 0.4
		_XShift("XShift", float) = 0
		_YShift("YShift", float) = 0
		_ZShift("ZShift", float) = 0
		_CloudTime("Time", float) = 0
	}

		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 400


		Lighting Off
		Cull off
		ZWrite off
		ZTest on
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
	{

		Tags{ "LightMode" = "ForwardBase" }

		CGPROGRAM

		#pragma target 4.0
		#pragma fragmentoption ARB_precision_hint_fastest


		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_fwdbase


		#include "UnityCG.cginc"
		#include "AutoLight.cginc"


		struct vertexInput {
			float4 vertex : POSITION;
			float4 texcoord : TEXCOORD0;
			float3 normal : NORMAL;
			float4 tangent : TANGENT;
		};

		struct v2f
		{
			float4 pos : SV_POSITION;
			float2 texcoord : TEXCOORD0;
			float3 normal : TEXCOORD1;
			float3 worldPosition : TEXCOORD2;
		};

		float4 _H;
		float4 _CloudColor;
		float _CloudScale;
		float _CloudDistance;
		float _MaxDetail;
		float _CloudSubtract;
		float _CloudScattering;
		float _CloudAlpha;
		float _CloudHardness;
		float _CloudBrightness;
		float _SunGlare;
		float _CloudHeightScatter;
		float _XShift;
		float _YShift;
		float _ZShift;
		float _CloudTime;

		v2f vert(vertexInput v)
		{
			v2f o;

			float4x4 modelMatrix = unity_ObjectToWorld;
			float4x4 modelMatrixInverse = unity_WorldToObject;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.normal = v.normal;
			o.worldPosition = mul(modelMatrix, v.vertex);
			return o;
		}

		inline float hh(in float n) { return frac(sin(n)*653.52351823);}
		inline float noise(in float3 x) { float3 p = floor(x);float3 f = frac(x); f = f*f*(3.0 - 2.0*f); float n = p.x + p.y*157.0 + 113.0*p.z; return lerp(lerp(lerp(hh(n + 0.0), hh(n + 1.0), f.x), lerp(hh(n + 157.0), hh(n + 158.0), f.x), f.y), lerp(lerp(hh(n + 113.0), hh(n + 114.0), f.x),lerp(hh(n + 270.0), hh(n + 271.0), f.x), f.y), f.z); }


		inline float getClouds(float3 p) {
			float ss = _CloudSubtract;
			float A = 0;
			float n = 0;
			p += float3(_XShift, _YShift, _ZShift);
			p.y*=_CloudHeightScatter;
			[unroll]
			for (int i = 1; i < 8; i++) {
				float f = pow(2, i);
				float amp = 1.0 / (2 * pow(i,_CloudScattering));
				float3 t = float3(0.3234, 0.0923, 0.25234)*_CloudTime*cos(i*3.1234 )*0.003;
				n += noise((p+t)*f*10) *amp;
				A += amp;
			}
			return clamp(n - ss*A,0, 1);
		}

		float4 rayCast(float3 start, float3 end, float3 direction,  float stepLength, float3 lDir, float2 hSpan, float3 skyColor, float3 camera, float light) {

			float3 pos = start;
			float3 dir = normalize(direction);
			float intensity = 0;
			int N = stepLength;
			float sl = length(end - start)/N;
			float scale = 0.0001*_CloudScale;
			float4 sum = float4(0,0,0,0);
			for (int i=0;i<N;i++)
				{
					if (pos.y >= hSpan.x*0.99 && pos.y <= hSpan.y && sum.a<0.99) {
						intensity = (getClouds(pos*scale))*_CloudAlpha*6;
					}
					else break;

					if (intensity > 0.01) { // Integrate
						float dif = clamp(intensity - getClouds((pos + 130*lDir)*scale)*2,0,1);
						float3 l = float3(0.65,0.7,0.75)*_CloudBrightness + float3(1.0, 0.6, 0.3)*dif; 
	    				float4 c = float4( lerp( float3(1.0,0.94,0.81), float3(0.251,0.31,0.352), intensity ), intensity );
	    				c.xyz *= l;
	    				c.a *= _CloudHardness;
	    				c.rgb *= c.a;
	    				sum = sum + c*(1.0-sum.a);

					}
					pos = pos + dir*sl;
			}
			float4 col = float4(1,1,1,1);
			col.xyz=col.xyz*(1.0-sum.w) + sum.xyz;
			float sun = clamp( dot(lDir,dir), 0.0, 1.0 );
			col += _SunGlare*float4(1.0,0.45,0.22,1)*pow( sun, 3.0 );
			col.a = sum.a;
			col.xyz*=light*skyColor;
			return col;
			}

		bool intersectPlane(in float3 n, in float3 p0, in float3 l0, in float3 l, out float t) 
		{ 
	    	float denom = dot(n, l);
	    	if (denom > 0) { 
	        	t = dot(p0 - l0, n) / denom;
	        	return (t >= 0); 
	  		} 
	    	return false; 
		} 

		fixed4 frag(v2f IN) : COLOR{

			float3 lightDirection = normalize(_WorldSpaceLightPos0);

			float3 viewDirection = normalize(
				_WorldSpaceCameraPos - IN.worldPosition.xyz)*-1;

			float2 h = _H.xy;

			float3 v3CameraPos = _WorldSpaceCameraPos;

			float t0, t1;

			float3 plane = float3(0,1,0);
			float3 plane1Pos = float3(0,1*h.x,0);
			float3 plane2Pos = float3(0,1*h.y,0);
			if (v3CameraPos.y<h.x) { // Below camera height
				if (intersectPlane(plane, plane1Pos, v3CameraPos, viewDirection,t0)) {
					if (intersectPlane(plane, plane2Pos, v3CameraPos, viewDirection,t1)) {

					}
					else discard;
				}
				else discard;
			}

			float3 startPos = _WorldSpaceCameraPos + t0*viewDirection;
			float3 endPos = _WorldSpaceCameraPos + t1*viewDirection;
			float light = pow(clamp(dot(lightDirection, normalize(plane))+0.15, 0, 1),1);

			float detail = clamp(2*100000.0/length(t0*viewDirection), 50, _MaxDetail*200);
			float dist = length(t0*viewDirection);
			float sub = clamp(_CloudDistance*dist*0.002 - 0.2, 0,1);
			float4 c;
			if (sub<0.99) 
				c = rayCast(startPos, endPos, viewDirection, detail, lightDirection, h, _CloudColor.xyz, _WorldSpaceCameraPos, light);
			c.a-=sub;

			return c;

		}
	ENDCG
	}
	}
Fallback "Diffuse"
}