using UnityEngine;
using System.Collections;

/**
 * Source code for the GUI
 * AIM : generating & updating the ingame global UI
 * Method Start() : generate the objects required for the GUI display
 * Method GetStaminaRatio(): get stamina value from PlayerMovement.cs and calculate the new stamina bar width
 * Methode OnGUI() : control the display of the GUI
 * Author : WASMER Audric
 **/
public class InGameUI : MonoBehaviour {


	Rect staminaBar;				// The rectangle which will be used to display the stamina
	Texture2D staminaTexture;		// The texture of the stamina bar
	 
	Rect huntingTraps;				// The rectangle which will be used to display the remaining traps
	public int _nbrHuntingTraps;	// Default number of remaining traps
	TextEditor trapsCount; 			// Text used to display the number of remaining traps
	public Texture trapsIcon;		// Icon used for the traps

	Rect crosshair;					// The rectangle which will be used to display a crosshair
	Texture2D crosshairTexture;		// The texture of the crosshair

	GUIStyle trapsStyle;				// Style object of the traps display
	Font fontEagleLake;				// font of the traps display


	/**
	 * Method: Start()
	 * Desc: Initializing GUI displays
	 * Author: WASMER Audric
	 **/
	void Start () {
		Cursor.lockState = CursorLockMode.Locked;

		/*Initialazing of the stamina bar display*/
		staminaBar = new Rect ((Screen.width / 10), (Screen.height * 9/10), (Screen.width / 3), (Screen.height / 50));
		staminaTexture = new Texture2D (1, 1);
		staminaTexture.SetPixel (0, 0, Color.white);
		staminaTexture.Apply ();
		/*----------------------------------------*/


		/*Initialazing traps number display*/
		trapsCount = new TextEditor ();
		trapsCount.text = _nbrHuntingTraps.ToString ();
		huntingTraps = new Rect ((Screen.width / 1.2f), (Screen.height * 8 / 10), 90, 90);
		fontEagleLake = (Font)Resources.Load ("Fonts/EagleLake-Regular", typeof(Font));
		trapsStyle = new GUIStyle ();
		trapsStyle.font = fontEagleLake;
		trapsStyle.fontSize = 30;
		trapsStyle.normal.textColor = Color.white;
		/*----------------------------------------*/

		/* Initializing a middle-screen crosshair */
		crosshairTexture = new Texture2D (2, 2);
		crosshairTexture.SetPixel(0,0,Color.white);
		crosshairTexture.SetPixel(1,0,Color.white);
		crosshairTexture.SetPixel(0,1,Color.white);
		crosshairTexture.SetPixel(1,1,Color.white);
		crosshairTexture.Apply();
		crosshair = new Rect((Screen.width - crosshairTexture.width) / 2,
			(Screen.height - crosshairTexture.height) /2, crosshairTexture.width, crosshairTexture.height);
		/*----------------------------------------*/

	}

	/**
	 * Method: GetRatioStamina()
	 * Param: float ratioStamina
	 * Desc: used to get the staminaLeft value from PlayerMovement.script, and then to calculate
	 * 		the width of the stamina bar displayed on screen. This width is set into the staminaBar game object.
	 * Return: none
	 * Author: WASMER Audric
	**/
	public void GetRatioStamina(float ratioStamina){
		/*Updating stamina bar's width*/
		float staminaBarWidth = ratioStamina * Screen.width / 3;
		staminaBar.width = staminaBarWidth;

		/*Updating stamina bar's color*/
		if (ratioStamina < 0.3) {
			staminaTexture.SetPixel (0, 0, Color.red);
			staminaTexture.Apply ();
		}
		if ((staminaTexture.GetPixel (0, 0) == Color.red) && (ratioStamina >= 0.3)) {
			staminaTexture.SetPixel (0, 0, Color.white);
			staminaTexture.Apply ();
		}
	}

	/**
	 * Method GetNumberOfTraps()
	 * Param: int nbTraps
	 * Desc: Used to get the number of remaining traps from Trap.cs, then to update the number displayed on the UI
	 * return: None
	 * Author: WASMER Audric
	 **/
	public void GetNumberOfTraps(int nbTraps){
		_nbrHuntingTraps = nbTraps;
		trapsCount.text = _nbrHuntingTraps.ToString();
	}

	/**
	 * Method: OnGUI()
	 * Param : None
	 * Desc : control the display of the global ingame UI
	 * Return : None
	 * Author : WASMER Audric
	 **/
	void OnGUI(){
		/*Stamina display*/
		GUI.DrawTexture (staminaBar, staminaTexture);

		/*Traps left display*/
		GUI.DrawTexture (huntingTraps, trapsIcon, ScaleMode.ScaleToFit);
		GUI.Box (huntingTraps, trapsCount.text, trapsStyle);

		/*Crosshair display*/
		GUI.DrawTexture (crosshair, crosshairTexture);


	}
}
