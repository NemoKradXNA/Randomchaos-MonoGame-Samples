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

VS_OUT VS_ColorMap(VS_IN input)
{
	VS_OUT output = (VS_OUT)0;
	
	output.Position = mul(input.Position,wvp);	
	output.Light = LightDirection;
	output.TexCoord = input.TexCoord;
	
	output.Normal = mul(input.Normal,world);
	
	return output;
}
PS_OUT PS_ColorMap(VS_OUT input)
{
	PS_OUT output = (PS_OUT)0;
	
	float3 LightDir = normalize(input.Light);
	float Diffuse = saturate(dot(LightDir,normalize(input.Normal)));
		
	float4 texCol = tex2D(ColorMapSampler,input.TexCoord);
	float4 Ambient = AmbientIntensity * AmbientColor;
			
	texCol *= Diffuse;
		
	output.Color =  Ambient + texCol;
	
	return output;
}

technique ColorMapTechnique
{
	pass Pass0
	{
		VertexShader = compile vs_4_0 VS_ColorMap();
		PixelShader = compile ps_4_0 PS_ColorMap();
	}
}