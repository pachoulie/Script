using UnityEngine;
using System.Collections;

// Class description
// Convert the 33D Vector to world Space
// Update the Character Rotation with Camera Rotation

public class Character_Motor : MonoBehaviour {
	
	public static	Character_Motor Instance;
	public Vector3	MoveVector;
	public Vector3	SlideVector;
	public float	TerminalVelocity = 50f;
	public float	JumpVelocity = 80f;
	public float	Speed = 10f;
	public float	Gravity = 9.8f;
	public float	SlideLimit = 0.9f;
	public float 	MaxMagnitudeSlide = 0.4f;
	private bool	Jumping = false;
	public bool		InvertedModel = true;

	public bool	IsJumping { 
		get { return Jumping; } 
		set { Jumping = value; } 
	}
	
	void Awake()
	{
		//Store an Instance of itself
		Instance = this;
	}
	
	public void ControlledUpdate()
	{
		AlignCharacterToCameraDirection();
		ProcessMotion();
	}
	
	
	public void ProcessMotion()
{
		// Save MoveVector.z and reapply as VerticalVelocity
		ApplyGravity();

		//Convert Vector to World Space
		Vector3 WorldPosition;

		// If model is inverted then invert Z and Y get world position and revert Z and Y
		if (InvertedModel) {
			MoveVector = MoveVector.InvertYZ ();
			WorldPosition = transform.TransformDirection (MoveVector);
			MoveVector = MoveVector.RevertYZ ();
		} else {
			WorldPosition = transform.TransformDirection (MoveVector);
		}

		float savedY = WorldPosition.y;
		WorldPosition.y = 0;

		float magnitude = WorldPosition.sqrMagnitude;

		//Normalize Vector
		if (magnitude > 1)
			WorldPosition.Normalize();

		WorldPosition = Slide (WorldPosition);

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
		if (MoveVector.x != 0 || MoveVector.z != 0) {
		//Replace the characters Y rotation with the cameras Y rotation
		Transform cameraRotation = Camera_Manager.Instance.transform;
		transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
		                                      cameraRotation.eulerAngles.y,
		                                      transform.eulerAngles.z);
		}
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

	public Vector3 Slide(Vector3 WorldPosition){
		//First check if the character is on the ground if not return;
		if (!Character_Manager.Instance.CharacterControllerComponent.isGrounded)
			return WorldPosition;

		//Zero out the slideVector
		SlideVector = Vector3.zero;

		RaycastHit hitInfo;

		//Move our raycast position up one unit in Y and cast it down
		if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hitInfo)) {

			//Check if the information from the normal.y is less than the our slide limit
			if (hitInfo.normal.y < SlideLimit) {
				//If it is, add our slideVector to our moveVector
				//Get the raycastInfo for each normal and store it as the slideVector
				SlideVector = new Vector3(hitInfo.normal.x, -hitInfo.normal.y, hitInfo.normal.z);

			}
		//If the magnitude of our slideVector (slide speed) is too great the we should lose the controls of our character
			if (SlideVector.magnitude < MaxMagnitudeSlide) {
				WorldPosition += SlideVector;
			} else {
				//If it is, set our moveVector to our slideVector
				WorldPosition = SlideVector;
			}
		}
		return WorldPosition;
	}
}
