using UnityEngine;
using System.Collections;

public class Camera_Manager : MonoBehaviour {
	
	public static Camera_Manager Instance;
	public Transform TargetLookAt = null;
	
	public float Dist = 6f;
	public float MinDist = .25f;
	public float MaxDist = 10f;

	public float MouseSpeed = 4f;

	public float MinY = -10f;
	public float MaxY = 40;

	private float mouseX = 0f;
	private float mouseY = 0f;
	private float defaultDist;

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
		// Call VerifyUserMouseInput ()
		VerifyUserMouseInput ();
	}

	Vector3 CreatePositionVector(float mouseX, float mouseY, Vector3 position)
	{
		//Create a new Vector3 to hold the given position
		Vector3 distance = new Vector3 (0, 0, -Dist);
		//Create a new Quaternion to hold the rotation data given
		Quaternion rotation = Quaternion.Euler (mouseY, mouseX, 0);
		//Return the character position plus the vector we have just created (rotation * distance)
		return (position + rotation * distance);
	}

	void VerifiyUserMouseInput()
	{
		mouseY = Helper.CameraClamp(mouseY, MinY, MaxY);
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
		VerifiyUserMouseInput();

		// Update Position
		transform.position = CreatePositionVector(mouseX, mouseY, TargetLookAt.transform.position);
		transform.LookAt(TargetLookAt);
	}
	
	public void InitialCameraPosition()
	{
		// Set the default value for both mouse axis
		mouseX = 0;
		mouseY = 10f;
		
		// Set the validated initial camera position
		Dist = defaultDist;
	}

	public void SmoothCameraPosition() {

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
