using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


/**
 * Source code for the in game menu
 * AIM : pausing the game, and allowing the player to leave || continue || restart the game
 * Method Start(): Set the menu as not displayed
 * Method Update(): Intercepting the Escape key press and displaying the menu
 * Methode onClickContinue(): Click on the Continue button to resume the game
 * Methode onClickRestart(): Click on the Restart button to restart the game
 * Method onClickQuit(): Click on the Quit button to get back to the main menu of the game
 * Author : WASMER Audric
 **/
public class InGameMenu : MonoBehaviour {

	public GameObject _inGameMenu;
	public bool display = false;

	// Use this for initialization
	void Start () {
		//By default, the menu is not displayed
		_inGameMenu.SetActive (false);
		display = false;
		Time.timeScale = 1.0f;
	}

	// Update is called once per frame
	public void Update () {

		//When the player press the Escape key
		if (Input.GetKeyDown (KeyCode.Escape)) {

			//If the menu was not displayed, then display it & stop the time
			if (display == false) {				
				_inGameMenu.SetActive (true);
				Time.timeScale = 0.0f;
				display = true;
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			} 
			//If the menu was displayed, then do not display it anymore
			else {
				_inGameMenu.SetActive (false);
				Time.timeScale = 1.0f;
				display = false;
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;

			}
		}
	}
		
	public void onContinueClicked(){	
			
		_inGameMenu.SetActive (false);
		Time.timeScale = 1.0f;
		display = false;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public void onRestartClicked(){
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
		_inGameMenu.SetActive (false);
		Time.timeScale = 1.0f;
		display = false;
	}

	public void onQuitClick(){
		SceneManager.LoadScene ("MainMenu");
	}

}