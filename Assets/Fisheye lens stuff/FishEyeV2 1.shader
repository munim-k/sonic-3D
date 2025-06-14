Shader "Custom/FishEyeV1" {
	Properties{
		_center("Center", Vector) = (0,0,0)
		_strength("strength", float) = 1
		_radius("Radius", float) = 100
		_MainTex(" Diffuse (RGB)", 2D) = "white" {}
		_NormalMap ("NOrmal Map", 2D) = "bump" {}
		_Tint ("Tint", Color) = (1,1,1,1)
		_Glossiness("Glossiness", float) = 0
		_Metalic("Metallic", float) = 0
	}
	SubShader{

		//some shader settings
		Tags { "Camera" = "Opaque" }
		Cull Off
        CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert addshadow

		//this is where all the variables are declared
		sampler2D _MainTex;
		sampler2D _NormalMap;

		struct Input{
			float2 uv_MainTex;
			float2 uv_NormalMap;
		};

		half _Glossiness;
		half _Metalic;
		fixed4 _Tint;
		uniform float3 _camPos1;
		float3 _center;
		float _strength;
		float _radius;
		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(PRops)

		//this is a simple function to calculate projection vectors
		float3 projected(float3 a, float3 b){
			return dot(a,normalize(b)) * normalize(b);
		}
		//this is the function which will calculate each vertex position
		void vert(inout appdata_full v){			
			//camera pos in world space
			float3 A = _WorldSpaceCameraPos;
			//center pos in world space
			float3 B = _camPos1 + float3(0,_center.y,_radius);			
			//get the vertex position in world space
			float3 V = mul(unity_WorldToObject, v.vertex);
			//calculate the object position in world space
			float3 OBJECT_POS = v.vertex.xyz - V;
			//convert vertex, camera and center pos into object space using the object position
			V = v.vertex.xyz;
			A -= OBJECT_POS;
			B -= OBJECT_POS;
			//get the direction of the camera pos to the center pos
			float3 AB = B - A;
			//get the direction of the camear pos to the vertex pos
			float3 AV = V - A;
			//get the direction of the center pos to the vertex pos
			float3 BV = V - B;
			//get the projected vector of camera->vertex onto camera->center
			float3 AC = projected(AV, AB);
			//calcualte point C based on the projected vector
			float3 C = AC + A;
			//get the direction of point C towards the center
			float3 CB = B - C;
			float3 pos = B + normalize(BV) * (length(CB) * _strength + (1-_strength)*length(BV));	
			//finally assign the vertex to the new position
			v.vertex.xyz = pos;
		}
		//this function is what will determine the colour of each pixel
		void surf (Input In, inout SurfaceOutputStandard o){
			fixed4 c = tex2D (_MainTex, In.uv_MainTex) * _Tint;
			o.Albedo = c.rgb;
			o.Normal = UnpackNormal (tex2D (_NormalMap, In.uv_NormalMap));
			o.Metallic = _Metalic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	
	//if there is an error in the shader this is the shader that will be used instead
	FallBack "Diffuse"
}
