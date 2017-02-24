using UnityEngine;
using System.Collections;

public class HitBoxP : MonoBehaviour {

	// Object linked to detector
	public GameObject _Player; // Object to inform -- Prey

	/**
	* Method : OnTriggerEnter
	* Param : Collider intruder -- object entering the detection sphere
	* Desc : send the order to set the Win Bool of the player to true -- win the game
	* Return : Void
	* Author : Grosshenny Guillaume
	**/
	void OnTriggerEnter (Collider intruder) 
	{
		if (intruder.tag == "Player") 
		{
			_Player.SendMessage ("SetWin");
		}
	}
}
