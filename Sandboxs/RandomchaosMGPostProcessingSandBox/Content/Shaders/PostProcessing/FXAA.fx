// Taken from https://catlikecoding.com/unity/tutorials/advanced-rendering/fxaa/
//#define LOW_QUALITY

//#define GAMMA_BLENDING

//#if defined(LOW_QUALITY)
//	#define EDGE_STEP_COUNT 4
//	#define EDGE_STEPS 1, 1.5, 2, 4
//	#define EDGE_GUESS 12
//#else
	#define EDGE_STEP_COUNT 10
	#define EDGE_STEPS 1, 1.5, 2, 2, 2, 2, 2, 2, 2, 4
	#define EDGE_GUESS 8
//#endif

static const float edgeSteps[EDGE_STEP_COUNT] = { EDGE_STEPS };

#include "PPVertexShader.fxh"

sampler screen : register(s0)
{
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

int width;
int height;

bool RenderHalfScreen = false;

float VLine;

// Trims the algorithm from processing darks.
//   0.0833 - upper limit (default, the start of visible unfiltered edges)
//   0.0625 - high quality (faster)
//   0.0312 - visible limit (slower)
float _ContrastThreshold;

// The minimum amount of local contrast required to apply algorithm.
//   0.333 - too little (faster)
//   0.250 - low quality
//   0.166 - default
//   0.125 - high quality 
//   0.063 - overkill (slower)
float _RelativeThreshold;

// Choose the amount of sub-pixel aliasing removal.
// This can effect sharpness.
//   1.00 - upper limit (softer)
//   0.75 - default amount of filtering
//   0.50 - lower limit (sharper, less sub-pixel aliasing removal)
//   0.25 - almost off
//   0.00 - completely off
float _SubpixelBlending;


float LinearRgbToLuminance(float3 linearRgb)
{
	return dot(saturate(linearRgb), float3(0.2126729f, 0.7151522f, 0.0721750f));
}

float4 Sample(float2 uv)
{
	return tex2D(screen, uv);
}

float LinearToGammaSpace(float f)
{
	return pow(f, 1 / 2.2);
}

float SampleLuminance(float2 uv) 
{
#if defined(GAMMA_BLENDING)
	return LinearToGammaSpace(saturate(Sample(uv)).rgb);
#endif
	return LinearRgbToLuminance(saturate(Sample(uv)).rgb);
}

float SampleLuminance(float2 uv, float uOffset, float vOffset)
{

	float2 pix = float2(1, 1) / float2(width, height);

	uv += pix * float2(uOffset, vOffset);
	return SampleLuminance(uv);
}

struct LuminanceData 
{
	float m, n, e, s, w;
	float ne, nw, se, sw;
	float highest, lowest, contrast;
};

LuminanceData SampleLuminanceNeighborhood(float2 uv)
{
	LuminanceData l;
	l.m = SampleLuminance(uv);
	l.n = SampleLuminance(uv, 0, 1);
	l.e = SampleLuminance(uv, 1, 0);
	l.s = SampleLuminance(uv, 0, -1);
	l.w = SampleLuminance(uv, -1, 0);

	l.ne = SampleLuminance(uv, 1, 1);
	l.nw = SampleLuminance(uv, -1, 1);
	l.se = SampleLuminance(uv, 1, -1);
	l.sw = SampleLuminance(uv, -1, -1);

	l.highest = max(max(max(max(l.n, l.e), l.s), l.w), l.m);
	l.lowest = min(min(min(min(l.n, l.e), l.s), l.w), l.m);
	l.contrast = l.highest - l.lowest;
	return l;
}

bool ShouldSkipPixel(LuminanceData l) 
{
	float threshold = max(_ContrastThreshold, _RelativeThreshold * l.highest);
	return l.contrast < threshold;
}

float DeterminePixelBlendFactor(LuminanceData l) 
{
	float filter = 2 * (l.n + l.e + l.s + l.w);
	filter += l.ne + l.nw + l.se + l.sw;
	filter *= 1.0 / 12;
	filter = abs(filter - l.m);
	filter = saturate(filter / l.contrast);

	float blendFactor = smoothstep(0, 1, filter);

	return blendFactor * blendFactor * _SubpixelBlending;;
}

struct EdgeData 
{
	bool isHorizontal;
	float pixelStep;
	float oppositeLuminance;
	float gradient;
};

EdgeData DetermineEdge(LuminanceData l) 
{
	EdgeData e;
	float horizontal =
		abs(l.n + l.s - 2 * l.m) * 2 +
		abs(l.ne + l.se - 2 * l.e) +
		abs(l.nw + l.sw - 2 * l.w);
	float vertical =
		abs(l.e + l.w - 2 * l.m) * 2 +
		abs(l.ne + l.nw - 2 * l.n) +
		abs(l.se + l.sw - 2 * l.s);
	e.isHorizontal = horizontal >= vertical;

	float pLuminance = e.isHorizontal ? l.n : l.e;
	float nLuminance = e.isHorizontal ? l.s : l.w;
	float pGradient = abs(pLuminance - l.m);
	float nGradient = abs(nLuminance - l.m);

	float2 pix = float2(1, 1) / float2(width, height);
	e.pixelStep = e.isHorizontal ? pix.y : pix.x;

	if (pGradient < nGradient) 
		e.pixelStep = -e.pixelStep;

	if (pGradient < nGradient) 
	{
		e.pixelStep = -e.pixelStep;
		e.oppositeLuminance = nLuminance;
		e.gradient = nGradient;
	}
	else 
	{
		e.oppositeLuminance = pLuminance;
		e.gradient = pGradient;
	}
	
	return e;
}

float DetermineEdgeBlendFactor(LuminanceData l, EdgeData e, float2 uv) 
{
	float2 uvEdge = uv;
	float2 edgeStep;

	float2 pix = float2(1, 1) / float2(width, height);

	if (e.isHorizontal) 
	{
		uvEdge.y += e.pixelStep * 0.5;
		edgeStep = float2(pix.x, 0);
	}
	else 
	{
		uvEdge.x += e.pixelStep * 0.5;
		edgeStep = float2(0, pix.y);
	}

	float edgeLuminance = (l.m + e.oppositeLuminance) * 0.5;
	float gradientThreshold = e.gradient * 0.25;

	float2 puv = uvEdge + edgeStep * edgeSteps[0];
	float pLuminanceDelta = SampleLuminance(puv) - edgeLuminance;
	bool pAtEnd = abs(pLuminanceDelta) >= gradientThreshold;


	[unroll]
	for (int i = 1; i < EDGE_STEP_COUNT && !pAtEnd; i++)
	{
		puv += edgeStep * edgeSteps[i];
		pLuminanceDelta = SampleLuminance(puv) - edgeLuminance;
		pAtEnd = abs(pLuminanceDelta) >= gradientThreshold;
	}

	if (!pAtEnd) 
	{
		puv += edgeStep * EDGE_GUESS;
	}

	float2 nuv = uvEdge - edgeStep * edgeSteps[0];
	float nLuminanceDelta = SampleLuminance(nuv) - edgeLuminance;
	bool nAtEnd = abs(nLuminanceDelta) >= gradientThreshold;

	[unroll]
	for (int i = 1; i < EDGE_STEP_COUNT && !nAtEnd; i++) {
		nuv -= edgeStep * edgeSteps[i];
		nLuminanceDelta = SampleLuminance(nuv) - edgeLuminance;
		nAtEnd = abs(nLuminanceDelta) >= gradientThreshold;
	}

	if (!nAtEnd) 
	{
		nuv -= edgeStep * EDGE_GUESS;
	}

	float pDistance;
	float nDistance;

	if (e.isHorizontal)
	{
		pDistance = puv.x - uv.x;
		nDistance = uv.x - nuv.x;
	}
	else
	{
		pDistance = puv.y - uv.y;
		nDistance = uv.y - nuv.y;
	}

	float shortestDistance;
	bool deltaSign;

	if (pDistance <= nDistance) 
	{
		shortestDistance = pDistance;
		deltaSign = pLuminanceDelta >= 0;
	}
	else 
	{
		shortestDistance = nDistance;
		deltaSign = nLuminanceDelta >= 0;
	}

	if (deltaSign == (l.m - edgeLuminance >= 0)) 
	{
		return 0;
	}

	return 0.5 - shortestDistance / (pDistance + nDistance);    
}

float4 BasicLuminancePS(VertexShaderOutput input) : COLOR0
{    float2 uv = input.TexCoord;	    if (uv.x >= VLine || !RenderHalfScreen)
    {        float4 sample = Sample(uv);
        sample.rgb = sample.g;
	
        return sample;    }	    return 0;}float4 LinearRgbToLuminancePS(VertexShaderOutput input) : COLOR0
{    float2 uv = input.TexCoord;	    if (uv.x >= VLine || !RenderHalfScreen)
    {        float4 sample = SampleLuminance(uv);
	
        return sample;    }    return 0;}float4 ContrastPS(VertexShaderOutput input) : COLOR0
{    float2 uv = input.TexCoord;	    if (uv.x >= VLine || !RenderHalfScreen)
    {        LuminanceData l = SampleLuminanceNeighborhood(uv);
	
        return l.contrast;    }    return 0;}float4 ContrastSkipLowPS(VertexShaderOutput input) : COLOR0
{    float2 uv = input.TexCoord;    if (uv.x >= VLine || !RenderHalfScreen)
    {        LuminanceData l = SampleLuminanceNeighborhood(uv);
	
        float4 retCol = l.contrast;
        if (l.contrast < _ContrastThreshold)
        {
            retCol.r = 1;
        }
	    
        return retCol;    }    return 0;}float4 ContrastSkipHighPS(VertexShaderOutput input) : COLOR0
{    float2 uv = input.TexCoord;	    if (uv.x >= VLine || !RenderHalfScreen)
    {        LuminanceData l = SampleLuminanceNeighborhood(uv);
	
        float4 retCol = l.contrast;
    
        if (l.contrast < _RelativeThreshold * l.highest)
        {
            retCol.g = 1;
        }
    
        return retCol;    }    return 0;}float4 ContrastSkipBothPS(VertexShaderOutput input) : COLOR0
{    float2 uv = input.TexCoord;	    if (uv.x >= VLine || !RenderHalfScreen)
    {        LuminanceData l = SampleLuminanceNeighborhood(uv);
	
        float4 retCol = l.contrast;
        if (l.contrast < _ContrastThreshold)
        {
            retCol.r = 1;
        }
	
        if (l.contrast < _RelativeThreshold * l.highest)
        {
            retCol.g = 1;
        }
    
        return retCol;    }    return 0;}float4 ContrastSkipPS(VertexShaderOutput input) : COLOR0
{    float2 uv = input.TexCoord;	    if (uv.x >= VLine || !RenderHalfScreen)
    {        LuminanceData l = SampleLuminanceNeighborhood(uv);
	
        if (ShouldSkipPixel(l))
        {
            return 0;
        }
    
        return l.contrast;    }    return 0;}float4 BlendFactorPS(VertexShaderOutput input) : COLOR0
{    float2 uv = input.TexCoord;	    if (uv.x >= VLine || !RenderHalfScreen)
    {        LuminanceData l = SampleLuminanceNeighborhood(uv);
	
        if (ShouldSkipPixel(l))
        {
            return 0;
        }
	
        float pixelBlend = DeterminePixelBlendFactor(l);
        return pixelBlend;    }    return 0;}float4 BlendFactorHorizontalPS(VertexShaderOutput input) : COLOR0
{    float2 uv = input.TexCoord;	    if (uv.x >= VLine || !RenderHalfScreen)
    {        LuminanceData l = SampleLuminanceNeighborhood(uv);
	
        if (ShouldSkipPixel(l))
        {
            return 0;
        }
	
        float pixelBlend = DeterminePixelBlendFactor(l);
        EdgeData e = DetermineEdge(l);
        return e.isHorizontal ? float4(1, 0, 0, 0) : 1;    }    return 0;}float4 BlendingPS(VertexShaderOutput input) : COLOR0
{    float2 uv = input.TexCoord;	    if (uv.x >= VLine || !RenderHalfScreen)
    {        LuminanceData l = SampleLuminanceNeighborhood(uv);
	
        if (ShouldSkipPixel(l))
        {
            return 0;
        }
	
        float pixelBlend = DeterminePixelBlendFactor(l);
        EdgeData e = DetermineEdge(l);	        if (e.isHorizontal)
        {
            uv.y += e.pixelStep * pixelBlend;
        }
        else
        {
            uv.x += e.pixelStep * pixelBlend;
        }
        return float4(Sample(uv).rgb, l.m);    }    return 0;}float4 EdgeLumincencePS(VertexShaderOutput input) : COLOR0
{    float2 uv = input.TexCoord;    float2 pix = float2(1, 1) / float2(width, height);	    if (uv.x >= VLine || !RenderHalfScreen)
    {            LuminanceData l = SampleLuminanceNeighborhood(uv);
	
        if (ShouldSkipPixel(l))
        {
            return Sample(uv);
        }
	
        float pixelBlend = DeterminePixelBlendFactor(l);
        EdgeData e = DetermineEdge(l);

        return DetermineEdgeBlendFactor(l, e, uv) - pixelBlend;    }     	return 0;
}float4 FxaaPS(VertexShaderOutput input) : COLOR0
{
    float2 uv = input.TexCoord;
	
    if (uv.x >= VLine || !RenderHalfScreen)
    {
        LuminanceData l = SampleLuminanceNeighborhood(uv);

        if (ShouldSkipPixel(l)) 
            return Sample(uv);

        float pixelBlend = DeterminePixelBlendFactor(l);

        EdgeData e = DetermineEdge(l);        float edgeBlend = DetermineEdgeBlendFactor(l, e, uv);
        float finalBlend = max(pixelBlend, edgeBlend);        if (e.isHorizontal) 
            uv.y += e.pixelStep * finalBlend;
        else
            uv.x += e.pixelStep * finalBlend;
        return float4(Sample(uv).rgb, l.m);    }	    return 0;}float4 halfPS(VertexShaderOutput input) : COLOR0
{
    if (!RenderHalfScreen)
        return 0;
	
	float2 uv = input.TexCoord;
    float2 pix = float2(1, 1) / float2(width, height);
	
    if (uv.x >= VLine - pix.x && uv.x <= VLine + pix.x)        return float4(0, 0, 0, 1);
    else if (uv.x <= VLine)
        return Sample(uv);
	else
        return 0;

}
technique BasicLuminance
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL BasicLuminancePS();
    }
    pass P1
    {
        AlphaBlendEnable = true;
        BlendOp = Subtract;
        PixelShader = compile PS_SHADERMODEL halfPS();
    }
}

