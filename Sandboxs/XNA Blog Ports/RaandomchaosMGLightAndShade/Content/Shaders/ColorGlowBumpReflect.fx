float4x4 wvp : WorldViewProjection;
float4x4 world : World;
float AmbientIntensity = 1;
float4 AmbientColor : AMBIENT = float4(0,0,0,1);

float3 LightDirection : Direction = float3(0,1,1);

float3 CameraPosition : CameraPosition; 

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

texture BumpMap ;
sampler BumpMapSampler = sampler_state
{
	texture = <BumpMap>;	
};

texture ReflectionMap : Diffuse;
sampler ReflectionMapSampler = sampler_state 
{
    texture = <ReflectionMap>;    
};


struct VS_IN
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
	float3 Normal : NORMAL;
	float3 Tangent : TANGENT;
};
struct VS_OUT
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
	float3 Light : TEXCOORD1;
	float3 CamView : TEXCOORD2;
	float4 posS : TEXCOORD3;
	float3 Normal : TEXCOORD4;	
};
struct PS_OUT
{
	float4 Color : COLOR;
};


VS_OUT VS_Ambient(VS_IN input)
{
	VS_OUT output = (VS_OUT)0;
	
	output.Position = mul(input.Position,wvp);
	
	float3x3 worldToTangentSpace;
	worldToTangentSpace[0] = mul(input.Tangent,world);
	worldToTangentSpace[1] = mul(cross(input.Tangent,input.Normal),world);
	worldToTangentSpace[2] = mul(input.Normal,world);
	
	float4 PosWorld = mul(input.Position,world);
	
	output.Light = mul(worldToTangentSpace,LightDirection);	
	output.CamView = CameraPosition - mul(input.Position,world);
	
	output.posS = input.Position;
	
	output.TexCoord = input.TexCoord;
	
	output.Normal = mul(input.Normal,world);
	
	return output;
}
PS_OUT PS_Ambient(VS_OUT input)
{
	PS_OUT output = (PS_OUT)0;
	
	float3 Normal = (2 * (tex2D(BumpMapSampler,input.TexCoord))) - 1.0;
	
	float3 LightDir = normalize(input.Light);
	float Diffuse = saturate(dot(LightDir,Normal));
		
	float4 texCol = tex2D(ColorMapSampler,input.TexCoord);
	float4 glowCol = tex2D(GlowMapSampler,input.TexCoord);
	
	float4 Ambient = AmbientIntensity * AmbientColor;
	
	float4 glow = glowCol * saturate(1-Diffuse);		
		
	texCol *= Diffuse;
	
	float3 Half = normalize(normalize(LightDirection) + normalize(input.CamView));	
	float specular = pow(saturate(dot(normalize(input.Normal),Half)),25);
	float4 specCol = 2 * tex2D(ReflectionMapSampler,input.TexCoord) * (specular * Diffuse);
	
	output.Color =  Ambient + texCol + glow + specCol;
	
	return output;
}

technique AmbientLight
{
	pass Pass0
	{
		VertexShader = compile vs_4_0 VS_Ambient();
		PixelShader = compile ps_4_0 PS_Ambient();
	}
}
