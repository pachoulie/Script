using UnityEngine;
using System.Collections;

public class Camera_Manager : MonoBehaviour {
	
	public static Camera_Manager Instance;
	public Transform TargetLookAt = null;	
	
	// DistanceVariables
	public float Dist = 6f;
	public float MinDist = 0.25f;
	public float MaxDist = 10f;

	// Speed for X, Y and Scroll of the Mouse
	public float MouseSpeed = 4f;
	
	// Y limitation of the Camera
	public float MinY = -10f;
	public float MaxY = 40;
	
	// Mouse and Scroll variables
	private float mouseX;
	private float mouseY;
	private float defaultDist;
	
	// Need to rename those variables
	private float desiredDistance = 0f;
	private float VelocityDistance = 0f;
	private Vector3 desiredPosition = Vector3.zero;
	private float velocityX = 0f;
	private float velocityY = 0f;
	private float velocityZ = 0f;
	private Vector3 position = Vector3.zero;
	public float DistanceSmooth =0.05f;
	public float X_Smooth = 0.05f;
	public float Y_Smooth = 0.1f;
	
	void Awake() {
		Instance = this;
	}
	
	void Start() {
		// Validate camera position  using Mathf.Clamp
		Dist = Mathf.Clamp (Dist, MinDist, MaxDist);
		
		// Save the validated camera position
		defaultDist = Dist;
		
		// Call InitialCameraPosition()
		InitialCameraPosition ();
	}

	void LateUpdate() {
		VerifyUserMouseInput ();
				
		SmoothCameraPosition ();

		SmoothCameraAxis ();
		
		ApplyCameraPosition();
	}
	
	// Rotates the camera based on the users inputs
	void VerifyUserMouseInput()
	{
		//Check if the right mouse button is depressed (not required but useful for easy debugging)
		if (!Input.GetMouseButton (1)) {
			mouseX += Input.GetAxis("Mouse X") * MouseSpeed;
			mouseY -= Input.GetAxis("Mouse Y") * MouseSpeed;
		}

		// Set mouseY as the Helper.CameraClamp
		mouseY = Helper.CameraClamp(mouseY, MinY, MaxY);
	}
	
	public void InitialCameraPosition()
	{
		// Set the default value for both mouse axis
		mouseX = 0;
		mouseY = 10f;
		
		// Set the validated initial camera position
		Dist = defaultDist;
		
		// Sets the default parameters of the cameras position
		desiredDistance = Dist;
	}

	
	void SmoothCameraAxis()
	{
		// Apply smoothing to each axis
		var positionX = Mathf.SmoothDamp (position.x, desiredPosition.x, ref velocityX, X_Smooth);
		var positionY = Mathf.SmoothDamp (position.y, desiredPosition.y, ref velocityY, Y_Smooth);
		var positionZ = Mathf.SmoothDamp (position.z, desiredPosition.z, ref velocityZ, X_Smooth);
				
		// Store smoothed axis as Vector3
		position = new Vector3 (positionX, positionY, positionZ);
	}
	
	void ApplyCameraPosition() {
		// Assign the Camera Position as the smoothed Vector3 in the previous method
		transform.position = position;
		
		// Make the camera look at the target
		transform.LookAt (TargetLookAt);
	}

	void SmoothCameraPosition()
	{
		// Apply smoothing to the position 
		Dist = Mathf.SmoothDamp (Dist, desiredDistance, ref VelocityDistance, DistanceSmooth);

		// Call CreatePositionVector() using the mouse inputs and smoothed position & Create a Vector3 to hold the result 
		desiredPosition = CreatePositionVector (mouseY, mouseX, Dist);
	}

	Vector3 CreatePositionVector(float mouseX, float mouseY, float position)
	{
		// Create a new Vector3 to hold the given position
		Vector3 distance = new Vector3 (0, 0, -position);

		// Create a new Quaternion to hold the rotation data given
		Quaternion rotation = Quaternion.Euler (mouseX, mouseY, 0);

		// Return the character position plus the vector we have just created (rotation * distance)
		return TargetLookAt.position + rotation * distance;
	}
	
	public static void InitialCameraCheck() {
		GameObject mainCamera;
		Camera_Manager cameraManager;		
		GameObject targetLookAt;
		
		// If no main camera then create one
		if (Camera.mainCamera) {
			mainCamera = Camera.mainCamera.gameObject;
		} else {
			mainCamera = new GameObject("MainCamera");
			mainCamera.AddComponent("Camera");
			mainCamera.tag = "MainCamera";
		}
		
		// Attach Camera_Manager script to the MainCamera
		if (!mainCamera.GetComponent("Camera_Manager")) {
			mainCamera.AddComponent("Camera_Manager");
		}
		cameraManager = mainCamera.GetComponent("Camera_Manager") as Camera_Manager;
		
		// Look for a attached targetLookAt, if none create one if it doesn't exist
		if (!cameraManager.TargetLookAt) {
			if (!(targetLookAt = GameObject.Find("targetLookAt") as GameObject)) {
			targetLookAt = new GameObject("targetLookAt");
			targetLookAt.transform.position = Vector3.zero;
			}
			// Save the target look at value
			cameraManager.TargetLookAt = targetLookAt.transform;
		}	
	}
}
