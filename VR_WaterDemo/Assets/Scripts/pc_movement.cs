using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pc_movement : MonoBehaviour
{
	[Header("Movement")]
	public float moveSpeed = 10f;          
	public float acceleration = 5f;        
	public float deceleration = 5f;        
	public float fastMultiplier = 3f;      

	[Header("Look")]
	public float lookSensitivity = 2f;
	public float lookSmoothing = 5f;       
	public float turnSpeed = 60f;          // degrees per second for WASD turning

	[Header("Drone Tilt")]
	public float maxTiltAngle = 15f;       
	public float tiltSmooth = 5f;          

	private Vector2 smoothVelocity;
	private Vector2 currentRotation;
	private Vector3 currentVelocity;

	private bool usingMouseLook = true;
	private Quaternion targetRotation;     

	void Start()
	{
		// Initialize currentRotation from starting orientation
		Vector3 euler = transform.rotation.eulerAngles;
		currentRotation = new Vector2(euler.y, euler.x); // yaw = y, pitch = x

		EnableMouseLook(true);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			usingMouseLook = !usingMouseLook;
			EnableMouseLook(usingMouseLook);
		}

		HandleRotation();
		HandleMovement();
		ApplyTilt();
	}

	void HandleRotation()
	{
		if (usingMouseLook)
		{
			float mouseX = Input.GetAxisRaw("Mouse X");
			float mouseY = Input.GetAxisRaw("Mouse Y");

			// Smooth mouse input
			smoothVelocity.x = Mathf.Lerp(smoothVelocity.x, mouseX, 1f / lookSmoothing);
			smoothVelocity.y = Mathf.Lerp(smoothVelocity.y, mouseY, 1f / lookSmoothing);

			currentRotation.x += smoothVelocity.x * lookSensitivity;
			currentRotation.y -= smoothVelocity.y * lookSensitivity;

			// --- Extra: edge turning when pressing A/D ---
			Vector3 mousePos = Input.mousePosition;
			float screenWidth = Screen.width;
			float edgeThreshold = 0.02f; // 2% of screen from edge

			if (Input.GetKey(KeyCode.A) && mousePos.x <= screenWidth * edgeThreshold)
				currentRotation.x -= turnSpeed * Time.deltaTime;

			if (Input.GetKey(KeyCode.D) && mousePos.x >= screenWidth * (1f - edgeThreshold))
				currentRotation.x += turnSpeed * Time.deltaTime;
		}
		else
		{
			float yaw = 0f;
			float pitch = 0f;

			if (Input.GetKey(KeyCode.LeftArrow)) yaw = -1f;
			if (Input.GetKey(KeyCode.RightArrow)) yaw = 1f;
			if (Input.GetKey(KeyCode.DownArrow)) pitch = 1f;
			if (Input.GetKey(KeyCode.UpArrow)) pitch = -1f;

			currentRotation.x += yaw * turnSpeed * Time.deltaTime;
			currentRotation.y += pitch * turnSpeed * Time.deltaTime;
		}

		currentRotation.y = Mathf.Clamp(currentRotation.y, -89f, 89f);
	}

	void EnableMouseLook(bool enable)
	{
		if (enable)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	void HandleMovement()
	{
		Vector3 input = new Vector3(
			(Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0),
			(Input.GetKey(KeyCode.E) ? 1 : 0) - (Input.GetKey(KeyCode.Q) ? 1 : 0),
			(Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0)
		).normalized;

		float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? fastMultiplier : 1f);

		Quaternion baseRotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0f);
		Vector3 targetVelocity = baseRotation * input * speed;

		currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity,
			(targetVelocity.magnitude > currentVelocity.magnitude ? acceleration : deceleration) * Time.deltaTime);

		transform.position += currentVelocity * Time.deltaTime;
	}

	void ApplyTilt()
	{
		Quaternion lookRot = Quaternion.Euler(currentRotation.y, currentRotation.x, 0f);

		Vector3 localVel = transform.InverseTransformDirection(currentVelocity);

		float roll = Mathf.Clamp(-localVel.x / moveSpeed, -1f, 1f) * maxTiltAngle;
		float pitch = Mathf.Clamp(localVel.z / moveSpeed, -1f, 1f) * maxTiltAngle;

		Quaternion tiltRot = Quaternion.Euler(pitch, 0f, roll);

		targetRotation = lookRot * tiltRot;

		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, tiltSmooth * Time.deltaTime);
	}
}