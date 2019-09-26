#include "Shared.fxh"

float4x4 world : World;
float4x4 wvp : WorldViewProjection;
float4x4 vp : ViewProjection;

float maxHeight = 30;
float mod = 1024;
float2 sqrt = 1024;
float2 hp;
float OneOverWidth;

float3 EyePosition;
float3 scale;
float3 scale2;

float3 Color = float3(1,1,1);
float power = 1;

float3 lightDirection = float3(1,1,0);

texture heightMap;

sampler heightMapSampler = sampler_state
{
	Texture = <heightMap>;  
	MinFilter = POINT;
	MagFilter = POINT;
	MipFilter = POINT;
	AddressU  = mirror;
    AddressV  = mirror;
};

// Terrain Textures.
texture  LayerMap0;
texture  LayerMap1;
texture  LayerMap2;
texture  LayerMap3;

// Terrain Normals for above texture.
texture BumpMap0;
texture BumpMap1;
texture BumpMap2;
texture BumpMap3;

// Normal samplers
sampler BumpMap0Sampler = sampler_state
{
	Texture = <BumpMap0>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
	AddressU = mirror;
	AddressV = mirror;
};

sampler BumpMap1Sampler = sampler_state
{
	Texture = <BumpMap1>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
	AddressU = mirror;
	AddressV = mirror;
};

sampler BumpMap2Sampler = sampler_state
{
	Texture = <BumpMap2>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
	AddressU = mirror;
	AddressV = mirror;
};

sampler BumpMap3Sampler = sampler_state
{
	Texture = <BumpMap3>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
	AddressU = mirror;
	AddressV = mirror;
};

// Texture Samplers
sampler LayerMap0Sampler = sampler_state
{
    Texture   = <LayerMap0>;
    MinFilter = LINEAR; 
    MagFilter = LINEAR;
    MipFilter = LINEAR;
    AddressU  = WRAP;
    AddressV  = WRAP;
};

sampler LayerMap1Sampler = sampler_state
{
	Texture   = <LayerMap1>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
	AddressU  = WRAP;
    AddressV  = WRAP;
};

sampler LayerMap2Sampler = sampler_state
{
    Texture   = <LayerMap2>;
    MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
	AddressU  = WRAP;
    AddressV  = WRAP;
};
sampler LayerMap3Sampler = sampler_state
{
    Texture   = <LayerMap3>;
    MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
	AddressU  = WRAP;
    AddressV  = WRAP;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float4 Color : Color0;    
};

struct VertexShaderOutput
{
	float4 Position         : SV_POSITION;
    float2 TexCoord    : TEXCOORD0;      
    float4 weight : TEXCOORD1;
    float3x3 Tangent : TEXCOORD4;
    float4 SPos : TEXCOORD3; 	
    float4 p : TEXCOORD2;
	float4 col : COLOR0;
};

struct PixelShaderOutput
{
	float4 Color	: COLOR0;	// Color
	float4 SGR		: COLOR1;	// Specular, Glow and Reflection values
	float4 Tangent	: COLOR2;	// Normal / Tangent map
	float4 Depth	: COLOR3;	// Depth map (R) Specular (G) Glow (B) and Reflection (A)
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

	float3 scaleToUse = scale;

	float4 pos = mul(input.Position, world);
	pos.z = input.Color.r == 1 && input.Color.g == 0 && input.Color.b == 0 && input.Color.a == 1 ? pos.z - .25f : pos.z;
	pos.x = input.Color.r == 1 && input.Color.g == 1 && input.Color.b == 0 && input.Color.a == 1 ? pos.x - .25f : pos.x;
	pos.x = input.Color.r == 1 && input.Color.g == 1 && input.Color.b == 1 && input.Color.a == 1 ? pos.x + .25f : pos.x;
	pos.z = input.Color.r == 1 && input.Color.g == 1 && input.Color.b == 1 && input.Color.a == 0 ? pos.z + .25f : pos.z;

	output.col = input.Color;

	float y = 0;
	
	// Generate Normals
	float3 lN = pos.xyz;
	float3 bN = pos.xyz;
		
