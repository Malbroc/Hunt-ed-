using UnityEngine;
using System.Collections;


enum PSphereMod {
	Normal,
	Wide,
	Short
}

public class DetectionSphereFleeLow : MonoBehaviour {

	// Object linked to detector
	public GameObject _Prey; // Object to inform -- Prey

	// Property : current detection sphere modifications
	private PSphereMod _currentSM;
	private PSphereMod CurrentPSphereMod {
		get { return _currentSM; }
		set { _currentSM = value; }
	}

	private Vector3 _originalScale;

	// Use this for initialization
	void Start () {
		CurrentPSphereMod = PSphereMod.Normal;
		_originalScale = transform.localScale;
	}


	// Update is called once per frame
	void Update () {
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
			if (CurrentPSphereMod != PSphereMod.Short) {
				// Shorten detection ranges
				transform.localScale -= new Vector3 (30.0f, 30.0f, 30.0f);
				CurrentPSphereMod = PSphereMod.Short;
			}
		} else { 
			// If player is sprinting
			if (Input.GetKey (KeyCode.LeftShift)){
				if (CurrentPSphereMod != PSphereMod.Wide) {
					// Widen detection ranges
					transform.localScale += new Vector3 (30.0f, 30.0f, 30.0f);
					CurrentPSphereMod = PSphereMod.Wide;
				}
			} else { 
				// Else, reset to initial scale
				if (CurrentPSphereMod != PSphereMod.Normal) {
					transform.localScale = _originalScale;
					CurrentPSphereMod = PSphereMod.Normal;
				}
			}
		}
	}

	/**
	* Method : OnTriggerEnter
	* Param : Collider intruder -- object entering the detection sphere
	* Desc : send the order to set the tracking lvl to LOW to the Prey
	* Return : Void
	* Author : Grosshenny Guillaume
	**/
	void OnTriggerEnter (Collider intruder) 
	{
		if (intruder.tag == "Player") 
		{
			_Prey.SendMessage ("SetLvlLow");
		}
	}


	/**
	* Method : OnTriggerExit
	* Param : Collider intruder -- object exiting the detection sphere
	* Desc : send the order to set the tracking lvl to NONE to the Prey
	* Return : Void
	* Author : Grosshenny Guillaume
	**/
	void OnTriggerExit(Collider intruder)
	{
		if (intruder.tag == "Player") 
		{
			_Prey.SendMessage ("SetLvlNone");
		}
	}
}
