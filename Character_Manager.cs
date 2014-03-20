using UnityEngine;
using System.Collections;

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
}
