#include "Noise.fx"

float4x4 world;
float4x4 view;
float4x4 projection;
float3 lightPoint;

Texture2D colorMap;

SamplerState filter : register(s0);

struct VSInput
{
    float3 Position : SV_POSITION;
	float3 Normal : NORMAL0;
	float2 UV : TEXCOORD0;
	float4 Color : COLOR0;
};

struct VSOutput
{
    float4 Position : SV_POSITION;
	float4 WorldPosition : COLOR0;
	float3 Normal : NORMAL0;
};

VSOutput ModelVS(VSInput input)
{
    VSOutput output;
	
	output.WorldPosition = mul(float4(input.Position, 1), world);
	float4 viewPosition = mul(output.WorldPosition, view);
    output.Position = mul(viewPosition, projection);
	output.Normal = mul(float4(input.Normal, 1), world).xyz;
	
    return output;
}

float4 ModelPS(VSOutput input) : SV_TARGET0
{
	float2 uv = float2(
		snoise(input.WorldPosition.xyz*2),
		snoise(input.WorldPosition.zxy*2)
	);
	
	float3 color = colorMap.Sample(filter, uv).rgb;
	
	float3 toLightVector = normalize(lightPoint - input.WorldPosition.xyz);
	float attenuation = max(0, dot(input.Normal, toLightVector));
	
	return float4(color * attenuation, 1);
}

technique Model
{
    pass Pass1
    {
        VertexShader = compile vs_5_0 ModelVS();
        PixelShader = compile ps_5_0 ModelPS();
    }
}
