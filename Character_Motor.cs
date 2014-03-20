using UnityEngine;
using System.Collections;

public class Character_Motor : MonoBehaviour {

	public static	Character_Motor Instance;
	public Vector3	MoveVector;
	public float	SpeedLimit;

	void Awake()
	{
		//Store an Instance of itself
		Instance = this;
	}

	public void ControlledUpdate()
	{
		//Call AlignCharacterToCameraDirection()
		ProcessMotion();
	}

	public void ProcessMotion()
	{
		//Convert Vector to World Space
		var WorldPosition = transform.TransformDirection(MoveVector);

		float magnitude = WorldPosition.sqrMagnitude;

		//Normalize Vector
		if (magnitude > 1)
			WorldPosition.Normalize();

		//Multiply magnifier
		WorldPosition = WorldPosition * SpeedLimit;

		//Convert to unit/second
		WorldPosition *= Time.deltaTime;

		//Move character

	}

	public void AlignCharacterToCameraDirection()
	{
		//Replace the characters Y rotation with the cameras Y rotation
	}
}