	lN.x += 1 * scaleToUse.x;
	bN.z -= 1 * scaleToUse.z;

	float d = saturate(length((EyePosition - pos))/30);
	
	y = (tex2Dlod(heightMapSampler,float4((lN.xz/sqrt)/scaleToUse.xz,d,0)).r * maxHeight) * scaleToUse.xz;
	lN.y = y;
	
	y = (tex2Dlod(heightMapSampler,float4((bN.xz/sqrt)/scaleToUse.xz,d,0)).r * maxHeight) * scaleToUse.xz;
	bN.y = y;
	
	y = (tex2Dlod(heightMapSampler,float4((pos.xz/sqrt)/scaleToUse.xz,d,0)) * maxHeight) * scaleToUse.xz;

	pos.y = y;

	
	float ys = y  / scaleToUse.xz;
	// Texture Weights
	output.weight.x = saturate(1 - (abs(ys) / 8));
	output.weight.y = saturate(1 - (abs(ys - 10) / 10));
	output.weight.z = saturate(1 - (abs(ys - 23) / 10));
	output.weight.w = saturate(1 - (abs(ys - 30) / 8));	
	
	float totW = output.weight.x + output.weight.y + output.weight.z + output.weight.w;	
	output.weight.x /= totW;
	output.weight.y /= totW;
	output.weight.z /= totW;
	output.weight.w /= totW;

	float3 side1 = pos.xyz - lN;
	float3 side2 = pos.xyz - bN;
	float3 normal = cross(side1,side2);
	
	// Generate Tangents	
	float3 tL = pos.xyz;
	
	tL.x -= 2 * scaleToUse.x;
	y = (tex2Dlod(heightMapSampler,float4((tL.xz/sqrt)/scaleToUse.xz,d,d)).r * maxHeight) * scaleToUse.xz;
	tL.y = y;
	
	float3 Tangent = normalize(tL-lN)*scaleToUse;
	
	output.Tangent[0] = mul(Tangent,world);
	output.Tangent[1] = mul(cross(Tangent,normal),world);
	output.Tangent[2] = mul(normal,world);
		
	output.Position = mul(pos, vp);
    
    output.TexCoord = ((pos.xz/mod) * 128) / scaleToUse.xz;
	
	output.SPos = output.Position;
	output.p = pos;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    PixelShaderOutput output = (PixelShaderOutput)0;
	
	float3 Normal;
	float4x4 norm;
	
	norm[0] = tex2D(BumpMap0Sampler, input.TexCoord);
    norm[1] = tex2D(BumpMap1Sampler, input.TexCoord);
    norm[2] = tex2D(BumpMap2Sampler, input.TexCoord);
    norm[3] = tex2D(BumpMap3Sampler, input.TexCoord);

	Normal = mul(input.weight,norm);
    Normal = 2.0f * Normal - 1.0f;
	
	float4 Col = 0;		
	float4x4 col;	
	
	col[0] = tex2D(LayerMap0Sampler, input.TexCoord);
    col[1] = tex2D(LayerMap1Sampler, input.TexCoord);
    col[2] = tex2D(LayerMap2Sampler, input.TexCoord);
    col[3] = tex2D(LayerMap3Sampler, input.TexCoord);

    Col += mul(input.weight,col);
	//Col = input.col;

	output.Color = Col;
	
	Normal = mul(Normal,input.Tangent);	
	output.Tangent.rgb = .5f  * (normalize(Normal) + 1.0f);
	output.Tangent.a = 1;
		
	output.SGR = 0;
	
	output.Depth.r = 1-(input.SPos.z/input.SPos.w);
	output.Depth.a = 1;
	
	//compute diffuse light
	float3 lightVector = normalize(lightDirection);
	float3 normal = normalize(Normal);

    float NdL = saturate(dot(normal,lightVector));
    float3 diffuseLight = (NdL * Color) * power;
	
	//return output.SGR;
	//return output.Tangent;
	//return output.Depth.r;
	return output.Color * float4(diffuseLight,1);
}

technique Technique1
{
    pass Pass1
    {
		FILLMODE = SOLID;
		CULLMODE = CCW;
        VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
    }
}
