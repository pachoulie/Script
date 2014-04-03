using UnityEngine;
using System.Collections;

public class Animation_Manager : MonoBehaviour {

	public static Animation_Manager	Instance;

	//List the Motions
	public enum MotionStateList{Stationary = 0, 
		Forward = 1,
		Backward = 2,
		Left = 4,
		Right = 8,
		LeftForward = 5,
		RightForward = 9,
		LeftBackward = 6,
		RightBackward = 10};

	public MotionStateList	CharacterMotionState;

	void Awake()
	{
		//Store an Instance of itself
		Instance = this;
	}

	public void CurrentMotionState() {
		//Initialize basic movement check boxes to false
		int direction = 0;
		//Check if which direction your moveVector is moving and set that check box to true
		Vector3 MoveVector = Character_Motor.Instance.MoveVector;

		if (MoveVector.z > 0)
			direction+= MotionStateList.Forward;
		if (MoveVector.z < 0)
			direction+=MotionStateList.Backward;
		if (MoveVector.x > 0)
			direction+=MotionStateList.Right;
		if (MoveVector.x < 0)
			direction+=MotionStateList.Left;

		//Check for combination directions. If the direction is forward and left/right then set the CharacterMotionState
		//Else set the CharacterMotionState to forward
		//Do the same for backwards
		//If you are not moving set the CharacterMotionState to MotionStateList.Stationary
		CharacterMotionState = (MotionStateList)direction;

		//Print your result to the console (for testing)
		Debug.Log(CharacterMotionState.ToString());
	}
}
