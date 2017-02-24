using UnityEngine;
using System.Collections;

public class HitBoxTrigger : MonoBehaviour {

	// Object linked to detector
	public GameObject _Player; // Object to inform -- enemy of player

	/**
	* Method : OnTriggerEnter
	* Param : Collider intruder -- object entering the detection sphere
	* Desc : send the order to set the alive Bool of the player to false -- end the game
	* Return : Void
	* Author : Grosshenny Guillaume
	**/
	void OnTriggerEnter (Collider intruder) 
	{
		if (intruder.tag == "Player") 
		{
			_Player.SendMessage ("SetDead");
		}
	}
}
