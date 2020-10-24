#include "DeferredHeader.fxh"

// Global Variables
float4x4 world : WORLD;
float4x4 wvp : WorldViewProjection;

float4 color = 1;
float2 UVMultiplier = float2(1,1);

texture textureMat;
sampler textureSample = sampler_state
{
	texture = <textureMat>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

texture occlusionMat;
sampler occlusionSample = sampler_state
{
	texture = <occlusionMat>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

texture BumpMap;
sampler BumpMapSampler = sampler_state
{
	Texture = <BumpMap>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

texture specularMap;
sampler SpecularSampler = sampler_state
{
	Texture = <specularMap>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

texture glowMap;
sampler GlowSampler = sampler_state
{
	Texture = <glowMap>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

texture reflectionMap;
sampler ReflectionMap = sampler_state
{
	Texture = <reflectionMap>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

texture normalMap;
texture depthMap;
texture sceneMap;

sampler sceneSample = sampler_state
{
	texture = <sceneMap>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

sampler normalSampler = sampler_state
{
	Texture = (normalMap);
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = POINT;
	MinFilter = POINT;
	Mipfilter = POINT;
};
sampler depthSampler = sampler_state
{
	Texture = (depthMap);
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = POINT;
	MinFilter = POINT;
	Mipfilter = POINT;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TexCoord : TexCoord0;
	float3 Normal	: Normal0;
	float3 Tangent	: Tangent0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TexCoord : TexCoord0;
	float3 Normal : Normal0;
	float3x3 Tangent: Tangent0;
	float4 SPos : TexCoord1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(input.Position, wvp);
	output.TexCoord = input.TexCoord;

	output.Normal = mul(input.Normal, world);
	
	output.Tangent[0] = normalize(mul(input.Tangent, world));
	output.Tangent[1] = normalize(mul(cross(input.Tangent, input.Normal), world));
	output.Tangent[2] = normalize(output.Normal);

	output.SPos = output.Position;

	return output;
}

PixelShaderOutput PSBasicTexture(VertexShaderOutput input) : COLOR0
{
	PixelShaderOutput output = (PixelShaderOutput)0;

	float ta = tex2D(textureSample, input.TexCoord * UVMultiplier).a;

	output.Color = tex2D(textureSample,input.TexCoord * UVMultiplier) * float4(color.rgb,1);

	float occlusion = tex2D(occlusionSample, input.TexCoord * UVMultiplier).r;

	output.Color.rgb *= occlusion * output.Color.a;

	if (ta != 1)
	{
		float2 sp = input.SPos.xy / input.SPos.w;

		/*sp.x = sp.x / 2.0f + 0.5f;
		sp.y = (-sp.y / 2.0f + 0.5f);*/

		output.Color = tex2D(sceneSample, sp);
		output.Tangent = tex2D(normalSampler, sp);
		output.Depth = tex2D(depthSampler, sp);
	}
	else
	{
		// Get value in the range of -1 to 1
		float3 n = 2.0f * tex2D(BumpMapSampler, input.TexCoord * UVMultiplier) - 1.0f;	

		// Multiply by the tangent matrix
		n = mul(n, input.Tangent);

		// Generate normal values
		output.Tangent.rgb = (.5f  * (normalize(n) + 1.0f)) * output.Color.a;
		output.Tangent.a = 1;

		// Depth
		output.Depth.r = 1 - (input.SPos.z / input.SPos.w); // Flip to keep accuracy away from floating point issues.
		output.Depth.a = 1;

		//output.Color += float4(1, 0, 0, 1);
	}

	//output.Color += output.Color.a;

	// Write out SGR
	output.SGR.r = tex2D(SpecularSampler, input.TexCoord * UVMultiplier);
	output.SGR.g = tex2D(GlowSampler, input.TexCoord * UVMultiplier);
	output.SGR.b = tex2D(ReflectionMap, input.TexCoord * UVMultiplier);
	output.SGR.w = 0;

	

	return output;
}

BlendState AlphaBlendingOn
{
	BlendEnable[0] = TRUE;
	DestBlend = INV_SRC_ALPHA;
	SrcBlend = SRC_ALPHA;
};


technique Deferred
{
	pass Pass1
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PSBasicTexture();
	}
}
