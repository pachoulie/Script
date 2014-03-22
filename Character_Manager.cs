using UnityEngine;
using System.Collections;

// Class description
// Process Player input
// Convert keypresses to a 3D Vector
// Perform a Constant  Camera Checlk


public class Character_Manager : MonoBehaviour {
	
	public static Character_Manager 	Instance;
	public float						DeadZone;
	public CharacterController			CharacterControllerComponent;
	
	
	void Awake()
	{
		Instance = this;
	}
	
	void Update()
	{
		//Constantly check for a camera

		//Call ControllerInput()
		ControllerInput();

		//Call ActionInput()
		ActionInput();

		//Call the ControlledUpdate() method of Character_Motor via the Singleton (created later…)
		Character_Motor.Instance.ControlledUpdate();
	}
	
	// Updates the MoveVector in Character_Motor
	void ControllerInput() {
		//Zeros out the Instance variable MoveVector except the y Axis
		Character_Motor.Instance.MoveVector.x = 0;
		Character_Motor.Instance.MoveVector.z = 0;

		//Check if the vertical axis is outside the deadZone
		//Add this motion to the MoveVector
		if (Input.GetAxis("Vertical") > DeadZone || Input.GetAxis("Vertical") < -DeadZone)
			Character_Motor.Instance.MoveVector.z = Input.GetAxis("Vertical");

		//Check if the horizontal axis is outside the deadZone
		//Add this motion to the MoveVector
		if (Input.GetAxis("Horizontal") > DeadZone || Input.GetAxis("Horizontal") < -DeadZone)
			Character_Motor.Instance.MoveVector.x = Input.GetAxis("Horizontal");
	}
	
	void ActionInput() {
		// if jump button pressed
		if (Input.GetButton("Jump")) {
			DelegateJump();
		}
	}
	
	void DelegateJump() {
		// Call Character_Motor jump
		if (!Character_Motor.Instance.IsJumping)
			Character_Motor.Instance.Jump();
	}

}
