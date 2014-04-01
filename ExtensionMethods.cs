using UnityEngine;
using System.Collections;

//Extending sealed unity classes and structs
public static class ExtensionMethods
{
	//Inverting Y and Z axis in order to handle inverted 3D models
	public static Vector3 InvertYZ(this Vector3 vec)
	{
		vec = new Vector3 (vec.x, -vec.z, vec.y);
		return vec;
	}

	public static Vector3 RevertYZ(this Vector3 vec)
	{
		vec = new Vector3 (vec.x, vec.z, -vec.y);
		return vec;
	}

}