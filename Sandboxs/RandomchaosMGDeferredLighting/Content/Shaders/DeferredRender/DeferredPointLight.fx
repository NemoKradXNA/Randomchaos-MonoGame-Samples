#include "DeferredHeader.fxh"

float4x4 wvp;

//color of the light 
float3 Color;

//position of the camera, for specular light
float3 cameraPosition;

//this is used to compute the world-position
float4x4 InvertViewProjection;

//this is the position of the light
float3 lightPosition;

//how far does this light reach
float lightRadius;

//control the brightness of the light
float lightIntensity = 1.0f;

// diffuse color, and specularIntensity in the alpha channel
texture colorMap;
// normals, and specularPower in the alpha channel
texture normalMap;
//depth
texture depthMap;

texture sgrMap;
sampler SGRSampler = sampler_state
{
	Texture = (sgrMap);
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	Mipfilter = LINEAR;
};

sampler colorSampler = sampler_state
{
	Texture = (colorMap);
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = LINEAR;
	MinFilter = LINEAR;
	Mipfilter = LINEAR;
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
sampler normalSampler = sampler_state
{
	Texture = (normalMap);
	AddressU = CLAMP;
	AddressV = CLAMP;
	MagFilter = POINT;
	MinFilter = POINT;
	Mipfilter = POINT;
};


struct VertexShaderInput
{
	float4 Position : SV_POSITION;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 ScreenPosition : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	//processing geometry coordinates
	output.Position = mul(input.Position, wvp);
	output.ScreenPosition = output.Position;
	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	//obtain screen position
	input.ScreenPosition.xy /= input.ScreenPosition.w;

    float2 texCoord = 0.5f * (float2(input.ScreenPosition.x,-input.ScreenPosition.y) + 1);
    //texCoord -= halfPixel;

    float4 normalData = tex2D(normalSampler,texCoord);
    float3 normal = 2.0f * normalData.xyz - 1.0f;

    float depthVal = 1 - tex2D(depthSampler,texCoord).r;

    //compute screen-space position
    float4 position;
    position.xy = input.ScreenPosition.xy;
    position.z = depthVal;
    position.w = 1.0f;

    position = mul(position, InvertViewProjection);
    position /= position.w;

    //surface-to-light vector
    float3 lightVector = lightPosition - position;

    //compute attenuation based on distance - linear attenuation
    float attenuation = saturate(1.0f - length(lightVector) / lightRadius);
    //attenuation = 1;

    //normalize light vector
    lightVector = normalize(lightVector);

    //compute diffuse light
    float NdL = saturate(dot(lightVector, normal));
	float3 diffuseLight = NdL * Color.rgb;

    //reflection vector
    float3 reflectionVector = normalize(reflect(normal, -lightVector));
    //camera-to-surface vector
	float3 directionToCamera = normalize(cameraPosition - position);

	float4 sgr = tex2D(SGRSampler, texCoord);

    float3 Half = normalize(directionToCamera) + reflectionVector;
    float specular = pow(saturate(dot(normal,Half)),25) * sgr.r;

	return (attenuation * lightIntensity * float4(diffuseLight.rgb, 1)) + (specular * attenuation * lightIntensity);// +glow;
}

technique Technique1
{
	pass Pass1
	{
		CULLMODE = CW;
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
}
