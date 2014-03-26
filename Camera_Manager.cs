using UnityEngine;
using System.Collections;

public class Camera_Manager : MonoBehaviour {
	
	public static Camera_Manager Instance;
	public Transform TargetLookAt;
	
	public float Dist = 5f;
	public float MinDist = .25f;
	public float MaxDist = 3f;

	public float MouseSpeed = 5f;

	public float MinY = -40f;
	public float MaxY = 80f;

	private float mouseX = 0f;
	private float mouseY = 0f;
	private float defaultDist = 0f;

	private Vector3 position = Vector3.zero;
	
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

	// Rotates the camera based on the users inputs
	void VerifyUserMouseInput()
	{
		//Check if the right mouse button is depressed (not required but useful for easy debugging)
		if (!Input.GetMouseButton (1)) {
			mouseX += Input.GetAxis("Mouse X") * MouseSpeed;
			mouseY -= Input.GetAxis("Mouse Y") * MouseSpeed;
		}

		// Clamp/limit mouseY, store the result in the appropriate variable
		mouseY = ClampAngle (mouseY, MinY, MaxY);

		Vector3 direction = new Vector3 (0, 0, -Dist);
		Quaternion rotation = Quaternion.Euler (mouseY, mouseX, 0);	
		position = TargetLookAt.position + rotation * direction;
		
		// Update Position
		transform.position = position;
		transform.LookAt (TargetLookAt);
	}

	public void InitialCameraPosition()
	{
		// Set the default value for both mouse axis
		mouseX = 0;
		mouseY = 10f;
		
		// Set the validated initial camera position
		Dist = defaultDist;
	}

	public static void InitialCameraCheck() {
		Camera_Manager cameraManager;		
		GameObject mainCamera;
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
		mainCamera.AddComponent("Camera_Manager");
		cameraManager = mainCamera.GetComponent("Camera_Manager") as Camera_Manager;
		
		// Look for a targetLookAt, create one if it doesn't exist
		targetLookAt = GameObject.Find("targetLookAt") as GameObject;
		if (!targetLookAt) {
			targetLookAt = new GameObject("targetLookAt");
			targetLookAt.transform.position = Vector3.zero;
		}
		
		// Save the target look at value
		cameraManager.TargetLookAt = targetLookAt.transform;
	}
	
	private float ClampAngle (float angle, float min, float max) {
    	if (angle < -360)
        	angle += 360;
	    if (angle > 360)
	        angle -= 360;
	    return Mathf.Clamp (angle, min, max);
	}

}
