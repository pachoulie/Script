using UnityEngine;
using System.Collections;

public static class Helper {
	public static float CameraClamp(float angle, float min, float max) {
		while (angle < -360) {
			angle += 360;
		}
		if (angle > 360) {
			angle %= 360;
		}
		return Mathf.Clamp(angle, min, max);
	}	
}
