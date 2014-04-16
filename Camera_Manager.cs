using UnityEngine;
using System.Collections;

public class Camera_Manager : MonoBehaviour {
	
	public static Camera_Manager Instance;
	public Transform TargetLookAt = null;	
	
	// DistanceVariables
	public float verifiedCameraDistance = 6f;
	public float MinDist = 0.5f;
	public float MaxDist = 10f;
	public float cameraDistanceBeforeObstruction = 0f;

	// Speed for X, Y and Scroll of the Mouse
	public float MouseSpeed = 4f;
	
	// Y limitation of the Camera
	public float MinY = -10f;
	public float MaxY = 40;
	
	// Mouse and Scroll variables
	private float mouseX;
	private float mouseY;
	private float wheelSensitivity = 4f;
	private float defaultDist;

	private float desiredDistance = 0f;
	private Vector3 verifiedCameraPosition = Vector3.zero;
	private Vector3 currentCameraDistance = Vector3.zero;
	
	// This are the velocity of the smooth as reference
	private float velocityX = 0f;
	private float velocityY = 0f;
	private float velocityZ = 0f;
	private float velocityDistance = 0f;
	
	// This is the resolution of the smooth
	public float smoothTimeSwitch = 0.1f;
	public float ObstructedSmoothTime = 1;
	public float UnobstructedSmoothTime = 0.1f;
	public float MouseSmoothTime = 0.1f;

	void Awake() {
		Instance = this;
	}
	
	void Start() {
		// Validate camera position  using Mathf.Clamp
		verifiedCameraDistance = Mathf.Clamp (verifiedCameraDistance, MinDist, MaxDist);
		
		// Save the validated camera position
		defaultDist = verifiedCameraDistance;

		InitialCameraPosition ();
	}

	void LateUpdate() {
		VerifyUserMouseInput ();

		SmoothCameraAxis ();

		int count = 0;
		do {
			//Within this loop we will call our SmoothCameraPosition() method 
			SmoothCameraPosition();
			count++;
		} while (ObstructedCameraCheck(count));



		ApplyCameraPosition();
	}
	
