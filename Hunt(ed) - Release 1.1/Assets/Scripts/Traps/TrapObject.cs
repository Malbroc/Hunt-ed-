using UnityEngine;
using System.Collections;
using UnityEngine.UI;


/**
 * Source code for the traps
 * AIM : Interactions with the traps placed by the player
 * Method Update(): Call the method tapActivation() at each frame
 * Methode PlaceTrap() : Activate the trap if something walks on it
 * Author : WASMER Audric
 **/
public class TrapObject : MonoBehaviour {

	RaycastHit hit;
	private GameObject _Target;
	public GameObject _trapInstance;
	public AudioSource _trigger;

	private float _timer;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
		if (TimerFinished() && TrapActivation ()) {
			_trigger.Play ();
			LaunchTimer (3.0f);
		}
	}


	/** ----------------------------------------------------------------------------------------------------
	 * Method : LaunchTimer
	 * Param : float value of the new timer duration
	 * Desc : Initialise _roamingTimer to the new timer duration
	 * Return : void
	 * Author : Martin Genet
	 **/
	private void LaunchTimer(float value){
		_timer = value;
	}

	/** ----------------------------------------------------------------------------------------------------
	 * Method : TimerFinished
	 * Param : void
	 * Desc : Decrease the timer by deltaTime and check if it reached 0
	 * Return : bool
	 * Author : Martin Genet
	 **/
	private bool TimerFinished(){
		_timer -= Time.deltaTime;
		return (_timer <= 0.0f);
	}

	
	/**
	 * Method: TrapActivation()
	 * Param: None
	 * Desc: Activate the trap if someone walks on it, and send a message to the dumb one who just got his leg ripped off.
	 * Return: true if activated, else false
	 * Author: WASMER Audric
	 **/
	public bool TrapActivation (){
		
		/*Creating a vertical ray in the center of the trap*/
		Vector3 trapPosition = new Vector3 (_trapInstance.transform.position.x, transform.position.y, transform.position.z);

		/*If an object cross the ray*/
		if (Physics.Raycast (trapPosition, Vector3.up, out hit, 0.2f)){
				
			/*then send a message to the method GetTrapped() of this object*/
			string tagHit = hit.transform.tag;

			_Target = GameObject.FindGameObjectWithTag (tagHit);
			if (_Target != null) {
				_Target.SendMessage ("GetTrapped", null);
			}
			/*A trap is used only once.*/
			DestroyObject (_trapInstance, 3.0f);

			return true;
		}

		return false;
	}
}

