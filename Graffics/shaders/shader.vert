#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec3 Normal;
out vec3 FragPos;
out vec2 TexCoord;

void main()
{
    FragPos = vec3(vec4(aPosition, 1.0) * model * view);
    Normal = aNormal * mat3(model) * mat3(view);
    TexCoord = aTexCoord;
    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
}