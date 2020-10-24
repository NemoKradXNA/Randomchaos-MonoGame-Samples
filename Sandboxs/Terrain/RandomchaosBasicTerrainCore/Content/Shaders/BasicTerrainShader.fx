

float4x4 world : WORLD;
float4x4 wvp : WorldViewProjection;

float3 lightDirection;

float2 UVMultiplier = float2(1, 1);

float2 UVChannel0 = float2(1, 1);
float2 UVChannel1 = float2(1, 1);
float2 UVChannel2 = float2(1, 1);
float2 UVChannel3 = float2(1, 1);

texture splatMap;
sampler splatMapSample = sampler_state
{
	texture = <splatMap>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

texture textureChannel0;
sampler textureChannel0Sample = sampler_state
{
	texture = <textureChannel0>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

texture textureChannel1;
sampler textureChannel1Sample = sampler_state
{
	texture = <textureChannel1>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};


texture textureChannel2;
sampler textureChannel2Sample = sampler_state
{
	texture = <textureChannel2>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};


texture textureChannel3;
sampler textureChannel3Sample = sampler_state
{
	texture = <textureChannel3>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

texture normalChannel0;
sampler normalChannel0Sample = sampler_state
{
	texture = <normalChannel0>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

texture normalChannel1;
sampler normalChannel1Sample = sampler_state
{
	texture = <normalChannel1>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};


texture normalChannel2;
sampler normalChannel2Sample = sampler_state
{
	texture = <normalChannel2>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};


texture normalChannel3;
sampler normalChannel3Sample = sampler_state
{
	texture = <normalChannel3>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};


struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TexCoord : TexCoord0;
	float3 Normal	: Normal0;
	float3 Tangent	: Tangent0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TexCoord : TexCoord0;
	float3 Normal : Normal0;
	float4 SPos : TexCoord1;
	float3x3 Tangent: Tangent0;
};

struct PixelShaderOutput
{
	float4 Color	: COLOR0;	// Color	
	float4 Depth	: COLOR1;	// Depth map 
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(input.Position, wvp);
	output.TexCoord = input.TexCoord;

	output.Normal = mul(input.Normal, world);

	output.Tangent[0] = normalize(mul(input.Tangent, world));
	output.Tangent[1] = normalize(mul(cross(input.Tangent, input.Normal), world));
	output.Tangent[2] = normalize(output.Normal);

	output.SPos = output.Position;

	return output;
}

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	PixelShaderOutput output = (PixelShaderOutput)0;

	float4 splatRaw = tex2D(splatMapSample, input.TexCoord);
	float4 splat = splatRaw;
	float4 splatBase = splatRaw;

	float2 uv0 = input.TexCoord * UVChannel0;
	float2 uv1 = input.TexCoord * UVChannel1;
	float2 uv2 = input.TexCoord * UVChannel2;
	float2 uv3 = input.TexCoord * UVChannel3;

	float4x4 col;
	// Diffuse
	col[0] = tex2D(textureChannel0Sample, uv0);
	col[1] = tex2D(textureChannel1Sample, uv1);
	col[2] = tex2D(textureChannel2Sample, uv2);
	col[3] = tex2D(textureChannel3Sample, uv3);

	float4x4 norm;
	// Normal
	norm[0] = tex2D(normalChannel0Sample, uv0);
	norm[1] = tex2D(normalChannel1Sample, uv1);
	norm[2] = tex2D(normalChannel2Sample, uv2);
	norm[3] = tex2D(normalChannel3Sample, uv3);

	float3 n = 2.0f * mul(splat, norm) - 1.0f;
	n = mul(n, input.Tangent);

	float3 lightVector = normalize(-lightDirection);

	float NdL = saturate(dot(lightVector, n));

	output.Color = mul(splat, col) * NdL;

	// Depth
	output.Depth.r = 1 - (input.SPos.z / input.SPos.w);
	output.Depth.a = 1;

	return output;
}

technique Technique1
{
	pass Pass1
	{
		//FILLMODE = WIREFRAME;
		VertexShader = compile vs_4_0 VertexShaderFunction();
		PixelShader = compile ps_4_0 PixelShaderFunction();
	}
}
