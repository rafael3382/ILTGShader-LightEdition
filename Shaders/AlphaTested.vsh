#ifdef HLSL

float2 u_origin;
float4x4 u_viewProjectionMatrix;
float3 u_viewPosition;
float2 u_fogStartInvLength;
float u_fogYMultiplier;

void main(
	in float3 a_position: POSITION,
	in float4 a_color: COLOR,
	in float2 a_texcoord: TEXCOORD,
	out float4 v_color : COLOR,
	out float2 v_texcoord : TEXCOORD,
	out float v_fog : FOG,
	out float4 sv_position: SV_POSITION
)
{
	// Texture
	v_texcoord = a_texcoord;

	// Vertex color
	v_color = a_color;
	
	// Fog
	float3 fogDelta = u_viewPosition - a_position;
	fogDelta.y *= u_fogYMultiplier;
	v_fog = saturate((length(fogDelta) - u_fogStartInvLength.x) * u_fogStartInvLength.y);
	
	// Position
	sv_position = mul(float4(a_position.x - u_origin.x, a_position.y, a_position.z - u_origin.y, 1.0), u_viewProjectionMatrix);
}

#endif
#ifdef GLSL

// <Semantic Name='POSITION' Attribute='a_position' />
// <Semantic Name='COLOR' Attribute='a_color' />
// <Semantic Name='TEXCOORD' Attribute='a_texcoord' />

uniform vec2 u_origin;
uniform mat4 u_viewProjectionMatrix;
uniform vec3 u_viewPosition;
uniform vec2 u_fogStartInvLength;
uniform float u_fogYMultiplier;

attribute vec3 a_position;
attribute vec4 a_color;
attribute vec2 a_texcoord;

varying vec4 v_color;
varying vec2 v_texcoord;
varying float v_fog;

void main()
{
	// Texture
	v_texcoord = a_texcoord;

	// Vertex color
	v_color = a_color;
	
	// Fog
	vec3 fogDelta = u_viewPosition - a_position;
	fogDelta.y *= u_fogYMultiplier;
	v_fog = clamp((length(fogDelta) - u_fogStartInvLength.x) * u_fogStartInvLength.y, 0.0, 1.0);
	
	// Position
	gl_Position = u_viewProjectionMatrix * vec4(a_position.x - u_origin.x, a_position.y, a_position.z - u_origin.y, 1.0);

	// Fix gl_Position
	OPENGL_POSITION_FIX;
}

#endif