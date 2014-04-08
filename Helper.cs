﻿using UnityEngine;
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

	public static ClipPlaneStruct FindNearClipPlanePositions(Vector3 pos) {
		//Returns a value of type ClipPlaneStruct
		//Finds each corner point of the Near Clip Plane using ScreenToWorldPoint and assign each to the struct
		ClipPlaneStruct clipPlanePoints = new ClipPlaneStruct();
		
		if(Camera.main == null)
		{return clipPlanePoints;}
		
		Transform transform = Camera.main.transform;
		float halfFOV = (Camera.main.fieldOfView/2)*Mathf.Deg2Rad;
		float aspect = Camera.main.aspect;
		float distance = Camera.main.nearClipPlane;
		float height = distance *Mathf.Tan(halfFOV);
		float width = height * aspect;
		
		clipPlanePoints.LowerRight = pos + transform.right *width;
		clipPlanePoints.LowerRight -= transform.up*height;
		clipPlanePoints.LowerRight += transform.forward * distance;
		
		clipPlanePoints.LowerLeft = pos - transform.right *width;
		clipPlanePoints.LowerLeft -= transform.up*height;
		clipPlanePoints.LowerLeft += transform.forward * distance;
		
		clipPlanePoints.UpperRight = pos + transform.right *width;
		clipPlanePoints.UpperRight += transform.up*height;
		clipPlanePoints.UpperRight += transform.forward * distance;
		
		clipPlanePoints.UpperLeft = pos - transform.right *width;
		clipPlanePoints.UpperLeft += transform.up*height;
		clipPlanePoints.UpperLeft += transform.forward * distance;
		
		return clipPlanePoints;
	}
}
