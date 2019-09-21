/************* TWEAKABLES **************/

float4x4 worldIT : WorldInverseTranspose;
float4x4 wvp : WorldViewProjection;
float4x4 world : World;
float4x4 viewI : ViewInverse;

float4 tint = float4(1,1,1,1);

float reflectStrength 
< 
	string Object = "ReflectiveStrength";
	string UIName = "ReflectiveStrength";
	string Space = "World";	
> = 1.0;

float refractStrength 
< 
	string Object = "RefractStrength";
	string UIName = "RefractStrength";
	string Space = "World";	
> = 1.0;

float3 etas 
 < 
	string Object = "ETAS";
	string UIName = "ETAS";
	string Space = "World";	
> = { 0.80, 0.82, 0.84 };

texture cubeMap;

texture fresnelTex;

samplerCUBE environmentMapSampler = sampler_state
{
	Texture = <cubeMap>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
};

sampler2D fresnelSampler = sampler_state
{
	Texture = <fresnelTex>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = None;
};

/************* DATA STRUCTS **************/

/* data from application vertex buffer */
struct appdata {
    float4 Position	: POSITION;
    //float4 UV		: TEXCOORD0;
    float3 Normal	: NORMAL;
};

/* data passed from vertex shader to pixel shader */
struct vertexOutput {
    float4 HPosition	: POSITION;
    //float4 TexCoord		: TEXCOORD0;
    float3 WorldNormal	: TEXCOORD1;
    float3 WorldView	: TEXCOORD2;
};

/*********** vertex shader ******/

vertexOutput mainVS(appdata IN)
{
    vertexOutput OUT;
    float3 normal = normalize(IN.Normal);
    OUT.WorldNormal = mul(normal, (float3x3) worldIT);
    float3 Pw = mul(IN.Position, world).xyz;
    //OUT.TexCoord = IN.UV;
    OUT.WorldView = viewI[3].xyz - Pw;
    OUT.HPosition = mul(IN.Position, wvp);
    return OUT;
}

/********* pixel shader ********/

// modified refraction function that returns boolean for total internal reflection
float3 refract2( float3 I, float3 N, float eta, out bool fail )
{
	float IdotN = dot(I, N);
	float k = 1 - eta*eta*(1 - IdotN*IdotN);
	fail = k < 0;
	return eta*I - (eta*IdotN + sqrt(k))*N;
}

// approximate Fresnel function
float fresnel(float NdotV, float bias, float power)
{
   return bias + (1.0-bias)*pow(1.0 - max(NdotV, 0), power);
}

// function to generate a texture encoding the Fresnel function
float4 generateFresnelTex(float NdotV : POSITION) : COLOR
{
	return fresnel(NdotV, 0.2, 4.0);
}

float4 mainPS(vertexOutput IN) : COLOR
{
    half3 N = normalize(IN.WorldNormal);
    float3 V = normalize(IN.WorldView);
    
 	// reflection
    half3 R = reflect(-V, N);
    half4 reflColor = texCUBE(environmentMapSampler, R);

	half fresnel = tex2D(fresnelSampler, dot(N, V));

	// wavelength colors
	const half4 colors[3] = {
    	{ 1, 0, 0, 0 },
    	{ 0, 1, 0, 0 },
    	{ 0, 0, 1, 0 },
	};
        
	// transmission
 	half4 transColor = 0;
  	bool fail = false;
    for(int i=0; i<3; i++) 
    {
    	half3 T = refract2(-V, N, etas[i], fail);
    	transColor += texCUBE(environmentMapSampler, T) * colors[i];
	}

    return lerp(transColor*refractStrength, reflColor*reflectStrength, fresnel) * tint;
}

/*************/

technique dx9 
{
	pass p0   
	{		
		VertexShader = compile vs_4_0 mainVS();
		PixelShader = compile ps_4_0 mainPS();
	}
}

/***************************** eof ***/
