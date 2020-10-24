#include "PPVertexShader.fxh"

#define NUM_SAMPLES 128

float3 lightPosition;


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
	
	float2 TexCoord = input.TexCoord;

	float2 DeltaTexCoord = (TexCoord - lightPosition.xy);
	DeltaTexCoord *= (1.0f / NUM_SAMPLES * Density);

	DeltaTexCoord = DeltaTexCoord ;

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
		//VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL lightRayPS();
	}
}
