using UnityEngine;
using System.Collections;

// Class description
// Process Player input
// Convert keypresses to a 3D Vector
// Perform a Constant  Camera Checlk


public class Character_Manager : MonoBehaviour {
	
	public static Character_Manager 	Instance;
	public CharacterController			CharacterControllerComponent;
	
	
	void Awake()
	{
		Instance = this;
	}
	
	void Update()
	{
		//Constantly check for a camera
		//Call ControllerInput()
		//Call the ControlledUpdate() method of Character_Motor via the Singleton (created later…)
		Character_Motor.Instance.ControlledUpdate();
	}
	
	// Updates the MoveVector in Character_Motor
	void ControllerInput() {
		// Calls processMotion();
		
	}
	
	void ActionInput() {
		// if jump button pressed
		// call delegatejump 
	}
	
	void DelegateJump() {
		// Call Character_Motor jump
	}
}
