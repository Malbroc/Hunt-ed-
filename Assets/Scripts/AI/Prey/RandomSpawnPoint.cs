using UnityEngine;
using System.Collections;

/**
 * Source code for the definition of the prey's spawn point.
 * AIM : Choose randomly in a few choices where the prey will spawn.
 * Author : WASMER Audric
 **/
public class RandomSpawnPoint : MonoBehaviour {

	private Vector3[] spawnPoints;		// Array of positions

	private int index = 0;				// Index in the array


	void Start () {
		/* Defining possible spawn points */
		spawnPoints = new Vector3 [5];
		spawnPoints [0] = new Vector3 (175.0f, 1.58f, 179.0f);
		spawnPoints [1] = new Vector3 (110.0f, 1.072f, 279.0f);
		spawnPoints [2] = new Vector3 (318.5f, 8.74f, 417.3f);
		spawnPoints [3] = new Vector3 (261.2f, 8.4f, 93.4f);
		spawnPoints [4] = new Vector3 (106.7f, 1.113f, 150.894f);

		/* Choosing one randomly */
		index = Random.Range (0, 4);
		transform.position = spawnPoints [index];


	}
	
}

