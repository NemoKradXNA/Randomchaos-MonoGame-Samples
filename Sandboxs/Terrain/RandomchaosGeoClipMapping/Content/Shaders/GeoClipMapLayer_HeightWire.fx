#include "Shared.fxh"

float4x4 world : World;
float4x4 wvp : WorldViewProjection;
float4x4 vp : ViewProjection;

float2 sqrt = 1024;
float3 camPos;
float2 halfPixel;
float2 hp;
float OneOverWidth;

float3 scale;
float3 EyePosition;
float maxHeight = 60;

texture heightMap;
sampler heightMapSampler = sampler_state
{
	Texture = <heightMap>;  
	MinFilter = POINT;
	MagFilter = POINT;
	MipFilter = POINT;
	AddressU  = MIRROR;
    AddressV  = MIRROR;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
	float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

	float4 pos = mul(input.Position,world);
	
	/*
	float y = tex2Dlod(heightMapSampler,float4(pos.xz,0,.0));
	float f = floor(y);
	float r = frac(y) * 512-256;

	float2 a = clamp((abs(pos.xz-camPos.xz)) * halfPixel,0,1);
	a.x = max(a.x,a.y);

	float z = f + a.x * r;

    pos.y = z;
	*/

	      //y = (tex2Dlod(heightMapSampler, float4((pos.xz / sqrt) / scaleToUse.xz, d, 0)) * maxHeight) * scaleToUse.xz;
    float d = saturate(length((EyePosition - pos)) / 30);
	float y =  tex2Dlod(heightMapSampler, float4((pos.xz / sqrt) / scale.xz,d,0)) * (maxHeight * scale.xz);
	pos.y = y;
	

    output.Position = mul(pos, vp);
	output.Color = input.Color;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return input.Color;
}

technique Technique1
{
    pass Pass1
    {
		FILLMODE = WIREFRAME;
		CULLMODE = NONE;
        VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}
