#include "ShaderTools.fxh"

float4x4 wvp : WorldViewProjection;
float4x4 world : World;

float AmbientIntensity = 1;
float4 AmbientColor : AMBIENT = float4(0,0,0,1);

float3 LightDirection : Direction = float3(0,1,1);

float3 CameraPosition : CameraPosition; 

float time : Time;

float cloudSpeed = .0025;
float cloudHeight = .005;
float cloudShadowIntensity = 1;

float cloudVertOffset = .02;
float outerAtmosVertOffset = .2;

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

texture CloudMap;
sampler CloudMapSampler = sampler_state
{
	texture = <CloudMap>;    
};

texture WaveMap;
sampler WaveMapSampler = sampler_state
{
	texture = <WaveMap>;    
};

texture AtmosMap;
sampler AtmosMapSampler = sampler_state
{
	texture = <AtmosMap>;    
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
struct VS_OUT2
{
	float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float4 pos : TEXCOORD2;
};
struct PS_OUT
{
	float4 Color : COLOR;
};

VS_OUT VS_Color(VS_IN input)
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

PS_OUT PS_Color(VS_OUT input)
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
	float3 regN = normalize(input.Normal);
	float3 Half = normalize(normalize(LightDirection) + normalize(input.CamView));	
	float specular = pow(saturate(dot(regN,Half)),25);
	float4 specCol = .75 * tex2D(ReflectionMapSampler,input.TexCoord) * (specular * Diffuse);
	
	float shadowHeight = cloudHeight * dot(regN,normalize(input.Light));
	
	float2 shadowAngle = input.TexCoord - float2(shadowHeight,shadowHeight);
	float2 cloudCoordS = RotateRight(shadowAngle,time*cloudSpeed);	
	
	float cloudShadow = (-tex2D(CloudMapSampler,cloudCoordS).a) * cloudShadowIntensity;
	cloudShadow *= Diffuse;	
	
	output.Color =  (Ambient + texCol + 0 + cloudShadow +  glow + specCol);
	
	return output;
}
PS_OUT PS_Cloud(VS_OUT2 input)
{
	PS_OUT output = (PS_OUT)0;

	float3 Normal = normalize(input.Normal);
	
	float3 LightDir = normalize(LightDirection);
	float Diffuse = saturate(dot(LightDir,Normal));

	float2 cloudCoord = RotateRight(input.TexCoord,time*cloudSpeed);
	
	float4 clouds = tex2D(CloudMapSampler,cloudCoord);	
	float4 cloudsN = tex2D(CloudMapSampler,cloudCoord);
		
	clouds *= Diffuse;	
	cloudsN *= saturate(.25-Diffuse);
	
	output.Color =  (clouds + cloudsN) * 2;
	
	return output;
}

PS_OUT PS_Water(VS_OUT input)
{
	PS_OUT output = (PS_OUT)0;
	
	float3 LightDir = normalize(input.Light);
	
	float3 Normal = (2 * (tex2D(WaveMapSampler,MoveInCircle(input.TexCoord,time,.001) * 50))) - 1.0;	
	float Diffuse = saturate(dot(LightDir,Normal));
	float4 wav = tex2D(ReflectionMapSampler,input.TexCoord) * saturate(dot(LightDir,Normal));//*.75;
	
	output.Color = wav * .5;

	return output;
}
PS_OUT PS_OuterAtmoshpere(VS_OUT2 input)
{
	PS_OUT output = (PS_OUT)0;
	
	// Do light scatter...
	float4 atmos = tex2D(AtmosMapSampler,input.TexCoord);

	float3 regN = normalize(input.Normal);
	float3 Half = normalize(normalize(CameraPosition-input.pos) + normalize(CameraPosition - input.pos));	
	float specular = 0;
	
	// Was playing about to see if I could get a better scatter, worked a bit, but not 100%
	// Guess I will need to read up on how to do it properly :P
	float Diffuse = 1-saturate(dot(normalize(LightDirection),-regN)) * 4;
		
	specular = 1-saturate(1.125 + dot(regN,Half)* .666) ;
	//specular = 1-saturate(1.1 + dot(regN,Half));
	atmos *= specular;
	
	output.Color = (atmos * Diffuse) * 1.5;
	
	
	return output;
}
PS_OUT PS_OuterAtmoshpereFlipped(VS_OUT2 input)
{
	PS_OUT output = (PS_OUT)0;

	// Do light scatter...
	float4 atmos = tex2D(AtmosMapSampler, input.TexCoord);

	float3 regN = normalize(input.Normal);
	float3 Half = normalize(normalize(CameraPosition - input.pos) + normalize(CameraPosition - input.pos));
	float specular = 0;

	// Was playing about to see if I could get a better scatter, worked a bit, but not 100%
	// Guess I will need to read up on how to do it properly :P
	float Diffuse = 1 - saturate(dot(normalize(LightDirection), -regN)) * 4;

	specular = (saturate(1 - dot(regN, Half)) * .333);
	atmos *= specular;
	
	output.Color = (atmos * Diffuse) *1.5;

	return output;
}

VS_OUT2 VS_InnerAtmoshpere(VS_IN input)
{
	VS_OUT2 output = (VS_OUT2)0;
	output.Normal = mul(input.Normal, world);
	output.Position = mul(input.Position, wvp) + (mul(cloudVertOffset, mul(input.Normal, wvp)));
	output.TexCoord = input.TexCoord;
	output.pos = mul(input.Position, world);

	return output;
}

VS_OUT2 VS_OuterAtmoshpere(VS_IN input)
{
	VS_OUT2 output = (VS_OUT2)0;
	output.Normal = mul(input.Normal, world);
	output.Position = mul(input.Position, wvp) + (mul(outerAtmosVertOffset, mul(input.Normal, wvp)));
	output.TexCoord = input.TexCoord;
	output.pos = mul(input.Position,world);
	
	return output;
}
technique PlanetShader
{
	pass Colors
	{
		AlphaBlendEnable = False;
		CullMode = CCW;
		VertexShader = compile vs_4_0 VS_Color();
		PixelShader = compile ps_4_0 PS_Color();		
	}

	pass Waves
	{
		AlphaBlendEnable = True;
        SrcBlend = SrcAlpha;
        DestBlend = One;
        
		PixelShader = compile ps_4_0 PS_Water();
	}	
	pass Clouds
	{
		// Already set, no need to set again, keep them incase I move the passes about though.
		//AlphaBlendEnable = True;
        //SrcBlend = SrcAlpha;
        //DestBlend = One;
        
        VertexShader = compile vs_4_0 VS_InnerAtmoshpere();
		PixelShader = compile ps_4_0 PS_Cloud();
	}			
	
	pass OuterAtmoshpere
	{
		//AlphaBlendEnable = True;
        //SrcBlend = SrcAlpha;
        DestBlend = InvSrcAlpha;
        
        // No need to move it out again, can use the same geom as the clouds.
		//VertexShader = compile vs_2_0 VS_OuterAtmoshpere(.02);
		PixelShader = compile ps_4_0 PS_OuterAtmoshpereFlipped();
	}
	
	pass UpperOuterAtmoshpere
	{
		//AlphaBlendEnable = True;
        //SrcBlend = SrcAlpha;
        //DestBlend = InvSrcAlpha;
        
		VertexShader = compile vs_4_0 VS_OuterAtmoshpere();
		PixelShader = compile ps_4_0 PS_OuterAtmoshpere();
		CullMode = CW;
	}		
	
}