	// Rotates the camera based on the users inputs
	void VerifyUserMouseInput()
	{
		//Check if the right mouse button is depressed (not required but useful for easy debugging)
		if (Input.GetMouseButton (1)) {
			mouseX += Input.GetAxis("Mouse X") * MouseSpeed;
			mouseY -= Input.GetAxis("Mouse Y") * MouseSpeed;
		}

		//Check the mouse scrollwheel for the camera (de)zoom
		if (Input.GetAxis ("Mouse ScrollWheel") != 0.0f){
			desiredDistance = Mathf.Clamp(verifiedCameraDistance - (wheelSensitivity * Input.GetAxis("Mouse ScrollWheel")), MinDist, MaxDist);
			//Assign our cameraDistanceBeforeObstruction to our verifiedUserCameraDistance (clamped value after user scroll wheel input)
			cameraDistanceBeforeObstruction = desiredDistance;

			//We also need to assign our smoothTimeSwitch to our UnobstructedSmoothTime
			smoothTimeSwitch = UnobstructedSmoothTime;
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
		verifiedCameraDistance = defaultDist;
		
		// Sets the default parameters of the cameras position
		desiredDistance = verifiedCameraDistance;

		//Assign our cameraDistanceBeforeObstruction to our verifiedCameraDistance (initial clamped value) to give an initial value
		cameraDistanceBeforeObstruction = verifiedCameraDistance;
	}

	
	void SmoothCameraAxis()
	{
		// Apply smoothing to each axis
		var positionX = Mathf.SmoothDamp (currentCameraDistance.x, verifiedCameraPosition.x, ref velocityX, smoothTimeSwitch );
		var positionY = Mathf.SmoothDamp (currentCameraDistance.y, verifiedCameraPosition.y, ref velocityY, smoothTimeSwitch );
		var positionZ = Mathf.SmoothDamp (currentCameraDistance.z, verifiedCameraPosition.z, ref velocityZ, smoothTimeSwitch );

		// Store smoothed axis as Vector3
		currentCameraDistance = new Vector3 (positionX, positionY, positionZ);
	}
	
	void ApplyCameraPosition() {
		// Assign the Camera Position as the smoothed Vector3 in the previous method
		transform.position = currentCameraDistance;
		
		// Make the camera look at the target
		transform.LookAt (TargetLookAt);
	}

	void SmoothCameraPosition()
	{
		EvaluateCameraDistanceBeforeObstruction();
		// Apply smoothing to the position 
		verifiedCameraDistance = Mathf.SmoothDamp (verifiedCameraDistance, desiredDistance, ref velocityDistance, smoothTimeSwitch );

		// Call CreatePositionVector() using the mouse inputs and smoothed position & Create a Vector3 to hold the result 
		verifiedCameraPosition = CreatePositionVector (mouseY, mouseX, verifiedCameraDistance);
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

	//Takes in our TargetLookAtTransform.position which is what we are looking at through our camera and smoothedCameraPosition
	float CameraCollisionPointsCheck(Vector3 targetLookAtPosition, Vector3 cameraPositionAfterSmoothing) {
		//Creates a cameraBackBuffer using the smoothedCameraPosition
		Vector3 cameraBackBuffer = cameraPositionAfterSmoothing + transform.forward * -camera.nearClipPlane;

		//Calls our previous Helper function and stores our clip plane points as a struct

		Helper.ClipPlaneStruct clipPlanePoints = Helper.FindNearClipPlanePositions(camera.transform.position);

		//Uses this information to draw out a Debug.DrawLine
		//Draws a line (red) to the cameraBackBuffer
		Debug.DrawLine(targetLookAtPosition, cameraBackBuffer, Color.red);
		//Draws the rectangle (white) of the Near Clip Plane
		Debug.DrawLine(targetLookAtPosition, clipPlanePoints.UpperLeft);
		Debug.DrawLine(targetLookAtPosition, clipPlanePoints.LowerLeft);
		Debug.DrawLine(targetLookAtPosition, clipPlanePoints.UpperRight);
		Debug.DrawLine(targetLookAtPosition, clipPlanePoints.LowerRight);
		//Draws the pyramid (white) connecting the targetLookAtPosition to the Near Clip Plane
		Debug.DrawLine(clipPlanePoints.UpperLeft, clipPlanePoints.UpperRight);
		Debug.DrawLine(clipPlanePoints.UpperRight, clipPlanePoints.LowerRight);
		Debug.DrawLine(clipPlanePoints.LowerRight, clipPlanePoints.LowerLeft);
		Debug.DrawLine(clipPlanePoints.LowerLeft, clipPlanePoints.UpperLeft);

		//Creates a variable called closestDistanceToCharacter
		float closestDistanceToCharacter = -1f;
		bool isObstructed = false;
		//Use if statements to cycle through each of our points
		//Make sure to ignore if your linecast collides with the player (by checking for “player” tags from the linecastInfo  collision in your code 
		//You will need to tag all the children of the character to “Player” in the Unity Editor in order for your code to work
		var hitInfo = new RaycastHit();

		foreach (var field in typeof(Helper.ClipPlaneStruct).GetFields(System.Reflection.BindingFlags.Instance |
		                                                               System.Reflection.BindingFlags.Public))
		{
			Vector3 planeCornerPosition = (Vector3)field.GetValue(clipPlanePoints);
			//Use Physics.Linecast to cast from our targetLookAtPosition to our clip plane points (available from our helper method we just created)
			if (Physics.Linecast(targetLookAtPosition, clipPlanePoints.UpperLeft, out hitInfo) && hitInfo.collider.tag != "Player"){
					//If our linecast collides with something, set the linecastInfo distance to the closestDistanceToCharacter variable
					// check if any of the other points are less than the closestDistanceToCharacter. If this is true then we set that as our shortest distance
					closestDistanceToCharacter = hitInfo.distance;
				}
			}
		
		if (Physics.Linecast(targetLookAtPosition, cameraPositionAfterSmoothing + transform.forward * -camera.nearClipPlane, out hitInfo) && hitInfo.collider.tag != "Player")
			if (hitInfo.distance < closestDistanceToCharacter || closestDistanceToCharacter == -1)
				closestDistanceToCharacter = hitInfo.distance;

		//Returns the value of closestDistanceToCharacter or a float of -1 if we have not collided with anything
		return closestDistanceToCharacter;
	}

	bool ObstructedCameraCheck (int obstructedCheckCount) {
		//Create a variable called cameraObstructionBool to check if our camera is obstructed
		bool cameraObstructionBool = false;
		//Checks if the camera is obstructed by calling our CameraCollisionPointsCheck()
		//Store the result as closestDistanceToCharacter
		float closestDistanceToCharacter = CameraCollisionPointsCheck(TargetLookAt.position, verifiedCameraPosition);

		//If obstructed
		if (closestDistanceToCharacter != -1)
		{
			if (obstructedCheckCount > 10){
				//If we have passed our limit then we need to move our Dist directly to our closestDistanceToCharacter minus our cameras back buffer
				verifiedCameraDistance = closestDistanceToCharacter - Camera.main.nearClipPlane;
				//Set our desiredDistance (our clamped camera position value) to our Dist
				desiredDistance = verifiedCameraDistance;
			}
			//Attempt to move the camera CurrentCameraDistance forward
			else
			{
				cameraObstructionBool = true;
				//Move the Dist forward by a set value
				verifiedCameraDistance -= 0.5f;
				if (verifiedCameraDistance < MinDist)
					verifiedCameraDistance = MinDist;
			}
			//Set our smoothTimeSwitch to ObstructedSmoothTime
			smoothTimeSwitch = ObstructedSmoothTime;
		}
		return cameraObstructionBool;
	}

	void EvaluateCameraDistanceBeforeObstruction() {
		//Check if our verifiedUserCameraDistance is less than our cameraDistanceBeforeObstruction. If it is then the camera has been adjusted because it was obstructed meaning we need to save the original position
		//This change of verifiedUserCameraDistance occurs within ObstructedCameraCheck()
		if (verifiedCameraDistance < cameraDistanceBeforeObstruction) {
			//We need to store our camera’s position before the adjustment so using our method CreatePositionVector() we can send it our mouse X and Y and also our cameraDistanceBeforeObstruction 
			Vector3 cameraPositionBeforeObstruction = CreatePositionVector(mouseY, mouseX, cameraDistanceBeforeObstruction);
			//Once we have our cameraPositionBeforeObstruction we can use our method CameraCollisionPointsCheck() which returns the closestDistanceToCharacter from this position
			//We will store the returned value as cameraPositionBeforeObstruction
			float closestDistanceToCharacter = CameraCollisionPointsCheck(TargetLookAt.position, cameraPositionBeforeObstruction);

			//If this closestDistanceToCharacter is equal to -1 then it is not being obstructed, therefore we can move our camera back to this point
			//To do this we can set the verifiedUserCameraDistance to the cameraDistanceBeforeObstruction
			if (closestDistanceToCharacter == -1) {
				verifiedCameraDistance = cameraDistanceBeforeObstruction;
			}
		}
	}
}