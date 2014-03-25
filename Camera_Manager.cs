using UnityEngine;
using System.Collections;

public class Camera_Manager : MonoBehaviour {
	
	public static Camera_Manager Instance;
	public Transform TargetLookAt;
	
	void Awake()
	{
		//Store an Instance of itself
		Instance = this;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
}
