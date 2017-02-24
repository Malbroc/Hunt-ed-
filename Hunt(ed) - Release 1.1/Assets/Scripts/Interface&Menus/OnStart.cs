using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class OnStart : MonoBehaviour {
	private float _timer = 4.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (_timer < 0.0f || Input.anyKeyDown) {
			SceneManager.LoadScene ("MainMenu");
		}

		_timer = _timer - Time.deltaTime;
	}
}
