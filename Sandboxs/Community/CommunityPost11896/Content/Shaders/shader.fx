// Shader origin: https://github.com/CartBlanche/MonoGame-Samples/blob/master/SpriteEffects/Content/refraction.fx

// Effect uses a scrolling displacement texture to offset the position of the main
// texture. Depending on the contents of the displacement texture, this can give a
// wide range of refraction, rippling, warping, and swirling type effects.

float2 DisplacementScroll;

Texture2D Texture;
sampler TextureSampler 
{
	Texture = <Texture>;
};

Texture2D Displacement;
sampler DisplacementSampler 
{
	Texture = <Displacement>;
};


struct vsOutput
{
	float4 position : SV_Position;
	float4 color : COLOR0;
	float2 texCoord : TEXCOORD0;
};

float4 main(vsOutput input) : COLOR0
{
	// Comment this line out to break the shader!!!
	float4 v =  tex2D(TextureSampler, input.texCoord);
	

	// Look up the displacement amount.
	float2 displacement = tex2D(DisplacementSampler, DisplacementScroll + input.texCoord / 3).xy;

	// Offset the main texture coordinates.
	input.texCoord += displacement * 0.2 - 0.15;

	// Look up into the main texture.
	return tex2D(TextureSampler, input.texCoord) * input.color;
}


technique Refraction
{
	pass Pass1
	{
		PixelShader = compile ps_4_0 main();
	}
}