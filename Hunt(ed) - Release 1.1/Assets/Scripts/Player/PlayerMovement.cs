/**
 * Deals with the player and camera movements for a First Person Sight.
 **/
using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	// Maccros
	private const float CAMERA_TURN_FACTOR = 6.0f;		// camera sensibility
	private const float SPRINT_FACTOR = 3.0f;			// acceleration factor while sprinting
	private const float SNEAK_FACTOR = 0.3f;			// slowing factor while crouching
	private const float TEST_GROUND_DISTANCE = 0.1f;	// distance to check if on earth
	private const float DEFAULT_SPEED = 2.0f;			// pretty obvious by itself :)
	private const float DEFAULT_STAMINA = 10.0f;  		// maximum sprint duration in seconds
	private const float REGEN_DELAY = 5.0f;  			// delay before stamina regeneration in seconds
	private const float CAM_Y_DELTA = 0.66f; 			// maximum distance between standing and crouching camera position
	private const float HEAD_DELTA = 0.1f;				// meximum distance between top and bottom camera position for head bobbing
	private const float TRAPPED_TIME = 3.0f;			// default time of the effect of a trap
	private const float SLOWED_DOWN_TIME = 2.0f;		// default time of the effect of a clue area

	// public variables
	public float _runSpeed = DEFAULT_SPEED;				// actual player speed
	public float _strafeSpeed = DEFAULT_SPEED;			// actual strafe speed
	public float _staminaLeft = DEFAULT_STAMINA;		// number of seconds left sprinting

	// private variables
	private float _jumpForce = 6.0f;					// jump force impulsion
	private float _timer	= REGEN_DELAY;				// timer used for stamina regeneration delay
	private float _crouchingSpeed = 5.0f;				// crouching movement (up/down) speed 
	private float _bobbingSpeed = 0.2f;					// head bobbing movement (up/down) speed
	private bool _timerIsSet = false;					// tells if the timer for stamina regeneration is running
	private float _currentCamYDelta = 0.0f;				// current distance to normal camera position (used for crouching)
	private float _headBobbing = 0.0f;					// current distance to normal camera position (used for headBobbing)
	private bool _bobbingUp = false;					// tells if head is bobbing up or down
	private float _camRotX = 0.0f;						// Current camera rotation around X
	private float _distToGround = 0.0f;					// Current distance to the ground
	private float _trappedTimer = TRAPPED_TIME;			// timer used for traps effect delay
	private bool _trappedTimerIsSet = false;			// tells if the player stepped on a trap
	private bool _isCrouching = false;					// tells if the player is crouching
	private bool _isSlowedDown = false;					// tells if the player is slowed down by a clue area
	private float _SlowedDownTimer = SLOWED_DOWN_TIME;	// timer used for clue area's effect delay

	// GameObjects
	public GameObject _cameraPivot;						// Empty used to rotate camera around X
	public GameObject _inGameUI;						// in game User Interface 
	public GameObject _light;							// Light source attached to the player
	public AudioSource _footStep;						// footstep sound to play on each headbobbing
	public AudioSource _HeartBeat;						// Heart beat while running
	public AudioSource _Breath; 						// Breath while running
	private bool _heartPlay = false;

	// Use this for initialization
	void Start () {
		Cursor.visible = false;
		_distToGround = GetComponent<Collider>().bounds.extents.y;
	}


	// Update is called once per frame
	void Update () {
		// Check if the game is paused. If yes, ignore.
		if (Time.timeScale == 0.0f)
			return;

		// Inform user interface about the current stamina ratio
		_inGameUI.SendMessage ("GetRatioStamina", (_staminaLeft / DEFAULT_STAMINA));
		if ((_staminaLeft / DEFAULT_STAMINA) <= 0.3f) {
			if (!_heartPlay) {
				_HeartBeat.Play ();
				_heartPlay = true;
			}
		} else {
			if (_heartPlay) {
				_HeartBeat.Stop ();
				_heartPlay = false;
			}
		}

		// If the timer is running, decrease timer by 1 second, or stop it if it reached 0
		if (_timerIsSet) {
			if (_timer <= 0) {
				StopRegenTimer ();
			} else {
				_timer = _timer - 1.0f * Time.deltaTime;
			}
		}
		
		//If the trap timer is running, decrease timer by 1 second, or stop it if it reached 0
		if (_trappedTimerIsSet) {
			if (_trappedTimer <= 0) {
				_trappedTimerIsSet = false;
				_trappedTimer = TRAPPED_TIME;
			}
			else {
				_trappedTimer = _trappedTimer - 1.0f * Time.deltaTime;
			}
		}

		// If the slowdown timer is running, decrease timer by 1 second, or stop it if it reached 0
		if (_isSlowedDown) {
			if (_SlowedDownTimer <= 0) {
				_isSlowedDown = false;
				_SlowedDownTimer = SLOWED_DOWN_TIME;
			}
			else {
				_SlowedDownTimer = _SlowedDownTimer - 1.0f * Time.deltaTime;
			}
		}

		StaminaManagement ();
		CrouchOrStand ();
		_light.SendMessage ("PlayerPosition", _isCrouching);
		MoveCamera ();
		MovePlayer ();
		GravityIsNotJustAWord ();
	}

	/** ----------------------------------------------------------------------------------------------------
	 * Method : IsOnEarth
	 * Param : void
	 * Desc : Casts a ray toward the ground and test if the player is standing on it or not.
	 * Return : true if on Earth, else false
	 * Author : Martin Genet
	 **/
	private bool IsOnEarth(){
		return Physics.Raycast(transform.position, -Vector3.up, _distToGround + TEST_GROUND_DISTANCE);
	}


	/** ----------------------------------------------------------------------------------------------------
	 * Method : StaminaManagement
	 * Param : void
	 * Desc : Regenerate stamina if it should do so
	 * Return : void
	 * Author : Martin Genet
	 **/
	private void StaminaManagement () {
		// If player is running or timer is running, ignore.
		if (_timerIsSet || (Input.GetKey (KeyCode.LeftShift) && _staminaLeft > 0))
			return;

		// Regeneration process
		if (_staminaLeft < DEFAULT_STAMINA) {
			_staminaLeft = _staminaLeft + 2.0f * Time.deltaTime;
		}
	}


	/** ----------------------------------------------------------------------------------------------------
	 * Method : StartRegenTimer
	 * Param : void
	 * Desc : Start a timer to wait before stamina can regenerate
	 * Return : void
	 * Author : Martin Genet
	 **/
	private void StartRegenTimer () {
		_timer = REGEN_DELAY;
		_timerIsSet = true;
	}


	/** ----------------------------------------------------------------------------------------------------
	 * Method : StopRegenTimer
	 * Param : void
	 * Desc : Stop the timer to wait before stamina can regenerate
	 * Return : void
	 * Author : Martin Genet
	 **/
	private void StopRegenTimer () {
		_timerIsSet = false;
	}


	/** ----------------------------------------------------------------------------------------------------
	 * Method : Crouch
	 * Param : void
	 * Desc : Make the Player crouch or stand up
	 * Return : void
	 * Author : Martin Genet
	 **/
	private void CrouchOrStand () {
		// If the player is crouching
		if (Input.GetKey (KeyCode.LeftControl)) {
			// but not yet totally crouched
			if (_currentCamYDelta < CAM_Y_DELTA) {
				_cameraPivot.transform.position -= _cameraPivot.transform.up * _crouchingSpeed * Time.deltaTime;
				_currentCamYDelta += _crouchingSpeed * Time.deltaTime;
			}
			_isCrouching = true;
		} else { // If the player is standing up
			if (_currentCamYDelta > 0.0f) { // but yet not totally up
				_cameraPivot.transform.position += _cameraPivot.transform.up * _crouchingSpeed * Time.deltaTime;
				_currentCamYDelta -= _crouchingSpeed * Time.deltaTime;
			}
			_isCrouching = false;
		}
	}


	/** ----------------------------------------------------------------------------------------------------
	 * Method : HeadBobbing 
	 * Param : void
	 * Desc : Make the player's head move up and down while walking
	 * Return : void
	 * Author : Martin Genet
	 **/
	private void HeadBobbing () {
		// If the player is not moving, or in the air, or crouching, ignore.
		if (((int)Input.GetAxis ("Vertical") == 0) || !IsOnEarth () || (Input.GetKey(KeyCode.LeftControl)))
			return;

		if (_bobbingUp) {
			// if head is bobbing up 
			if (_headBobbing > 0.0f) { // but has not yet reached highest point
				_cameraPivot.transform.position += _cameraPivot.transform.up * _bobbingSpeed * _runSpeed * Time.deltaTime;
				_headBobbing -= _bobbingSpeed * _runSpeed * Time.deltaTime;
			} else {
				_bobbingUp = false;
			}
		} else { // if head is bobbing down
			if (_headBobbing < HEAD_DELTA) {
				_cameraPivot.transform.position -= _cameraPivot.transform.up * _bobbingSpeed * _runSpeed * Time.deltaTime;
				_headBobbing += _bobbingSpeed * _runSpeed * Time.deltaTime;
			} else {
				_footStep.Play ();
				_bobbingUp = true;
			}
		}
	}


	/** ----------------------------------------------------------------------------------------------------
	 * Method : GravityIsNotJustAWord
	 * Param : void
	 * Desc : increases gravity force on the player while he's in the air to make jumps more realistic
	 * Return : void
	 * Author : Martin Genet
	 **/
	private void GravityIsNotJustAWord(){
		// If the player is on the ground, ignore.
		if (IsOnEarth ())
			return;

		// If not, bring this dumb player back on reality
		GetComponent<Rigidbody> ().AddForce (new Vector3(0, Physics.gravity.y * Time.deltaTime, 0), ForceMode.VelocityChange);
	}


	/** ----------------------------------------------------------------------------------------------------
	 * Method : MoveCamera
	 * Param : void
	 * Desc : Transform the rotation of the Player/Camera pivot with mouse input
	 * Return : void
	 * Author : Martin Genet
	 **/
	private void MoveCamera () {
		// Get mouse input
		float horizontalMouseInput = Input.GetAxis("Mouse X");
		float verticalMouseInput = Input.GetAxis("Mouse Y");
		_camRotX += verticalMouseInput * CAMERA_TURN_FACTOR;

		// Rotate player around Y axis with horizontal mouse movements
		transform.RotateAround(transform.position, transform.up, 
			CAMERA_TURN_FACTOR * horizontalMouseInput);

		// If the rotation exceeds 90° (up or down), ignore the rest.
		if (_camRotX < -90f) {
			_camRotX = -90f;
			return;
		}
		if (_camRotX > 90f) {
			_camRotX = 90f;
			return;
		}
		
		// Rotate camera pivot (and NOT player) around X axis with vertical mouse movements (not inverted)
		_cameraPivot.transform.RotateAround (_cameraPivot.transform.position, _cameraPivot.transform.right,
			- verticalMouseInput * CAMERA_TURN_FACTOR);
	}


	/** ----------------------------------------------------------------------------------------------------
	 * Method : MovePlayer
	 * Param : void
	 * Desc : Transform the position of the Player with keyboard input
	 * Return : void
	 * Author : Martin Genet
	 **/
	private void MovePlayer () {
		// Get the input from player movement keys
		float horizontalInput = Input.GetAxis ("Horizontal");
		float verticalInput = Input.GetAxis ("Vertical");

		// Check if Player should be exhausted
		if (!_timerIsSet && (_staminaLeft <= 3.0f) && (Input.GetKey (KeyCode.LeftShift))) {
			StartRegenTimer ();
		}

		// Setting the run speed to default
		_runSpeed = DEFAULT_SPEED;
		_strafeSpeed = DEFAULT_SPEED;

		// If the player stepped on a trap
		if (_trappedTimerIsSet) {
			_runSpeed = 0.0f;
			_strafeSpeed = 0.0f;
		} else {
			// Get the action Sneak
			if (Input.GetKey (KeyCode.LeftControl)) {
				_runSpeed = DEFAULT_SPEED * SNEAK_FACTOR;
				_strafeSpeed = DEFAULT_SPEED * SNEAK_FACTOR;
			} else {
				// Get the action Sprint
				if (Input.GetKey (KeyCode.LeftShift)) {
					if (_staminaLeft <= 0.0f) {
						// Nope, you can't sprint.
						Debug.Log ("No stamina left");
						_Breath.Stop ();
						_HeartBeat.Stop ();
					} else {
						// Increase run speed and decrease stamina
						_runSpeed = DEFAULT_SPEED * SPRINT_FACTOR;
						_strafeSpeed = DEFAULT_SPEED * SPRINT_FACTOR * 0.5f;
						_staminaLeft = _staminaLeft - 1.0f * Time.deltaTime;
						if (!_Breath.isPlaying && !_HeartBeat.isPlaying) 
						{
							_Breath.Play ();
						}
					}
				}
				else
				{
					if (_Breath.isPlaying && _HeartBeat.isPlaying) 
					{
						_Breath.Stop ();
					}
				}
			}
			if (_isSlowedDown) {
				_runSpeed = _runSpeed * 0.6f;
				_strafeSpeed = _strafeSpeed * 0.6f;
			}
		}

		// Run, player, run !
		transform.position += transform.forward * _runSpeed * Time.deltaTime * verticalInput
		+ transform.right * _strafeSpeed * Time.deltaTime * horizontalInput;

		HeadBobbing ();

		// Get the action Jump
		if (Input.GetButtonDown ("Jump") && !Input.GetKey(KeyCode.LeftControl) && !_trappedTimerIsSet) {
			// and test if player is on the ground
			if (IsOnEarth ()) {
				GetComponent<Rigidbody> ().AddForce (new Vector3(0, 
					_jumpForce * GetComponent<Rigidbody> ().mass, 0), ForceMode.Impulse);
			} else {
				// Nope, you can't fly.
				Debug.Log ("Not on ground, " + TEST_GROUND_DISTANCE);
			}
		}
	}
	
		/**
	 * Method: GetTrapped()
	 * Param: None
	 * Desc: Called when the concerned object steps on a trap. The boolean _trappedTimerIsSet is set to true, so the object won't be able to move for a short time.
	 * Return: None
	 * Author: WASMER Audric
	 **/	
	public void GetTrapped(){
		_runSpeed = 0.0f;
		_trappedTimerIsSet = true;
	}

	/**
	 * Method: SlowDownClueArea()
	 * Param: None
	 * Desc: Called when the player walks into the area of effect of a clue. He will then be slowed down for a few seconds.
	 * Author: WASMER Audric
	 **/
	public void SlowDownClueArea(){
		_isSlowedDown = true;
	}
}
