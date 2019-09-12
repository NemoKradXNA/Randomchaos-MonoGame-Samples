#include "PPVertexShader.fxh"


sampler screen : register(s0)
{
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

float4 GreyScale(VertexShaderOutput input) : COLOR0
{
	float4 col = tex2D(screen, input.TexCoord);

	float value = (col.r + col.g + col.b) / 3;
	col = float4(value, value, value, col.a);

	return col;
}

technique PostInvert
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL GreyScale();
	}
}