technique LinearRgbLuminance
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL LinearRgbToLuminancePS();
    }
    pass P1
    {
        AlphaBlendEnable = true;
        BlendOp = Subtract;
        PixelShader = compile PS_SHADERMODEL halfPS();
    }
}

technique Contrast
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL ContrastPS();
    }
    pass P1
    {
        AlphaBlendEnable = true;
        BlendOp = Subtract;
        PixelShader = compile PS_SHADERMODEL halfPS();
    }
}

technique ContrastSkipLow
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL ContrastSkipLowPS();
    }
    pass P1
    {
        AlphaBlendEnable = true;
        BlendOp = Subtract;
        PixelShader = compile PS_SHADERMODEL halfPS();
    }
}

technique ContrastSkipHigh
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL ContrastSkipHighPS();
    }
    pass P1
    {
        AlphaBlendEnable = true;
        BlendOp = Subtract;
        PixelShader = compile PS_SHADERMODEL halfPS();
    }
}

technique ContrastSkipBoth
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL ContrastSkipBothPS();
    }
    pass P1
    {
        AlphaBlendEnable = true;
        BlendOp = Subtract;
        PixelShader = compile PS_SHADERMODEL halfPS();
    }
}

technique ContrastSkip
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL ContrastSkipPS();
    }
    pass P1
    {
        AlphaBlendEnable = true;
        BlendOp = Subtract;
        PixelShader = compile PS_SHADERMODEL halfPS();
    }
}

technique BlendFactor
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL BlendFactorPS();
    }
    pass P1
    {
        AlphaBlendEnable = true;
        BlendOp = Subtract;
        PixelShader = compile PS_SHADERMODEL halfPS();
    }
}

technique BlendFactorHorizontal
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL BlendFactorHorizontalPS();
    }
    pass P1
    {
        AlphaBlendEnable = true;
        BlendOp = Subtract;
        PixelShader = compile PS_SHADERMODEL halfPS();
    }
}

technique Blending
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL BlendingPS();
    }
    pass P1
    {
        AlphaBlendEnable = true;
        BlendOp = Subtract;
        PixelShader = compile PS_SHADERMODEL halfPS();
    }
}

technique EdgeLumincense
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL EdgeLumincencePS();
    }
    pass P1
    {
        AlphaBlendEnable = true;
        BlendOp = Subtract;
        PixelShader = compile PS_SHADERMODEL halfPS();
    }
}

technique FXAA
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL FxaaPS();
	}
    pass P1
    {
        AlphaBlendEnable = true;
        BlendOp = Subtract;
        PixelShader = compile PS_SHADERMODEL halfPS();
    }
}