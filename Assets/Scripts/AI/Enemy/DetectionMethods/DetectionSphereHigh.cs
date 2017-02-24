/**
 * Source code for Enemy AI : Detection sphere - Low lvl
 * AIM : Set the high lvl tracking mod for AI when player enter in the detection sphere 
 * 		 and set low lvl tracking for AI when player exit the detection sphere
 * method OnDetectionSphereEnter : Send order to set High lvl to enemy object
 * method OnDetectionSphereExit : Send order to set Low lvl to enemy Object
 **/

using UnityEngine;
using System.Collections;

public class DetectionSphereHigh : MonoBehaviour {

	// Object linked to detector
	public GameObject _Enemy; // Object to inform -- enemy of player

	// Sphere properties
	private SphereMod _currentSM;
	private SphereMod CurrentSphereMod 
	{
		get { return _currentSM; }
		set { _currentSM = value; }
	}

	private Vector3 _originalScale;


	void Start () 
	{
		CurrentSphereMod = SphereMod.Normal;
		_originalScale = transform.localScale;
	}


	void Update () 
	{
		AdaptToPlayerPosture ();
	}

	/**
	* Method : AdaptToPlayerPosture
	* Param :  void
	* Desc : Increase or decrease the detection sphere according to player posture
	* Return : Void
	* Author : Martin Genet
	**/
	private void AdaptToPlayerPosture(){
		// If player is sneaking
		if (Input.GetKey (KeyCode.LeftControl)) {
			if (CurrentSphereMod != SphereMod.Short) {
				// Shorten detection ranges
				transform.localScale -= new Vector3 (5.0f, 5.0f, 5.0f);
				CurrentSphereMod = SphereMod.Short;
			}
		} else {
			// If player is sprinting
			if (Input.GetKey (KeyCode.LeftShift)) {
				if (CurrentSphereMod != SphereMod.Wide) {
					// Widen detection ranges
					transform.localScale += new Vector3 (10.0f, 10.0f, 10.0f);
					CurrentSphereMod = SphereMod.Wide;
				}
			} else { 
				// Else, reset to initial scale
				if (CurrentSphereMod != SphereMod.Normal) {
					transform.localScale = _originalScale;
					CurrentSphereMod = SphereMod.Normal;
				}
			}
		}
	}


	/**
	* Method : OnTriggerEnter
	* Param : Collider intruder -- object entering the detection sphere
	* Desc : send the order to set the tracking lvl to LOW to the Enemy
	* Return : Void
	* Author : Grosshenny Guillaume
	**/
	void OnTriggerEnter (Collider intruder) 
	{
		if (intruder.tag == "Player") 
		{
			_Enemy.SendMessage ("SetLvlHigh");
			Debug.Log ("Alerte niveau HIGH");
		}
	}


	/**
	* Method : OnTriggerExit
	* Param : Collider intruder -- object exiting the detection sphere
	* Desc : send the order to set the tracking lvl to NONE to the Enemy
	* Return : Void
	* Author : Grosshenny Guillaume
	**/
	void OnTriggerExit(Collider intruder)
	{
		if (intruder.tag == "Player") 
		{
			_Enemy.SendMessage ("SetLvlLow");
			Debug.Log ("fin High, début Low");
		}
	}
}
