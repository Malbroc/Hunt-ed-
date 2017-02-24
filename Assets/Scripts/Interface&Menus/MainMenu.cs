/**
 * Navigate through the main menu pages, displaying credits and controls, changing options, starting game etc.
 **/
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
	public GameObject _Credits;
	public GameObject _Options;
	public GameObject _Controls;

	private bool _showCredits = false;
	private bool _showOptions = false;
	private bool _showControls = false;

	// Use this for initialization
	void Start () {
		Cursor.visible = true;
		_Credits.SetActive (false);
		_Options.SetActive (false);
		_Controls.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/** ----------------------------------------------------------------------------------------------------
	 * Method : SwapMenu
	 * Param : void
	 * Desc : Set the right menu page active (only one or zero of the 3 booleans should be true)
	 * Return : void
	 * Author : Martin Genet
	 **/
	void SwapMenu(){
		_Options.SetActive (_showOptions);
		_Controls.SetActive (_showControls);
		_Credits.SetActive (_showCredits);
	}

	/** ----------------------------------------------------------------------------------------------------
	 * Methods : OnCreditsClicked, OnOptionsClicked, OnControlsClicked
	 * Param : void
	 * Desc : Respectively setting the booleans _showCredits, _showOptions and _showControls
	 * to the right value.
	 * Return : void
	 * Author : Martin Genet
	 **/
	public void OnCreditsClicked(){
		if (!_showCredits) {
			_showCredits = true;
		} else {
			_showCredits = false;
		}
		_showOptions = false;
		_showControls = false;
		SwapMenu ();
	}

	public void OnOptionsClicked(){
		if (!_showOptions) {
			_showOptions = true;
		} else {
			_showOptions = false;
		}
		_showCredits = false;
		_showControls = false;
		SwapMenu ();
	}

	public void OnControlsClicked(){
		if (!_showControls) {
			_showControls = true;
		} else {
			_showControls = false;
		}
		_showCredits = false;
		_showOptions = false;
		SwapMenu ();
	}

	/** ----------------------------------------------------------------------------------------------------
	 * Method : OnPlayClicked
	 * Param : void
	 * Desc : Load the Game scene to start playing
	 * Return : void
	 * Author : Martin Genet
	 **/
	public void OnPlayClicked(){
		SceneManager.LoadScene ("Game");
		Cursor.visible = false;
	}

	/** ----------------------------------------------------------------------------------------------------
	 * Method : OnExitClicked
	 * Param : void
	 * Desc : Quit the game
	 * Return : void
	 * Author : Martin Genet
	 **/
	public void OnExitClicked(){
		Application.Quit ();
	}

	/** ----------------------------------------------------------------------------------------------------
	 * Methods : OnGLowClicked, OnGMediumClicked, OnGHighClicked
	 * Param : void
	 * Desc : Respectiveley setting the graphic quality level to low, medium or high
	 * Return : void
	 * Author : Martin Genet
	 **/
	public void OnGLowClicked(){
		QualitySettings.SetQualityLevel (0, false);
	}

	public void OnGMediumClicked(){
		QualitySettings.SetQualityLevel (2, false);
	}

	public void OnGHighClicked(){
		QualitySettings.SetQualityLevel (5, false);
	}

	/** ----------------------------------------------------------------------------------------------------
	 * Methods : OnAAx2Clicked, OnAAx4Clicked, OnAAx8Clicked
	 * Param : void
	 * Desc : Respectiveley setting the anti-aliasing to MSAx2, MSAx4, MSAx8
	 * Return : void
	 * Author : Martin Genet
	 **/
	public void OnAAx2Clicked(){
		QualitySettings.antiAliasing = 2;
	}

	public void OnAAx4Clicked(){
		QualitySettings.antiAliasing = 4;
	}

	public void OnAAx8Clicked(){
		QualitySettings.antiAliasing = 8;
	}
}
