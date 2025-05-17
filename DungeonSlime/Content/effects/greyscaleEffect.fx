#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
// Value between 0 and 1 that controls the intensity of the
// grayscale effect.
float Saturation = 1.0;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	// Sample the texture
	float4 color = text2D(SpriteTextureSampler, input.TextureCoordinates) * input.color;

	// Calculate the grayscale value based on human perception of colors
	float grayscale = dot(color.rgb, float3(0.3, 0.59, 0.11));

	// create a grayscale color vector 
	float3 grayscaleColor = float3(grayscale, grayscale, grayscale);

	// linear interpolation between the grayscale color and the original
	// color's rgb values.
	float3 finalColor = lerp(grayscale, color.rgb, Saturation);

	// return the final color with alpha value
	return tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};