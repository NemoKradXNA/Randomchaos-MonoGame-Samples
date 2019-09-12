//////////////////////////////////////////////////////////////////
//																//
//	Sun and lens flare as a post process						//
//		By: C.Humphrey											//
//																//
//	Shader written for www.sgtconker.com						//
//																//
//	Find my blog here: xna-uk.net/blogs/randomchaos				//
//																//
//	14/04/2010													//
//																//
//////////////////////////////////////////////////////////////////

float4x4 VP : ViewProjection;

float3 cameraPosition; 

float SunSize = 1500;

float Density = 3;
float Exposure = -.1;
float Weight = .25;
float Weight2 = .125;
float Decay = .5;

float3 lightPosition;

float lightIntensity = 10.0f;
float3 Color = float3(1,1,1);

texture flare;
sampler Flare = sampler_state
{
    Texture = (flare);
    AddressU = CLAMP;
    AddressV = CLAMP;
};

sampler BackBuffer : register(s0);

//depth
texture depthMap;
sampler depthSampler = sampler_state
{
    Texture = (depthMap);   
	MinFilter = Point;
	MagFilter = Point;
	MipFilter = None;
};


struct VertexShaderOutputToPS
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 texCoord : TEXCOORD0;
};

float4 DoLenseFlare(float4 ScreenLightPosition,float2 texCoord,bool fwd)
{
	// Calculate vector from pixel to light source in screen space.  
	float2 deltaTexCoord = (texCoord - ScreenLightPosition.xy);  
	
	// Divide by number of samples and scale by control factor.  
	deltaTexCoord *= 1.0f / 3 * Density;  
	
	// Store initial sample.  
	float3 color = 0;
	
	// Set up illumination decay factor.  
	float illuminationDecay = 1.0f;  
		
	for (int i = 0; i < 3 ; i++)  
	{  	
		// Step sample location along ray.  
		if(fwd)
			texCoord -= deltaTexCoord;  
		else
			texCoord += deltaTexCoord;  
		// Retrieve sample at new location.  
		float3 sample = tex2D(Flare, texCoord);
		
		// Apply sample attenuation scale/decay factors.  
		if(fwd)
			sample *= illuminationDecay * Weight;  
		else
			sample *= illuminationDecay * Weight2;			
		
		// Accumulate combined color.  
		color += sample;  
		// Update exponential decay factor.  
		illuminationDecay *= Decay;
	}  

	return float4(color,1);
}

float4 PixelShaderFunction(VertexShaderOutputToPS input) : COLOR0
{
	// Get the scene
	float4 col = tex2D(BackBuffer,input.texCoord);
	
	// Find the suns position in the world and map it to the screen space.
	float4 ScreenPosition = mul(lightPosition - cameraPosition,VP);
	float scale = ScreenPosition.z;
	ScreenPosition.xyz /= ScreenPosition.w;
	ScreenPosition.x = ScreenPosition.x/2.0f+0.5f;
	ScreenPosition.y = (-ScreenPosition.y/2.0f+0.5f);
	
	// get the depth from the depth map
	float depthVal = 1 - tex2D(depthSampler, input.texCoord).r;	 
	
	// Are we looking in the direction of the sun?
	if(ScreenPosition.w > 0)
	{		
		float2 coord;
		
		float size = SunSize / scale;
					
		float2 center = ScreenPosition.xy;

		coord = .5 - (input.texCoord - center) / size * .5;
		
		if (depthVal > ScreenPosition.z - .0003)
			col += (pow(tex2D(Flare, coord) * float4(Color, 1), 2) * lightIntensity) * 2;
		
		// Lens flare
		col += ((DoLenseFlare(ScreenPosition,input.texCoord,true) + DoLenseFlare(ScreenPosition,input.texCoord,false)) * float4(Color,1) * lightIntensity) * 5;
	}
	
	return col;	
}

technique SunRender
{
    pass Sun
    {
        PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
    }
}
