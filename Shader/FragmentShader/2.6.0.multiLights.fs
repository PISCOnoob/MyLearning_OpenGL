 #version 330 core

struct Material
{
	sampler2D diffuseMap;
	sampler2D specularMap;
	float shininess;
};

struct DirLight
{
	//vec3 position;
	vec3 direction;
	
	vec3 ambientCol;
	vec3 diffuseCol;
	vec3 specularCol;
};

struct PointLight
{
	vec3 position;
	
	// for attenuation
	float constant;
	float linear;
	float quadratic;
	
	vec3 ambientCol;
	vec3 diffuseCol;
	vec3 specularCol;
};

struct SpotLight
{
	vec3 position;
	vec3 direction;
	
	float innerAngle;
	float outerAngle;
	
	// for attenuation
	float constant;
	float linear;
	float quadratic;
	
	vec3 ambientCol;
	vec3 diffuseCol;
	vec3 specularCol;
};

in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoords;

#define NR_POINT_LIGHTS 4

uniform vec3 viewPos;
uniform Material material;
uniform DirLight dirLight;
uniform PointLight pointLights[NR_POINT_LIGHTS];
uniform SpotLight spotLight;

out vec4 FragColor;

// declare function
vec3 CalDirLight(DirLight light, vec3 normal, vec3 viewDir);
vec3 CalPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
vec3 CalSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir);

void main()
{
	vec3 norm = normalize(Normal);
	vec3 viewDir = normalize(viewPos - FragPos);
	
	vec3 res_dirLight = CalDirLight(dirLight, norm, viewDir);
	
	vec3 res_pointLight = vec3(0.0, 0.0, 0.0);
	for(int i = 0; i<NR_POINT_LIGHTS; ++i)
	{
		res_pointLight += CalPointLight(pointLights[i], norm, FragPos, viewDir);
	}
	
	vec3 res_spotLight = CalSpotLight(spotLight, norm, FragPos, viewDir);
	
	FragColor = vec4(res_dirLight + res_pointLight + res_spotLight, 1.0); //res_dirLight + res_pointLight + res_spotLight
}


// calculates the color when using a directional light.
vec3 CalDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
	// diffuse
	vec3 lightDir = normalize(-light.direction);
	float lambert = max(dot(normal, lightDir),0.0);
	vec3 diffuse = light.diffuseCol * lambert * texture(material.diffuseMap, TexCoords).rgb;
	//vec3 diffuse =  light.diffuseCol * lambert * glm::vec3(1.0, 1.0, 1.0);
	
	// specular
	vec3 reflectDir = reflect(-lightDir, normal);
	float phong = pow(max(dot(viewDir, reflectDir),0.0), material.shininess);
	vec3 specular = light.specularCol * phong * texture(material.specularMap, TexCoords).rgb;
	
	// ambient
	vec3 ambient = light.ambientCol * texture(material.diffuseMap,TexCoords).rgb;
	
	vec3 finalCol = diffuse + specular + ambient;
	return finalCol;
}

// calculates the color when using a point light.
vec3 CalPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
	vec3 lightDir = normalize(light.position - fragPos);
	
	// attenuation
	float distance = length(light.position - fragPos);
	float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
	
	
	// diffuse
	float lambert = max(dot(normal,lightDir),0.0);
	vec3 diffuse = light.diffuseCol * lambert * texture(material.diffuseMap, TexCoords).rgb;
	
	// specular
	vec3 reflectDir = reflect(-lightDir, normal);
	float phong = pow(max(dot(viewDir, reflectDir), 0.0),material.shininess);
	vec3 specular = light.specularCol * phong * texture(material.specularMap, TexCoords).rgb;
	
	// ambient
	vec3 ambient = light.ambientCol * texture(material.diffuseMap,TexCoords).rgb;
	
	vec3 finalCol = (diffuse + specular + ambient) * attenuation;
	return finalCol;
}

// calculates the color when using a spot light.
vec3 CalSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
	vec3 lightDir = normalize(light.position - fragPos);
	
	// attenuation
	float distance = length(light.position - fragPos);
	float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
	
	// spotlight intensity(including soft edge)
	float theta = dot(lightDir, normalize(-light.direction));
	float epsilon = light.innerAngle - light.outerAngle;
	float intensity = clamp((theta - light.outerAngle) / epsilon, 0.0, 1.0);
	
	// diffuse
	float lambert = max(dot(normal,lightDir),0.0);
	vec3 diffuse = light.diffuseCol * lambert * texture(material.diffuseMap, TexCoords).rgb;
	
	// specular
	vec3 reflectDir = reflect(-lightDir, normal);
	float phong = pow(max(dot(viewDir, reflectDir), 0.0),material.shininess);
	vec3 specular = light.specularCol * phong * texture(material.specularMap, TexCoords).rgb;
	
	// ambient
	vec3 ambient = light.ambientCol * texture(material.diffuseMap,TexCoords).rgb;
	
	vec3 finalCol = (diffuse + specular + ambient) * attenuation *intensity;
	return finalCol;
}