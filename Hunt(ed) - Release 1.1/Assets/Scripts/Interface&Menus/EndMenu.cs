using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void OnButtonClicked(){
		SceneManager.LoadScene ("MainMenu");
	}
}
