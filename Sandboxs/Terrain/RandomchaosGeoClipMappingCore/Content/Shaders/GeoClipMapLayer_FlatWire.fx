#include "Shared.fxh"

float4x4 world : World;
float4x4 wvp : WorldViewProjection;

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
	float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    
	output.Position = mul(input.Position, wvp);
	output.Color = input.Color;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return input.Color;
}

technique Technique1
{
    pass Pass1
    {
		FILLMODE = WIREFRAME;
		CULLMODE = NONE;
        VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}
