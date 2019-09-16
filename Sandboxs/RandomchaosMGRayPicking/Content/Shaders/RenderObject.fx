float4x4 world : WORLD;
float4x4 wvp : WorldViewProjection;

float3 lightDirection;

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
        
    output.Normal = mul(input.Normal,world);

	output.Tangent[0] = normalize(mul(input.Tangent,world));
	output.Tangent[1] = normalize(mul(cross(input.Tangent,input.Normal),world));
	output.Tangent[2] = normalize(output.Normal);

	output.SPos = output.Position;

    return output;
}

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    PixelShaderOutput output = (PixelShaderOutput)0;
	
	float4 Color = tex2D(textureSample,input.TexCoord);
		
	// Get value in the range of -1 to 1
	float3 n = 2.0f * tex2D(BumpMapSampler, input.TexCoord) - 1.0f;
	n = mul(n, input.Tangent);

	float3 lightVector = normalize(-lightDirection);

	float NdL = saturate(dot(lightVector, n));

	output.Color = Color * NdL;

	// Depth
	output.Depth.r = 1-(input.SPos.z/input.SPos.w); 
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
