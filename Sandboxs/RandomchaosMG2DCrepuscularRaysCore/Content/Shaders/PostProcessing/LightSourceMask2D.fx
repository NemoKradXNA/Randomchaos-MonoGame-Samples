#include "PPVertexShader.fxh"

float2 lightPosition;


float2 halfPixel;

float SunSize = 1500;

sampler Scene: register(s0)
{
	AddressU = Clamp;
	AddressV = Clamp;
};

texture flare;
sampler Flare = sampler_state
{
    Texture = (flare);
    AddressU = Clamp;
    AddressV = Clamp;
};

float4 LightSourceMaskPS(VertexShaderOutput input) : COLOR0
{
	float4 v = tex2D(Scene, input.TexCoord);
	// Get the scene
	float4 col = 0;
	
	// Find the suns position in the world and map it to the screen space.
	float2 coord;
		
	float size = SunSize / 1;
					
	float2 center = lightPosition;

	coord = .5 - (input.TexCoord - center) / size * .5;
	col += (pow(tex2D(Flare,coord),2) * 1) * 2;						
	
	
	return col + v;
}

technique LightSourceMask
{
	pass p0
	{
		//VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL LightSourceMaskPS();
	}
}