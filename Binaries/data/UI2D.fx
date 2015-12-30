float4x4 ViewProjection: register( b0 );

Texture2D Texture : register(t0);

SamplerState ClampSampler : register(s0);
SamplerState LinearSampler : register(s1);

//---------------------------------------------------------------------------

struct VS_IN {
	float4 Position : POSITION;
	float4 Color    : COLOR;
};

struct PS_IN {
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD;
	float4 Color    : COLOR;
};

//---------------------------------------------------------------------------

PS_IN VS( VS_IN input ) {
	PS_IN output = (PS_IN)0;

	output.Position = mul(float4(input.Position.xy, 0.0f, 1.0f),ViewProjection);
	output.TexCoord = input.Position.zw;
	output.Color = input.Color;

	return output;
}

float4 PS( PS_IN input ) : SV_Target {
	return Texture.Sample(LinearSampler, input.TexCoord) * input.Color;
}
