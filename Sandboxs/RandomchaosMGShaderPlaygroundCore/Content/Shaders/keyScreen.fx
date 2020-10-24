#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

float4 key = 0;

texture keyTexture;
sampler keySample = sampler_state
{
	Texture = (keyTexture);
	AddressU = CLAMP;
	AddressV = CLAMP;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TexCoord : TEXCOORD0;
};
struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TexCoord : TexCoord0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = float4(input.Position.xyz, 1);

	output.TexCoord = input.TexCoord;

	return output;
}



sampler screen : register(s0)
{
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};
bool floatInRange(float v, float m, float r)
{
	return v <= m + r && v >= m - r;
}
float4 Invert(VertexShaderOutput input) : COLOR0
{

	float4 col =  tex2D(screen, input.TexCoord);
	float4 keyPixel = tex2D(keySample, input.TexCoord);

	float4 fcol = keyPixel;

	if (floatInRange(key.r, keyPixel.r,.125) && floatInRange(key.g ,keyPixel.g,.125) && floatInRange(key.b , keyPixel.b,.125))
		fcol = col;

	return fcol;
}



technique PostInvert
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL Invert();
	}
}