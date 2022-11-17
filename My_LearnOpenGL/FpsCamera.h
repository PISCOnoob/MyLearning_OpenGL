#ifndef FPSCAMERA_H
#define FPSCAMERA_H

// this Fps Camera use EulerAngle not Quaternion
#include <glad/glad.h>
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>

#include <vector>

// Define several possible options for camera movement. Used as 
// abstraction to stay away from window system specific input methods
enum FpsCamera_Movement
{
	FORWARD,
	BACKWARD,
	LEFT,
	RIGHT
};

// Defalt camera values
const float default_yaw = -90.0f;
const float default_pitch = 0.0f;
const float default_speed = 2.5f;
const float default_sensitivity = 0.1f;
const float default_zoom = 45.0f;


// An abastract camera class that processes input and calculates the
// corresponding Euler Angles, Vectors and Matrices for use in OpenGL
class FpsCamera
{
public:
	typedef glm::vec3 vec3;
	typedef glm::mat4 mat4;

	// FpsCamera Attribute
	glm::vec3 m_position;
	glm::vec3 m_front;
	glm::vec3 m_up;
	glm::vec3 m_right;
	glm::vec3 m_worldUp;	
	// euler angles
	float m_yaw;
	float m_pitch;
	// camera opations
	float m_moveSpeed;
	float m_mouseSensitivity;
	float m_zoom;

	// ctor with vectors
	FpsCamera(vec3 position = vec3(0.0f, 0.0f, 0.0f), vec3 up = vec3(0.0f, 1.0f, 0.0f), float yaw = default_yaw, float pitch = default_pitch)
		:m_front(vec3(0.0f, 0.0f, -1.0f)),
		m_moveSpeed(default_speed),
		m_mouseSensitivity(default_sensitivity),
		m_zoom(default_zoom)
	{
		m_position = position;
		m_worldUp = up;
		m_yaw = yaw;
		m_pitch = pitch;
		UpdateCameraVectors();
	}

	FpsCamera(float posX, float posY, float posZ, float upX, float upY, float upZ, float yaw = default_yaw, float pitch = default_pitch)
		:m_front(vec3(0.0f, 0.0f, -1.0f)),
		m_moveSpeed(default_speed),
		m_mouseSensitivity(default_sensitivity),
		m_zoom(default_zoom)
	{
		m_position = vec3(posX, posY, posZ);
		m_worldUp = vec3(upX, upY, upZ);
		m_yaw = yaw;
		m_pitch = pitch;
		UpdateCameraVectors();
	}

	// returns the view matrix calculated using Euler Angles and LookAt Matrix
	mat4 GetViewMatrix()
	{
		return glm::lookAt(m_position, m_position + m_front, m_up);
	}

	// processes input received from any keyboard-like input system.
	// Accepts input parameter in the form of camera defined ENUM
	// (to abstract it from windowing systems)
	void ProcessKeyboard(FpsCamera_Movement direction, float deltaTime)
	{
		float velocity = m_moveSpeed * deltaTime;
		if (direction == FORWARD)
		{
			m_position += m_front * velocity;
		}
		if (direction == BACKWARD)
		{
			m_position -= m_front * velocity;
		}
		if (direction == LEFT)
		{
			m_position -= m_right * velocity;
		}
		if (direction == RIGHT)
		{
			m_position += m_right * velocity;
		}
	}

	// processes input received from a mouse input system. Expects
	// the offset value in both the x and y direction
	void ProcessMouseMovement(float offsetX, float offsetY, GLboolean constrainPitch = true)
	{
		offsetX *= m_mouseSensitivity;
		offsetY *= m_mouseSensitivity;

		m_yaw += offsetX;
		m_pitch += offsetY;

		// make sure that when pitch is out of the bounds
		// screen doesn't get flipped
		if (constrainPitch)
		{
			if (m_pitch > 89.0f)
				m_pitch = 89.0f;
			if (m_pitch < -89.0f)
				m_pitch = -89.0f;
		}

		UpdateCameraVectors();
	}

	// processes input received from a mouse scroll-wheel event.
	// only requires input on the vertical wheel-axis
	void ProcessMouseScroll(float offsetY)
	{
		m_zoom -= (float)offsetY;
		if (m_zoom < 1.0f)
			m_zoom = 1.0f;
		if(m_zoom > 45.0f)
			m_zoom = 45.0f;
	}
	

private:
	// calculate the front vector from the Camera's (updated) Euler Angles
	void UpdateCameraVectors()
	{
		// calculate the new front vector
		vec3 front;
		front.x = cos(glm::radians(m_yaw)) * cos(glm::radians(m_pitch));
		front.y = sin(glm::radians(m_pitch));
		front.z = sin(glm::radians(m_yaw)) * cos(glm::radians(m_pitch));
		m_front = glm::normalize(front);

		m_right = glm::normalize(glm::cross(m_front, m_worldUp));
		m_up = glm::normalize(glm::cross(m_right, m_front));
	}
};



#endif // !FPSCAMERA_H


