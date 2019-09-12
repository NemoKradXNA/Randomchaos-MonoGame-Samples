// Shader for creating a Depth of field postprocess.
// Also includes a technique for rendering a depth
// values of the scene.

// Depth buffer resolve parameters
// x = focus distance
// y = focus range
// z = near clip
// w = far clip / ( far clip - near clip )
float4 DoFParams;

uniform extern texture SceneTex;
//uniform extern texture SceneBlurTex;
uniform extern texture SceneDepthTex;

float2 halfPixel;

// Textures
sampler2D SceneSampler = sampler_state
{
	Texture = <SceneTex>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
};
sampler2D SceneBlurSampler : register(s0);

sampler2D SceneDepthSampler = sampler_state
{
	Texture = <SceneDepthTex>;
	MinFilter = Point;
	MagFilter = Point;
	MipFilter = None;
};

struct OutputVS
{
    float4 posH    : POSITION0;
    float2 depth   : TEXCOORD0;
};

///Drawing the depth of field
float4 DepthOfFieldPS_R32F(float4 Position : SV_POSITION,
	float4 Color : COLOR0,
	float2 texCoord : TEXCOORD0) : COLOR0
{
	texCoord -= halfPixel;
	
	float4 vSceneBlurSampler = tex2D(SceneBlurSampler, texCoord);
    float4 vSceneSampler      = tex2D( SceneSampler, texCoord );	
	float vDepthTexel = 1 - tex2D(SceneDepthSampler, texCoord);
    
    
    // This is only required if using a floating-point depth buffer format
	//fDepth = 1.0 - fDepth;
	float fDepth = vDepthTexel;

    // Back-transform depth into camera space
    float fSceneZ = ( -DoFParams.z * DoFParams.w ) / ( fDepth - DoFParams.w );
    
    // Compute blur factor
    float fBlurFactor = 0;

	if(fSceneZ <= DoFParams.x)
		fBlurFactor = saturate( ( DoFParams.x - fSceneZ ) / DoFParams.y );
	else
	fBlurFactor = saturate( ( fSceneZ - DoFParams.x ) / DoFParams.y );
    
	// Compute resultant pixel
    float4 color;
    color.rgb = lerp( vSceneSampler.rgb, vSceneBlurSampler.rgb, fBlurFactor );
    color.a	= 1.0;

    return color;
}

technique DepthOfField_R32F
{
    pass P0
    {
        PixelShader = compile ps_4_0_level_9_1 DepthOfFieldPS_R32F();
    }
}