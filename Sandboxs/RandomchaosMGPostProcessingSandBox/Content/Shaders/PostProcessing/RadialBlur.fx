#include "PPVertexShader.fxh"
float radialBlurScaleFactor = -0.004f;//0.96f;

uniform extern float2 windowSize;

//uniform extern texture sceneMap;
sampler sceneMapSampler : register(s0);
/* = sampler_state 
{
    texture = <sceneMap>;
    MinFilter = Anisotropic;
    MagFilter = Anisotropic;
    MaxAnisotropy = 8;
    MipFilter = Linear;
};*/

struct VB_OutputPos8TexCoords
{
	float4 pos : SV_POSITION;
	float4 Color : COLOR0;
	float2 texCoord[8] : TEXCOORD0;
};

float4 PS_RadialBlur20(VB_OutputPos8TexCoords In) : COLOR
{
	float2 texelSize = 1.0 / windowSize;
	
	In.texCoord[0] = In.texCoord[0] + texelSize*0.5f;
	
	float2 texCentered = (In.texCoord[0]-float2(0.5f, 0.5f))*2.0f;
	
	for (int i=1; i < 8; i++)
	{
		texCentered = texCentered + radialBlurScaleFactor*(0.5f+i*0.15f)*texCentered*abs(texCentered);
		In.texCoord[i] = (texCentered+float2(1.0f, 1.0f))/2.0f + texelSize*0.5;
	}

	float4 radialBlur = tex2D(sceneMapSampler, In.texCoord[0]);
	for (int i2=1; i2 < 8; i2++)
		radialBlur += tex2D(sceneMapSampler, In.texCoord[i2]);
		
	return radialBlur / 8;		
}

technique RadialBlur
{
	pass RadialBlur
	{
		PixelShader  = compile PS_SHADERMODEL PS_RadialBlur20();
	}
	
}
