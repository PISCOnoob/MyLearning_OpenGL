#version 330 core

struct Material
{
	sampler2D diffuseMap;
	sampler2D specularMap;
	float shininess;
};

struct Light
{
	//vec3 position;
	vec3 direction;
	
	vec3 ambientCol;
	vec3 diffuseCol;
	vec3 specularCol;
};

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoords;

uniform vec3 viewPos;
uniform Material material;
uniform Light light;

out vec4 FragColor;

void main()
{
	vec3 norm = normalize(Normal);
	
	// diffuse
	vec3 lightDir = normalize(-light.direction);
	float lambert = max(dot(norm,lightDir),0.0);
	vec3 diffuse = light.diffuseCol * lambert * texture(material.diffuseMap,TexCoords).rgb;
	//vec3 diffuse =  light.diffuseCol * lambert * glm::vec3(1.0,1.0,1.0);
	// specular
	vec3 viewDir = normalize(viewPos - FragPos);
	vec3 reflectDir = reflect(-lightDir, norm);
	float phong = pow(max(dot(viewDir,reflectDir),0.0),material.shininess);
	vec3 specular = light.specularCol * phong * texture(material.specularMap,TexCoords).rgb;
	
	// ambient
	vec3 ambient = light.ambientCol * texture(material.diffuseMap,TexCoords).rgb;
	
	vec3 finalCol = diffuse + specular + ambient;
	FragColor = vec4(finalCol,1.0);
}