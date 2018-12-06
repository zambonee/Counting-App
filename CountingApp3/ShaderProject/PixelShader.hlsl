sampler2D input : register(S0);
float brightness : register(C0);
float contrast : register(C1);

float4 main(float2 uv : TEXCOORD) : COLOR
{
	float4 color = tex2D(input, uv);
	color.rgb /= color.a;
	float newContrast = contrast;
	if (newContrast > 0)
	{
		newContrast = newContrast * 10;
	}
	color.rgb = ((color.rgb - 0.5f) * max(newContrast + 1, 0)) + 0.5f;
	color.rgb += brightness;
	color.rgb *= color.a;
	return color;
}