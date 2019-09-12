#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

//uniform extern texture g_texBlit;

sampler g_sampBlit : register(s0);
/* = sampler_state
{
	texture = (g_texBlit);
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
	AddressU = Clamp;
	AddressV = Clamp;
};*/

float g_BlurAmount = 1.0f; // Kernel size multiplier
static const int g_cKernelSize = 13;

float aTexelKernel[g_cKernelSize] = { -6,-5,-4,-3,-2,-1,0,1,2,3,4,5,6, };

static const float aBlurWeights[g_cKernelSize] = 
{
  0.002216,0.008764,0.026995,0.064759,0.120985,0.176033,
  0.199471,
  0.176033,0.120985,0.064759,0.026995,0.008764,0.002216
};

float4 BlurPSH(float2 vec2TC : TEXCOORD0) : COLOR
{
	float4 clrOrg = tex2D(g_sampBlit,vec2TC);
    float4 clrBlurred = 0;
    for (int i=0; i<g_cKernelSize; i++)
    {
		float2 vec2TexCoord = vec2TC;
		float fOffset = (aTexelKernel[i] / 256.0f) * g_BlurAmount;
		
		vec2TexCoord.x += fOffset;
		
        clrBlurred += tex2D(g_sampBlit,vec2TexCoord) * aBlurWeights[i];
	}
    return clrBlurred;
}

float4 BlurPSV(float2 vec2TC : TEXCOORD0) : COLOR
{
	float4 clrOrg = tex2D(g_sampBlit,vec2TC);
	float4 clrBlurred = 0;
	for (int i = 0; i < g_cKernelSize; i++)
	{
		float2 vec2TexCoord = vec2TC;
		float fOffset = (aTexelKernel[i] / 256.0f) * g_BlurAmount;
		vec2TexCoord.y += fOffset;
		clrBlurred += tex2D(g_sampBlit,vec2TexCoord) * aBlurWeights[i];
	}
	return clrBlurred;
}

technique BlurH
{
	pass pHorz
	{
		PixelShader = compile PS_SHADERMODEL BlurPSH();
	}	
}
technique BlurV
{
	pass pVert
	{
		PixelShader = compile PS_SHADERMODEL BlurPSV();
	}
}