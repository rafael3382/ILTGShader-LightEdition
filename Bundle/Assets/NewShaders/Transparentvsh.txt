#ifdef HLSL

#endif
#ifdef GLSL
#define TRANSPARENT
// this code was generated, the code that the developer codes is much cleaner than what you're seeing here

#ifdef GL_ES
#ifdef GL_FRAGMENT_PRECISION_HIGH
precision highp float;
#else
precision mediump float;
#endif
#endif

// <Semantic Name='POSITION' Attribute='a_position' />
// <Semantic Name='COLOR' Attribute='a_color' />
// <Semantic Name='TEXCOORD' Attribute='a_texcoord' />

uniform vec2 u_origin;
uniform mat4 u_viewProjectionMatrix;
uniform vec3 u_viewPosition;
uniform vec2 u_fogStartInvLength;
uniform float u_fogYMultiplier;
uniform float u_time;
uniform float u_waterDepth;
attribute vec3 a_position;
attribute vec4 a_color;
attribute vec2 a_texcoord;
varying vec4 v_pure_color;
varying vec4 v_color;
varying vec2 v_texcoord;
varying float v_fog;
varying float wavelevel;
varying vec3 v_pos;
varying vec3 v_playerPos;
varying float v_incidence;


varying vec3 normal;
void main()
{
	// Texture
	v_texcoord = a_texcoord;
    float AVar = u_time * u_waterDepth; // Just to remove the error of u_time is not used
	// Vertex color
	vec3 direction = u_viewPosition - a_position;
	float l = length(direction);
	float incidence = abs(direction.y / l);
	v_incidence = incidence;
	float topAlpha = 0.5; //clamp(mix(1.0, 0.0, incidence), 0.0, 1.0);
	float sideAlpha = 0.7;
	float alpha = mix(topAlpha, sideAlpha, a_color.w);		// Alpha component of a_color encodes whether this is top (0) or side (1) vertex
	v_color = vec4(a_color.xyz * alpha, a_color.w);
	v_pure_color = a_color;

    
    v_playerPos = u_viewPosition;
	// Fog
	vec3 fogDelta = u_viewPosition - a_position;
	fogDelta.y *= u_fogYMultiplier;
	v_fog = clamp((length(fogDelta) - u_fogStartInvLength.x) * u_fogStartInvLength.y, 0.0, 1.0);
	
	// Position
    #define Mult 3000.0
	vec4 vtx = vec4(a_position.x - u_origin.x, a_position.y, a_position.z - u_origin.y, 1.0);
	
	/*
    vtx.z -= 4.0;
    vtx.xz += fract(u_viewPosition.xz);
    vtx.xz -= vec2(1.0)-fract(u_viewPosition.xz);
    */
	
	
	
	#define PI 3.1415926538
    float elevation = 0.05 * sin(2.0 * PI * (u_time*800.0 + a_position.x /  2.5 + a_position.z / 5.0))
				   + 0.05 * sin(2.0 * PI * (u_time*600.0 + a_position.x / 6.0 + a_position.z /  12.0));
    
    //(sin(2.0 * a_position.x + u_time*Mult ) * (cos(1.5 * a_position.z + u_time*Mult*1.5) * 0.2)*sin(a_position.z+(u_time*Mult)))*0.9;
    vtx.y -= elevation;
    wavelevel = elevation;
    v_pos = vtx.xyz + vec3(u_origin.x, 0.0, u_origin.y);
	gl_Position = u_viewProjectionMatrix * vtx;
	
// Fix gl_Position
	OPENGL_POSITION_FIX;
}

#endif