#if !defined(FLOW_INCLUDED)
#define FLOW_INCLUDED

float3 FlowUVW(float2 _uv, float2 _flowVector, float2 _jump, float _flowOffset, float _tiling, float _time, bool _flowB)
{
	float phaseOffset = _flowB ? 0.5 : 0;
	
	float progress = frac(_time + phaseOffset);

	float3 uvw;	
	uvw.xy = _uv - _flowVector * (progress + _flowOffset);
	uvw.xy *= _tiling;
	uvw.xy += phaseOffset;
	uvw.xy += (_time - progress) * _jump;
	uvw.z = 1 - abs(1 - 2 * progress);

	return uvw;
}

float2 DirectionalFlowUV(float2 _uv, float3 _flowVectorAndSpeed, float _tiling, float _time, out float2x2 _rotation)
{
	float2 dir = normalize(_flowVectorAndSpeed.xy);

	_rotation = float2x2(dir.y, dir.x, -dir.x, dir.y);

	_uv = mul(float2x2(dir.y, -dir.x, dir.x, dir.y), _uv);	
	_uv.y -= _time * _flowVectorAndSpeed.z;

	return _uv * _tiling;
}

#endif