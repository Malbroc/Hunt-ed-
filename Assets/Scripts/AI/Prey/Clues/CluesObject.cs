using UnityEngine;
using System.Collections;

/**
 * Source code for the clue objects
 * AIM : Animate the clues to make them more visible & control their hitbox
 * Method Update(): Each frame, animate the clue and its light ball
 * Method OnTriggerEnter(): when the player enter in the clue's area, send him a message to slow him down.
 * Author : WASMER Audric
 **/
public class CluesObject : MonoBehaviour {

	public GameObject _Clue;					// Auto-referencing to the clue
	public GameObject _LightBallArea;			// The lightball gravitating around the clue
	private GameObject _Player;					// The player

	private const float MAX_RANGE = 0.25f;		// Maximum range of clue's light
	private const float MIN_RANGE = 0.1f;		// Minimum range of clue's light
	private bool _RangeGettingLarger = true;	// Determine if the range of the clue's light is increasing or decreasing
	private float angle;						// Angle value used for the lightball movement
	private float speed = (2*Mathf.PI)/2.5f;	// Speed value used for the lightball movement
	private float radius = 3.14f;				// Radius value used for the lightball movement


	void Start () {
		// The clue lasts for 60.
		DestroyObject (_Clue, 60.0f);
	}
		
	void Update () {

		/* Updating the lightball position to make it draw a circle around the clue */
		angle += speed*Time.deltaTime;
		_LightBallArea.transform.position = new Vector3(Mathf.Cos (angle) * radius + _Clue.transform.position.x,
			_LightBallArea.transform.position.y,
			Mathf.Sin (angle) * radius + _Clue.transform.position.z);

		/* Updating clue's light range */
		if (_RangeGettingLarger) {
			if (_Clue.GetComponent<Light> ().range < MAX_RANGE)
				_Clue.GetComponent<Light> ().range += (1.0f * Time.deltaTime)/10;
			else
				_RangeGettingLarger = false;
		} else {
			if (_Clue.GetComponent<Light> ().range > MIN_RANGE)
				_Clue.GetComponent<Light> ().range -= (1.0f * Time.deltaTime)/10;
			else
				_RangeGettingLarger = true;
		}

	}

	/**
	 * Method: OnTriggerEnter()
	 * Param: None
	 * Desc: when the player enter in the clue's area, send him a message to slow him down.
	 * Return: None
	 * Author: WASMER Audric
	 **/
	void OnTriggerEnter (Collider intruder) {
		
		if (intruder.tag == "Player") 
		{
			_Player = GameObject.FindGameObjectWithTag ("Player");
			_Player.SendMessage ("SlowDownClueArea");
		}
	}


}
