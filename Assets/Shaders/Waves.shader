Shader "Custom/Waves"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_WaveA("Wave A (dir, steepness, waveLength)", Vector) = (1, 0, 0.5, 10)
		_SpeedA("Speed Wave A", Float) = 1
		_WaveB("Wave B (dir, steepness, waveLength)", Vector) = (0, 1, 0.25, 20)
		_SpeedB("Speed Wave B", Float) = 1
		_WaveC("Wave C (dir, steepness, waveLength)", Vector) = (1, 1, 0.15, 10)
		_SpeedC("Speed Wave C", Float) = 1
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert addshadow
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
			half4 customColor;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float4 _WaveA;
		float4 _WaveB;
		float4 _WaveC;
		float _SpeedA;
		float _SpeedB;
		float _SpeedC;
		float3 _ObjectPosition;
		float _TimeSinceLevelLoad;

		float GerstnerWave(float4 _wave, float3 _p, float _speed, bool _isFirstWave, inout half4 _color, inout float3 _tangent, inout float3 _binormal)
		{
			float steepness = _wave.z;
			float waveLength = _wave.w;

			float k = 2 * UNITY_PI / waveLength;
			float c = sqrt(9.8 / k) * _speed;
			float2 d = normalize(_wave.xy);
			float f = k * (dot(d, _p.xz) - c * _TimeSinceLevelLoad);
			float a = steepness / k;

			_tangent += float3(-d.x * d.x * (steepness * sin(f)), d.x * (steepness * cos(f)), -d.x * d.y * (steepness * sin(f)));

			_binormal += float3(-d.x * d.y * (steepness * sin(f)), d.y * (steepness * cos(f)), -d.y * d.y * (steepness * sin(f)));

			float y = a * sin(f);

			return y;
		}

		void vert(inout appdata_full _vertexData, out Input _data)
		{
			UNITY_INITIALIZE_OUTPUT(Input, _data);

			_data.customColor = _Color;
			
			float3 gridPoint = mul(unity_ObjectToWorld, _vertexData.vertex).xyz;

			float3 tangent = float3(1, 0, 0);
			float3 binormal = float3(0, 0, 1);

			float p = gridPoint.y;
			p += GerstnerWave(_WaveA, gridPoint, _SpeedA, true, _data.customColor, tangent, binormal);
			p += GerstnerWave(_WaveB, gridPoint, _SpeedB, false, _data.customColor, tangent, binormal);
			p += GerstnerWave(_WaveC, gridPoint, _SpeedC, true, _data.customColor, tangent, binormal);

			float3 normal = normalize(cross(binormal, tangent));

			_vertexData.vertex.y = p - _ObjectPosition.y;
			_vertexData.normal = normal;
		}

		void surf(Input IN, inout SurfaceOutputStandard _o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.customColor;
			_o.Albedo = c.rgb;
			_o.Metallic = _Metallic;
			_o.Smoothness = _Glossiness;
			_o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
