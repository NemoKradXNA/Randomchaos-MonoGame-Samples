#include "DeferredHeader.fxh"
#include "DeferredSoftShadows.fxh"

float4x4 ViewProjectionInv : InverseViewProjection;
float4x4 LightViewProjection : LightViewProjection;

float3 LightPosition : POSITION;

//direction of the light
float3 lightDirection;
float ConeAngle;
float ConeDecay;

float power = 1;

float3 CameraPosition;

float mod = .0001f;

//color of the light 
float3 Color;

// normals, and specularPower in the alpha channel
texture normalMap;
texture depthMap;
texture shadowMap;

sampler depthSampler = sampler_state
{
    Texture = (depthMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
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
sampler shadowSampler = sampler_state
{
    Texture = (shadowMap);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};

texture sgrMap;
sampler sgrSampler = sampler_state
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
    output.texCoord = input.texCoord - halfPixel;
    return output;
}

float4 DirectionalLightPS(VertexShaderOutputToPS input) : COLOR0
{
	//input.texCoord -= halfPixel;
    float3 normalData = tex2D(normalSampler, input.texCoord);
    float3 normal = 2.0f * normalData.xyz - 1.0f;

    float depthVal = 1 - (tex2D(depthSampler, input.texCoord).r);

    float4 position;
    position.x = input.texCoord.x*2.0f-1.0f;
	position.y = -(input.texCoord.y*2.0f-1.0f);
	position.z = depthVal;
	position.w = 1.0f;
	
	float4 worldPos = mul(position, ViewProjectionInv);
    worldPos /= worldPos.w;
	
	//find screen position as seen by the light
	float4 lightScreenPos = mul(worldPos, LightViewProjection);
    lightScreenPos /= lightScreenPos.w;
	
	//find sample position in shadow map
	float2 lightSamplePos;
    lightSamplePos.x = lightScreenPos.x/2.0f+0.5f;
	lightSamplePos.y = (-lightScreenPos.y/2.0f+0.5f);

	//determine shadowing criteria
	float realDistanceToLight = lightScreenPos.z;
	float distanceStoredInDepthMap = shadowSample(shadowSampler, lightSamplePos);
//    float mod = .0001f;
	//mod = .00005f;
    
	bool shadowCondition = distanceStoredInDepthMap < realDistanceToLight - mod;

    //determine cone criteria
    float3 ld = normalize(worldPos - LightPosition);
    float coneDot = dot(ld, normalize(lightDirection));
	bool coneCondition = coneDot > ConeAngle;

    //calculate shading
    float shading = 0;	

	if(!CastShadow)
		shadowCondition = false;
		
	if (coneCondition)// && !shadowCondition)
	{
		float coneAttenuation = pow(coneDot, ConeDecay);
		shading = saturate(dot(normal, -ld));		
		shading *= power;
		shading *= coneAttenuation;

		if (shadowCondition)
		{
			if (hardShadows)
				shading = 0;
			else
				shading = SoftShadow(distanceStoredInDepthMap, depthVal, lightSamplePos, shading, realDistanceToLight, shadowSampler, worldPos*1000.0);
		}
	}	

	



	float4 sgr = tex2D(sgrSampler, input.texCoord);

	float3 directionToCamera = CameraPosition - worldPos;

	float3 Half = normalize(-lightDirection + normalize(directionToCamera));
	float specular = pow(saturate(dot(normalize(normal), Half)), 25);
	float specCol = 4 * sgr.r * specular * coneDot * power;
	
	return float4(Color + specCol,1) * shading; 
}

technique DirectionalLight
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
        PixelShader = compile PS_SHADERMODEL DirectionalLightPS();
    }
}