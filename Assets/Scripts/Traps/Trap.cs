using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/**
 * Source code for the traps
 * AIM : Global control of the traps the player can use
 * Method Start() : initialize the number of traps on the GUI
 * Method Update(): Call the method used for placing a trap when the player press the linked key
 * Methode PlaceTrap() : Determine where is trap will be placed when the player press the linked key
 * Author : WASMER Audric
 **/
public class Trap : MonoBehaviour {

	public int _nbrHuntingTraps = 5;

	public GameObject _trap;		//Trap from the Prefabs folder
	public GameObject _inGameUI;		//Global InGameUI object
	public GameObject _cameraPivot;

	
	// Update is called once per frame
	void Update () {
		if ((_trap != null) && (_nbrHuntingTraps != 0) && (Input.GetButtonDown ("mouse 1")) && (Time.timeScale != 0.0f)) {
			PlaceTrap ();
		}
	}

	/**
	 * Method: PlaceTrap()
	 * Param: None
	 * Desc: Allow the player to place a trap in the direction where he's looking
	 * Return: None
	 * Author: WASMER Audric
	 **/
	public void PlaceTrap(){
			
		/*Creating a trap in front of the player*/
		RaycastHit hitPoint;
		Ray spawnPoint = _cameraPivot.GetComponent<Camera>().ScreenPointToRay (Input.mousePosition);


		if (Physics.Raycast (spawnPoint.origin, spawnPoint.direction, out hitPoint, 5.0f)) {
			
			Vector3 correctedPosition;

			/* Case of unknown source which place the trap right under the player. This is some patchwork way to avoid it. */
			if ((int)hitPoint.point.x == (int)transform.position.x
			    && (int)hitPoint.point.z == (int)transform.position.z) {

				if (spawnPoint.direction.x < 0) {
					
					if (spawnPoint.direction.z < 0) {
						correctedPosition = new Vector3 (hitPoint.point.x + (spawnPoint.direction.x * 5),
							hitPoint.point.y, hitPoint.point.z + (spawnPoint.direction.z * 5));
					} 

					else {
						correctedPosition = new Vector3 (hitPoint.point.x + (spawnPoint.direction.x * 5),
							hitPoint.point.y, hitPoint.point.z - (spawnPoint.direction.z * 5));
					}
				} 

				else {
					if (spawnPoint.direction.z > 0) {
						correctedPosition = new Vector3 ((spawnPoint.direction.x * 5) - hitPoint.point.x,
							hitPoint.point.y, (spawnPoint.direction.z * 5) - hitPoint.point.z);
					} 
					else {
						correctedPosition = new Vector3 ((spawnPoint.direction.x * 5) - hitPoint.point.x,
							hitPoint.point.y, hitPoint.point.z + (spawnPoint.direction.z * 5));
					}
				}
			} 

			else {
				correctedPosition = hitPoint.point;
			}

			Vector3 ObjectSpawnPosition = correctedPosition;
			ObjectSpawnPosition = new Vector3 (ObjectSpawnPosition.x, Terrain.activeTerrain.SampleHeight(ObjectSpawnPosition),
				ObjectSpawnPosition.z);
				
			Instantiate (_trap, ObjectSpawnPosition, Quaternion.identity);
				
			/*Updating number of remaining traps in Trap.cs & InGameUI.cs*/
			_nbrHuntingTraps--;
			if (_inGameUI != null)
				_inGameUI.SendMessage ("GetNumberOfTraps", _nbrHuntingTraps);
			
		}
	}
}
 