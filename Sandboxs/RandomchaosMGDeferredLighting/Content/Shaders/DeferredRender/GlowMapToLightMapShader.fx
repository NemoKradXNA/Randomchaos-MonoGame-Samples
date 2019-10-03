#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif


#include "DeferredHeader.fxh"


texture sgrMap;
sampler SGRSampler = sampler_state
{
	Texture = (sgrMap);
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	Mipfilter = LINEAR;
};

struct VertexShaderInput
{
	float3 Position : POSITION0;
	float2 texCoord : TEXCOORD0;
};

VertexShaderOutputToPS VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutputToPS output = (VertexShaderOutputToPS)0;
	output.Position = float4(input.Position, 1);
	output.texCoord = input.texCoord;// -halfPixel;
	return output;
}

float4 RenderScenePS(VertexShaderOutputToPS input) : COLOR0
{
	float gMap = tex2D(SGRSampler, input.texCoord).g;


	return gMap;
}
technique RenderScene
{
	pass Pass1
	{
		//ZEnable = true;
		//ZWriteEnable = true;
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL RenderScenePS();
	}
}