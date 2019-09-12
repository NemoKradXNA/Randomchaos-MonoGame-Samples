#include "PPVertexShader.fxh"

#define NUM_SAMPLES 255

float3 lightPosition;


float4x4 matVP;

float2 halfPixel;

float Density = .5f;
float Decay = .95f;
float Weight = 1.0f;
float Exposure = .15f;

sampler2D Scene: register(s0){
	AddressU = Clamp;
	AddressV = Clamp;
};


float4 lightRayPS(VertexShaderOutput input) : COLOR0
{
	// Find light pixel position
	float4 ScreenPosition = mul(lightPosition, matVP);
	ScreenPosition.xyz /= ScreenPosition.w;
	ScreenPosition.x = ScreenPosition.x/2.0f+0.5f;
	ScreenPosition.y = (-ScreenPosition.y/2.0f+0.5f);

	float2 TexCoord = input.TexCoord - halfPixel;

	float2 DeltaTexCoord = (TexCoord - ScreenPosition.xy);
	DeltaTexCoord *= (1.0f / NUM_SAMPLES * Density);

	DeltaTexCoord = DeltaTexCoord * clamp(ScreenPosition.w * ScreenPosition.z,0,.5f);

	float3 col = tex2D(Scene,TexCoord);
	float IlluminationDecay = 1.0;
	float3 Sample;
	
	for( int i = 0; i < NUM_SAMPLES; ++i )
	{
		TexCoord -= DeltaTexCoord;
		Sample = tex2D(Scene, TexCoord);
		Sample *= IlluminationDecay * Weight;
		col += Sample;
		IlluminationDecay *= Decay;			
	}


	return float4(col * Exposure,1);
	
}

technique LightRayFX
{
	pass p0
	{
		//VertexShader = compile vs_4_0_level_9_1 VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL lightRayPS();
	}
}
