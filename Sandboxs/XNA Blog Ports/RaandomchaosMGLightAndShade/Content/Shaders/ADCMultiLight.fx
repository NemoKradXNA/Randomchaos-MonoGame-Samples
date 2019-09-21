float4x4 world : World;
float4x4 wvp : WorldViewProjection;
float4x4 itw : WorldInverseTranspose;

float AmbientIntensity = 1;
float4 AmbientColor : Ambient = float4(.5,.5,.5,1);

#define MaxLights 3

float DiffuseIntensity[MaxLights] = {1,1,1};
float4 DiffuseColor[MaxLights] : Diffuse = {float4(1,0,0,1),float4(0,1,0,1),float4(0,0,1,1)};
float3 LightDirection[MaxLights] : Direction = {float3(1,0,0),float3(0,1,0),float3(0,0,1)};

struct VS_IN
{
	float4 Position : POSITION;
	float3 Normal : NORMAL;
};

struct VS_OUT
{
	float4 Position : POSITION;
	float3 Light[MaxLights] : TEXCOORD0;
	float3 Normal : TEXCOORD3;
};

struct PS_OUT
{
	float4 Color : COLOR;
};

VS_OUT VS_AmbientDiffuse(VS_IN input)
{
	VS_OUT output = (VS_OUT)0;
	
	output.Position = mul(input.Position,wvp);
	
	// Normalize the light directions
	for(int l=0;l<MaxLights;l++)
		output.Light[l] = normalize(LightDirection[l]);
	
	output.Normal = normalize(mul(itw,input.Normal));	
	
	return output;
}

PS_OUT PS_AmbientDiffuse(VS_OUT input)
{
	PS_OUT output = (PS_OUT)0;
	
	// Get ambient light
	float4 ambient = AmbientColor * AmbientIntensity;
	
	// Get diffuse light
	float4 TotalDiffuseColor = float4(0,0,0,0);
	
	for(int l=0;l<MaxLights;l++)
		TotalDiffuseColor += (DiffuseIntensity[l] * DiffuseColor[l]) * saturate(dot(input.Light[l],input.Normal));

	output.Color = ambient + TotalDiffuseColor;
	
	return output;
}

technique AmbientDiffuseLight
{
	pass Pass0
	{
		VertexShader = compile vs_4_0 VS_AmbientDiffuse();
		PixelShader = compile ps_4_0 PS_AmbientDiffuse();
	}
}