using UnityEngine;
using System.Collections;

public static class Helper {

	public struct ClipPlaneStruct {
		public Vector3 UpperLeft;
		public Vector3 UpperRight;
		public Vector3 LowerLeft;
		public Vector3 LowerRight;
	}

	public static float CameraClamp(float angle, float min, float max) {
		while (angle < -360) {
			angle += 360;
		}
		if (angle > 360) {
			angle %= 360;
		}
		return Mathf.Clamp(angle, min, max);
	}	

	//Returns a value of type ClipPlaneStruct
	//Finds each corner point of the Near Clip Plane using ScreenToWorldPoint and assign each to the struct
	public static ClipPlaneStruct FindNearClipPlanePositions(Vector3 camPos) {

		ClipPlaneStruct clipPlanePoints = new ClipPlaneStruct();

		clipPlanePoints.LowerRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0, Camera.main.camera.nearClipPlane));
		clipPlanePoints.LowerLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.camera.nearClipPlane));
		clipPlanePoints.UpperRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, Camera.main.camera.nearClipPlane));
		clipPlanePoints.UpperLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight, Camera.main.camera.nearClipPlane));

		return clipPlanePoints;
	}
}
