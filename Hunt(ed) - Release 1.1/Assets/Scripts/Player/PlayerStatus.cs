using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerStatus : MonoBehaviour {


	// Use this for initialization
	void Start () 
	{
	}

	/**
	* Method : SetDead
	* Param : void
	* Desc : Set the Alive bool to false on playerStatue and on Player animator.
	* Return : Void
	* Author : Grosshenny Guillaume
	**/
	void SetDead()
	{
		SceneManager.LoadScene ("GameOver");
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}


	/**
	* Method : SetWin
	* Param : void
	* Desc : Set the Win bool to true on playerStatue and on Player animator.
	* Return : Void
	* Author : Grosshenny Guillaume
	**/
	void SetWin()
	{
		SceneManager.LoadScene ("Win");
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}
}
