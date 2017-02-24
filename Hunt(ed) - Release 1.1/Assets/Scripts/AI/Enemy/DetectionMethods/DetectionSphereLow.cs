/**
 * Source code for Enemy AI : Detection sphere - Low lvl
 * AIM : Set the low lvl tracking mod for AI when player enter in the detection sphere 
 * 		 and set None lvl tracking for AI when player exit the detection sphere
 * method OnDetectionSphereEnter : Set low lvl
 **/

using UnityEngine;
using System.Collections;

enum SphereMod 
{
	Normal,
	Wide,
	Short
}

public class DetectionSphereLow : MonoBehaviour {



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
		if (Input.GetKey (KeyCode.LeftControl)){
			if (CurrentSphereMod != SphereMod.Short) {
				// Shorten detection ranges
				transform.localScale -= new Vector3 (30.0f, 30.0f, 30.0f);
				CurrentSphereMod = SphereMod.Short;
			}
		} else { 
			// If player is sprinting
			if (Input.GetKey (KeyCode.LeftShift)){
				if (CurrentSphereMod != SphereMod.Wide) {
					// Widen detection ranges
					transform.localScale += new Vector3 (30.0f, 30.0f, 30.0f);
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
			_Enemy.SendMessage ("SetLvlLow");
			Debug.Log ("niveau d'alerte low");
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
			_Enemy.SendMessage ("SetLvlNone");
			Debug.Log ("fin niveau d'alerte Low, début de NONE");
		}
	}
}
