// Toony Colors Pro+Mobile 2
// (c) 2014,2015 Jean Moreno


Shader "Toony Colors Pro 2/User/TCP2_Alpha Test"
{
	Properties
	{
		//TOONY COLORS
		_Color ("Color", Color) = (0.5,0.5,0.5,1.0)
		_HColor ("Highlight Color", Color) = (0.6,0.6,0.6,1.0)
		_SColor ("Shadow Color", Color) = (0.3,0.3,0.3,1.0)
		
		//DIFFUSE
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
		
		//TOONY COLORS RAMP
		_Ramp ("#RAMPT# Toon Ramp (RGB)", 2D) = "gray" {}
		
		//Alpha Testing
		_Cutoff ("#CUTOUT# Alpha cutoff", Range(0,1)) = 0.5
	}
	
	SubShader
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		Cull Off
		
		CGPROGRAM
		
		#include "../../Shaders 2.0/Include/TCP2_Include.cginc"
		#pragma surface surf ToonyColors alphatest:_Cutoff addshadow
		#pragma target 3.0
		#pragma glsl
		
		#pragma multi_compile TCP2_RAMPTEXT
		
		//================================================================
		// VARIABLES
		
		fixed4 _Color;
		sampler2D _MainTex;
		
		
		struct Input
		{
			half2 uv_MainTex;
		};
		
		//================================================================
		// SURFACE FUNCTION
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
			
			o.Albedo = mainTex.rgb * _Color.rgb;
			o.Alpha = mainTex.a * _Color.a;
			
		}
		
		ENDCG
	}
	
	Fallback "Diffuse"
	CustomEditor "TCP2_MaterialInspector"
}
