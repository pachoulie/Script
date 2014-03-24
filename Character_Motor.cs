using UnityEngine;
using System.Collections;

// Class description
// Convert the 33D Vector to world Space
// Update the Character Rotation with Camera Rotation

public class Character_Motor : MonoBehaviour {
	
	public static	Character_Motor Instance;
	public Vector3	MoveVector;
	public float	TerminalVelocity = 50f;
	public float	JumpVelocity = 80f;
	public float	Speed = 10f;
	public float	Gravity = 9.8f;
	public bool		IsJumping;
	
	void Awake()
	{
		//Store an Instance of itself
		Instance = this;
		IsJumping = false;
	}
	
	public void ControlledUpdate()
	{
		//Call AlignCharacterToCameraDirection()
		ProcessMotion();
	}
	
	
	public void ProcessMotion()
{
		// Save MoveVector.y and reapply as VerticalVelocity
		ApplyGravity();


		//Convert Vector to World Space
		var WorldPosition = transform.TransformDirection(MoveVector);

		float savedY = WorldPosition.y;
		WorldPosition.y = 0;

		float magnitude = WorldPosition.sqrMagnitude;

		//Normalize Vector
		if (magnitude > 1)
			WorldPosition.Normalize();



		//Multiply magnifier
		WorldPosition = WorldPosition * Speed;

		WorldPosition.y = savedY;

		//Convert to unit/second
		WorldPosition *= Time.deltaTime;



		//Move character
		Character_Manager.Instance.CharacterControllerComponent.Move(WorldPosition);
	}
	
	public void AlignCharacterToCameraDirection()
	{
		//Replace the characters Y rotation with the cameras Y rotation
	}
	
	void ApplyGravity() {
		//Check if our Y vector is less than TerminalVelcocity
		//If yes, apply gravity 
		//Check if the character is grounded, if it is then apply a small amount of gravity
		if (Character_Manager.Instance.CharacterControllerComponent.isGrounded) {
			if (IsJumping)
				IsJumping = false;
			else
				MoveVector.y = 0.01f;
		}
		else if (MoveVector.y > -TerminalVelocity)
		{
			MoveVector.y -= Gravity;
			if (MoveVector.y < -TerminalVelocity)
				MoveVector.y = -TerminalVelocity;
		}

	}
	
	public void Jump() {
		//Check if the character is grounded
		//If it is, move our character (VerticalVelocity)
		if (Character_Manager.Instance.CharacterControllerComponent.isGrounded)
		{
			MoveVector.y = JumpVelocity;
			IsJumping = true;
		}
	}
}
