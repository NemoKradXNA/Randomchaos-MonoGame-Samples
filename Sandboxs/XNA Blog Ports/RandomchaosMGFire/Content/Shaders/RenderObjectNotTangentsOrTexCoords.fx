float4x4 world : WORLD;
float4x4 wvp : WorldViewProjection;

float3 lightDirection;
float4 color = 1;

texture textureMat;

sampler textureSample = sampler_state
{
	texture = <textureMat>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

texture BumpMap;
sampler BumpMapSampler = sampler_state
{
	Texture = <BumpMap>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float3 Normal	: Normal0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float3 Normal : Normal0;
	float4 SPos : TexCoord1;
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

    output.Normal = mul(input.Normal, world);

    output.SPos = output.Position;

    return output;
}

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	PixelShaderOutput output = (PixelShaderOutput)0;

	float4 Color = color;

	// Get value in the range of -1 to 1
	float3 n = normalize(input.Normal);
	float3 ld = normalize(lightDirection);
	float dif = clamp(saturate(dot(-ld, n)), .25, 1);

	output.Color = Color * dif;

	// Depth
	output.Depth.r = 1 - (input.SPos.z / input.SPos.w);
	output.Depth.a = 1;

	return output;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_4_0_level_9_1 VertexShaderFunction();
        PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
    }
}
