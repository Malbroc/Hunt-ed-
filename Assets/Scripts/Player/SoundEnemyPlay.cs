/**
 * Source code for Player sound play : SoundEnemyNearby 
 * AIM : Play a random scarying sound (Brrrrr) when enemy enter the trigger !
 * method OnDetectionSphereEnter : Send order to play sound
 * method OnDetectionSphereExit : Send order to stop the sound
 **/

using UnityEngine;
using System.Collections;

public class SoundEnemyPlay : MonoBehaviour {

	public AudioClip[] Sounds;
	public AudioSource _audio;

	private int index = 0;


	// Use this for initialization
	public void PlayEnemySound() 
	{
		if (!_audio.isPlaying) 
		{
			index = Random.Range (0, Sounds.Length - 1);
			_audio.clip = Sounds [index];
			_audio.Play ();
		}
	}

}
