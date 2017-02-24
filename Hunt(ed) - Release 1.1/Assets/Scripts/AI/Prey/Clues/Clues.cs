using UnityEngine;
using System.Collections;

/**
 * Source code for the clues leading to the prey
 * AIM : Global control of the clues
 * Method Update(): Each 15s, the prey spawns a clue right on its position.
 * Method ClueInstantiate(): Instanciate a clue on the position of the prey.
 * Author : WASMER Audric
 **/
public class Clues : MonoBehaviour {

	private const float PERIOD = 15.0f;		//After a few seconds, the prey is on her period for a single frame. Just kidding. Default period separating clue's spawns.
	private float _remaining_time = PERIOD;	//Timer used for clue's spawning delay.

	public GameObject _prey;				//The prey
	public GameObject _clue;				//The clue's prefab
	public GameObject _clueArea;			//The little light gravitating around the clue to visually display the effect area of the clue.

	// Update is called once per frame

	// Each 20s, the prey spawns a clue right on his position.
	void Update () {
		if (Time.timeScale == 0.0f)
			return;

		// If the timer variable is equal to 0, instanciate a clue.
		if (_remaining_time <= 0) {
			ClueInstantiate ();
			_remaining_time = PERIOD;
		} 
		// If not, decrement the timer variable.
		else {
			_remaining_time = _remaining_time - 1.0f * Time.deltaTime;
		}
	}

	/**
	 * Method: ClueInstanciation()
	 * Param: None
	 * Desc: Instanciate a clue on the position of the prey.
	 * Return: None
	 * Author: WASMER Audric
	 **/
	public void ClueInstantiate(){

		/* Instanciate a clue at the prey's position */
		Instantiate (_clue, _prey.transform.position,Quaternion.identity);




	}
}
