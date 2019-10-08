#include "DeferredHeader.fxh"
#include "DeferredSoftShadows.fxh"

float4x4 viewProjectionInv;
float4x4 lightViewProjection;

float3 CameraPosition;

//direction of the light
float3 lightDirection;

float power = 1;

//color of the light 
float3 Color;

// normals, and specularPower in the alpha channel
texture normalMap;
texture sgrMap;
texture depthMap;
texture shadowMap;

float mod = .00005f;


sampler sgrSampler = sampler_state
{
	Texture = <sgrMap>;
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
sampler shadowSampler = sampler_state
{
	Texture = (shadowMap);
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
	output.texCoord = input.texCoord;// -halfPixel;
	return output;
}

float4 DirectionalLightPS(VertexShaderOutputToPS input) : COLOR0
{
	//input.texCoord -= halfPixel;	
	float4 normalData = tex2D(normalSampler,input.texCoord);	
	float3 normal = 2.0f * normalData.xyz - 1.0f;
	

	float depth = 1 - (tex2D(depthSampler, input.texCoord).r);

	//create screen position
	float4 screenPos;
	screenPos.x = input.texCoord.x*2.0f - 1.0f;
	screenPos.y = -(input.texCoord.y*2.0f - 1.0f);

	screenPos.z = depth;
	screenPos.w = 1.0f;

	float4 worldPos = mul(screenPos, viewProjectionInv);
	worldPos /= worldPos.w;

	//find screen position as seen by the light
	float4 lightScreenPos = mul(worldPos, lightViewProjection);
	lightScreenPos /= lightScreenPos.w;

	//find sample position in shadow map
	float2 lightSamplePos;
	lightSamplePos.x = lightScreenPos.x / 2.0f + 0.5f;
	lightSamplePos.y = (-lightScreenPos.y / 2.0f + 0.5f);

	//determine shadowing criteria
	float realDistanceToLight = lightScreenPos.z;

	realDistanceToLight -= mod;

	float2 texelSize = float2(1.0 / 1920.0, 1.0 / 1080.0);
	float initShade = saturate(pow(.75, power));
	float shading = initShade;
	float add = (1 / 26.0) *  saturate((1 - depth) * 30);
	float DiscRadius = .25;

	float ss = 0;// shadowSample(lightSamplePos);

	if (CastShadow) 
	{
		float ss =  shadowSample(shadowSampler, lightSamplePos);

		if (ss < realDistanceToLight)
		{
			if (hardShadows)
				shading = 0;
			else
				shading = SoftShadow(ss, depth, lightSamplePos, shading, realDistanceToLight, shadowSampler, worldPos*1000.0);
		}
	}

	shading = saturate(shading);


	float3 lightVector = normalize(-lightDirection);


	//compute diffuse light
	float NdL = saturate(dot(lightVector, normal));
	float3 diffuseLight = (NdL * Color.rgb) * power;

	// specular
	float4 sgr = tex2D(sgrSampler, input.texCoord);

	float3 directionToCamera = worldPos.xyz - CameraPosition.xyz;


	float3 Half = normalize(normalize(directionToCamera)) + lightVector;

	float specular = pow(saturate(dot(normal, Half)), 2);	
	float specCol = 1 * sgr.r * specular;// *NdL * power;
	//specCol *= shading;

	diffuseLight = ((diffuseLight + specCol) * shading);

	//output the two lights
	return  (float4(diffuseLight.rgb, 1));
}

technique DirectionalLight
{
	pass Pass1
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL DirectionalLightPS();
	}
}