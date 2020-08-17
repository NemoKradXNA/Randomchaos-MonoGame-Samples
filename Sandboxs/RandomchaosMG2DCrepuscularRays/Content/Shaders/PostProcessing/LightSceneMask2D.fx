#include "PPVertexShader.fxh"

float2 lightPosition;


sampler2D Scene: register(s0){
	AddressU = Mirror;
	AddressV = Mirror;
};

texture depthMap;
sampler2D DepthMap = sampler_state
{
	Texture = <depthMap>;
	MinFilter = Point;
	MagFilter = Point;
	MipFilter = None;
};

float4 LightSourceSceneMaskPS(VertexShaderOutput input) : COLOR0
{
	float4 scene = tex2D(Scene,input.TexCoord);

	float depthVal = 1-tex2D(DepthMap, input.TexCoord).a;
		

	return scene * depthVal;
}


technique LightSourceSceneMask
{
	pass p0
	{
		//VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL LightSourceSceneMaskPS();
	}
}