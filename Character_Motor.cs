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
		// Save MoveVector.z and reapply as VerticalVelocity
		ApplyGravity();


		//Convert Vector to World Space
		var WorldPosition = transform.TransformDirection(MoveVector);

		float savedZ = WorldPosition.z;
		WorldPosition.z = 0;

		float magnitude = WorldPosition.sqrMagnitude;

		//Normalize Vector
		if (magnitude > 1)
			WorldPosition.Normalize();



		//Multiply magnifier
		WorldPosition = WorldPosition * Speed;

		WorldPosition.z = savedZ;

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
								MoveVector.z = 0.01f;
				} 
		else if (MoveVector.z > -TerminalVelocity)
		{
			MoveVector.z -= Gravity;
			if (MoveVector.z < -TerminalVelocity)
				MoveVector.z = -TerminalVelocity;
		}

	}
	
	public void Jump() {
		//Check if the character is grounded
		//If it is, move our character (VerticalVelocity)
		if (Character_Manager.Instance.CharacterControllerComponent.isGrounded)
		{
			MoveVector.z = JumpVelocity;
			IsJumping = true;
		}
	}
}
