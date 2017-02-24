/**
 * Source code for Player sound trigger : SoundEnemyNearby 
 * AIM : Play a random scarying sound (Brrrrr) when enemy enter the trigger !
 * method OnDetectionSphereEnter : Send order to play sound
 * method OnDetectionSphereExit : Send order to stop the sound
 **/

using UnityEngine;
using System.Collections;

public class SoundEnemyNearby : MonoBehaviour {

	public GameObject _Player;

	/**
	* Method : OnTriggerEnter
	* Param : Collider intruder -- object entering the detection sphere
	* Desc : send the order to set the tracking lvl to LOW to the Enemy
	* Return : Void
	* Author : Grosshenny Guillaume
	**/
	void OnTriggerEnter (Collider intruder) 
	{
		if (intruder.tag == "enemy1" || intruder.tag == "enemy2" || intruder.tag == "enemy3" || intruder.tag == "enemy4" || intruder.tag == "enemy5" || intruder.tag == "enemy6" || intruder.tag == "enemy7" || intruder.tag == "enemy8") 
		{
			_Player.SendMessage ("PlayEnemySound");
		}
	}
		
}
