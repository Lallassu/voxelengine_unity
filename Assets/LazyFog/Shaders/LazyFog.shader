// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "LemonSpawn/LazyFog" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Scale ("Scale", Range(0,5)) = 1
		_Intensity ("Intensity", Range(0,1)) = 0.5
		_Alpha ("Alpha", Range(0,2.5)) = 0.75
		_AlphaSub ("AlphaSub", Range(0,1)) = 0.0
		_Pow ("Pow", Range(0,4)) = 1.0
	}
SubShader {
	    Tags {"Queue"="Transparent+101" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 400


		Lighting On
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
       Pass
         {

         
             CGPROGRAM
		    
             #pragma target 3.0
             #pragma fragmentoption ARB_precision_hint_fastest
             
             #pragma vertex vert
             #pragma fragment frag
             #pragma multi_compile_fwdbase
             
             #include "AutoLight.cginc"
             #include "UnityCG.cginc"
                         

             sampler2D _MainTex;
			 float4 _Color;		
			 float _Scale;
			 float _Intensity;
			 float _Alpha;
			 float _AlphaSub;
    		 float _Pow;
    		
             struct v2f
             {
                 float4 pos : POSITION;
                 float4 texcoord : TEXCOORD0;
                 float3 normal : TEXCOORD1;
                 float4 color: TEXCOORD2;
                 float2 uv : TEXCOORD3;
                 float3 worldPosition: TEXCOORD4;
 
             };
              
             v2f vert (appdata_full v)
             {
                 v2f o;
                 o.pos = UnityObjectToClipPos( v.vertex);
                 o.uv = v.texcoord;
                 o.normal = normalize(v.normal).xyz;
                 o.texcoord = v.texcoord;
 				 o.worldPosition = mul (unity_ObjectToWorld, v.vertex).xyz;
			     o.color =v.color;
        //         TRANSFER_VERTEX_TO_FRAGMENT(o);
                 
                 return o;
             }

		fixed4 frag(v2f IN) : COLOR {
			float3 worldSpacePosition = IN.worldPosition;
			float3 N;
			float3 distScale = 0.5;		
			float3 viewDirection = normalize(_WorldSpaceCameraPos - worldSpacePosition);
			float dist = clamp(pow(length(0.1*distScale*(_WorldSpaceCameraPos - worldSpacePosition)),1.0),0,1);
			float scale = 5;
			float shadowscale = 1;
			float4 c = tex2D(_MainTex, IN.uv*_Scale);
			float xx = c.r*_Intensity;
			xx = pow(xx,_Pow);
			c.a = c.r;			
			c.rgb = float3(xx*_Color.r,xx*_Color.g,xx*_Color.b);	
			c.a *= IN.color.a-2.5*length(IN.uv-float2(0.5, 0.5));
			c.a*=_Alpha;
			c.a-=_AlphaSub;
			return c;
			
             }
             ENDCG
         }
	}
 }