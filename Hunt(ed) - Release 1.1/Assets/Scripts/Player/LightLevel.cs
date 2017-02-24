using UnityEngine;
using System.Collections;

/**
 * Source code for light
 * AIM : Global control of the intensity of the player's light
 * Method Update(): Each frame, determine the light intensity according to the player position
 * Methode PlaceTrap() : Each frame, recieve a message from the player informing about his position, then set the _isCrouching boolean in consequence
 * Author : WASMER Audric
 **/
public class LightLevel : MonoBehaviour {

	private const float STAND_INTENSITY = 2.6f;
	private const float CROUCH_INTENSITY = 0.6f;

	public GameObject _light;
	public GameObject _player;
	public bool _isCrouching = false;

	// Update is called once per frame
	void Update () {
		if (_isCrouching == false) {
			if(_light.GetComponent<Light> ().intensity < 2.6f)
				_light.GetComponent<Light> ().intensity += 0.2f;
		}
		if (_isCrouching == true) {
			if(_light.GetComponent<Light> ().intensity > 0.6f)
			_light.GetComponent<Light> ().intensity -= 0.2f;
		}
	}

	/**
	 * Method: PlayerPosition()
	 * Param: Bool
	 * Desc: Each frame, recieve a message from the player informing about his position, then set the _isCrouching boolean in consequence
	 * Return: None
	 * Author: WASMER Audric
	 **/
	public void PlayerPosition(bool isCrouching){
		_isCrouching = isCrouching;
	}
}
