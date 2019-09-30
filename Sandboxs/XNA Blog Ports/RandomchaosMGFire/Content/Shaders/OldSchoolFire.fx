//////////////////////////////////////////////////////
//													//
//	Old School Fire Shader							//
//													//
//	By: C.Humphrey (aka Nemo Krad)					//
//													//
//	xna.net/blogs/randomchaos						//
//													//
//////////////////////////////////////////////////////

bool cap = false;

// Screen pixel values 
uniform extern texture fireMap;
sampler map = sampler_state 
{
	Texture = <fireMap>;
};

sampler screen : register(s0);

// Screen dims
float height = 480;
float width = 800;

// Oxygen level
float oxygen = 1;

// color palette
texture paletteMap;
sampler palette = sampler_state
{
	Texture = <paletteMap>;
	AddressU = CLAMP;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TexCoord : TEXCOORD0;
};

// Fire Map function
float4 UpdateFireMap(VertexShaderOutput input) : COLOR0
{
	float col = 0;

	// Calculate the size of a pixel in texture space
	float2 pix = float2(1,1) / float2(width,height);

	float4 scrn = tex2D(screen, input.TexCoord);
	// Sample the pixels below me.
	if (cap)
	{
		if ((input.TexCoord.y + pix.y) < 1 - pix.y)
		{
			col += tex2D(map,float2(input.TexCoord.x - pix.x,input.TexCoord.y + pix.y));
			col += tex2D(map,float2(input.TexCoord.x,input.TexCoord.y + (pix.y * 2)));
			col += tex2D(map,float2(input.TexCoord.x + pix.x,input.TexCoord.y + pix.y));
			col += tex2D(map,float2(input.TexCoord.x,input.TexCoord.y + (pix.y * 2)));
		}
	}
	if (!cap)
	{
		col += tex2D(map,float2(input.TexCoord.x - pix.x,input.TexCoord.y + pix.y));
		col += tex2D(map,float2(input.TexCoord.x,input.TexCoord.y + (pix.y * 2)));
		col += tex2D(map,float2(input.TexCoord.x + pix.x,input.TexCoord.y + pix.y));
		col += tex2D(map,float2(input.TexCoord.x,input.TexCoord.y + (pix.y * 2)));
	}

	// Divied the sum by the 4 + the oxygen mod.
	col /= (4 + oxygen);

	// return the map value;
	return  float4(col, col, col, 1);
}

// Flame color function
float4 ColorMap(VertexShaderOutput input) : COLOR0
{
	// Get this pixels map value and color it based on the palette
	float col = tex2D(map, input.TexCoord);

	// Write it out to the screen :D
	float4 pcol = tex2D(palette,float2(col,0));

	float4 p = tex2D(palette, input.TexCoord);

	return pcol;
}

// Technique to build/update fire map
technique UpdateFire
{
	pass Pass1
	{
		PixelShader = compile ps_4_0 UpdateFireMap();
	}
}
// Technique to color the fire map
technique ColorFire
{
	pass Pass1
	{
		PixelShader = compile ps_4_0 ColorMap();
	}
}
