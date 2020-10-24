float4x4 wvp : WorldViewProjection;
float4x4 world : World;

float AmbientIntensity = 1;
float4 AmbientColor : AMBIENT = float4(0,0,0,1);

float3 LightDirection : Direction = float3(0,1,1);

texture ColorMap : Diffuse;
sampler ColorMapSampler = sampler_state 
{
    texture = <ColorMap>;
};

texture GlowMap : Diffuse;
sampler GlowMapSampler = sampler_state 
{
    texture = <GlowMap>;
};

struct VS_IN
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
	float3 Normal : NORMAL;	
};
struct VS_OUT
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
	float3 Light : TEXCOORD1;
	float3 Normal : TEXCOORD2;	
};
struct PS_OUT
{
	float4 Color : COLOR;
};

VS_OUT VS_ColorGlowMap(VS_IN input)
{
	VS_OUT output = (VS_OUT)0;
	
	output.Position = mul(input.Position,wvp);	
	output.Light = LightDirection;
	output.TexCoord = input.TexCoord;
	
	output.Normal = mul(input.Normal,world);
	
	return output;
}
PS_OUT PS_ColorGlowMap(VS_OUT input)
{
	PS_OUT output = (PS_OUT)0;
	
	float3 LightDir = normalize(input.Light);
	float Diffuse = saturate(dot(LightDir,normalize(input.Normal)));
		
	float4 texCol = tex2D(ColorMapSampler,input.TexCoord);
	float4 glowCol = tex2D(GlowMapSampler,input.TexCoord);
	float4 Ambient = AmbientIntensity * AmbientColor;
			
	float4 glow = glowCol * saturate(1-Diffuse);	
	
	texCol *= Diffuse;
	
	output.Color =  Ambient + texCol + glow;
	
	return output;
}

technique ColorGlowMap
{
	pass Pass0
	{
		VertexShader = compile vs_4_0 VS_ColorGlowMap();
		PixelShader = compile ps_4_0 PS_ColorGlowMap();
	}
}