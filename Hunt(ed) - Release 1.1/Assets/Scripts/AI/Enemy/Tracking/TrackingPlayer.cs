/**
 * Source code for Enemy AI : tracking player
 * AIM : Track the player when he/she is in one of the detection spheres
 * method BasicBehavior : Random move for AI
 * Method TrackingLow : Track the player in a "passiv way"
 * method TrackingFast : Track the player in a "aggressiv way"
 * method SetLvlNone : Set tracking lvl to None -- basic behavior for AI
 * method SetLvlLow : Set tracking lvl to low when the correct message is received
 * method SetLvlHigh : Set tracking lvl to High when the correct message is received
 **/

using UnityEngine;
using System.Collections;

enum AIState
{
	Roaming,             // NONE
	PlayerAround,        // LOW
	TrackingPlayer,      // HIGH
	Trapped              // Trapped 
}

// Global macros for detection and enemy behavior


public class TrackingPlayer : MonoBehaviour {

	// Global MAP
	public Terrain _map;

	// Global macros for detection and enemy behavior
	private AIState _currentState = AIState.Roaming;
	private AIState _previousState;

	// Global macros for AI speed
	private const float Roaming_speed = 1.8f; // No chase speed
	private const float PlayerAround_speed = 2.2f; // Low lvl chase speed
	private const float TrackingPlayer_speed = 5.0f; // High lvl chase speed
	private const float TRAPPED_TIME = 3.0f;		// default time of the effect of a trap
	private const float KeepDirection_time = 5.0f;  // default time for AI to keep direction in roaming mode

	// Global macro for movement direction
	private float x = 1; // Random X position 
	private float y = 1; // Random Y position
	private float z = 1; // Random Z position

	// Tracking aim, AI and state
	public GameObject _Player; // Aim of the enemy
	public GameObject _AI; // AI of the pathfinding
	public GameObject _DetectionSphere_High; // DetectionSphere of the AI vision

	private UnityEngine.AI.NavMeshAgent agentAI; // NavMeshAgent of the AI
	private Vector3 targetAI; // target position of AI -- basicBehavior
	private Vector3 mapLimit; // Limit of the map
	private Vector3 mapPosition; // Global position of the map
	private float _trappedTimer = TRAPPED_TIME;	// timer used for traps effect delay
	private float _keepDirectionTimer = KeepDirection_time; // timer used  for AI to keep direction
	private RaycastHit _Hit; // Hit of the rayCast

	public AudioSource _Roar;	// Sound used
	private float ROAR_DELAY = 5.0f; // Delay between two roars
	private float _roarTimer;

	// Use Ai states like variable :
	private AIState CurrentState 
	{
		get { return _currentState; }
		set { _currentState = value; }
	}


	// Use this for initialization
	// Set CurrentState to Roaming
	// Get NavMeshAgent of Player or create it if it does not exist
	// Get AI NavMeshAgent
	// Get actual position of AI
	void Start () 
	{
		CurrentState = AIState.Roaming;

		_AI.AddComponent<UnityEngine.AI.NavMeshAgent>();

		// Get NavMeshAgent of AI
		agentAI = _AI.GetComponent<UnityEngine.AI.NavMeshAgent>();

		agentAI.speed = Roaming_speed;

		// Get actual position of AI and set its target with it
		targetAI = new Vector3 (0.0f, 0.0f, 0.0f);
		targetAI.x = _AI.transform.position.x;
		targetAI.y = _AI.transform.position.y;
		targetAI.z = _AI.transform.position.z;

		y = targetAI.y; // Keep the y position of AI in global variable

		mapLimit = new Vector3 (0.0f, 0.0f, 0.0f);
		// Get terrain size and set it into mapLimit
		mapLimit.x = _map.terrainData.size.x; 
		mapLimit.y = _map.terrainData.size.y;
		mapLimit.z = _map.terrainData.size.z;

		mapPosition = _map.transform.position; // Get global position of the map
		// Add global position of the map into the limit -> get the limit position in global scene position
		mapLimit.x += mapPosition.x;
		mapLimit.y += mapPosition.y;
		mapLimit.z += mapPosition.z;

	}
	
	// Update is called once per frame
	void Update () 
	{
		switch (CurrentState) 
		{
			case AIState.Trapped: 
				if (_trappedTimer <= 0) {
					CurrentState = _previousState;
					_trappedTimer = TRAPPED_TIME;
					GetComponent<Animator> ().SetBool ("isTrapped", false);
				} else {
					_trappedTimer = _trappedTimer - 1.0f * Time.deltaTime;
				}
				break;
			
			case AIState.Roaming:
				GetComponent<Animator> ().SetBool ("tracking", false);
				if (_keepDirectionTimer <= 0) 
				{
					_keepDirectionTimer = KeepDirection_time;
					BasicBehavior ();
				} 
				else 
				{
					_keepDirectionTimer = _keepDirectionTimer - 1.0f * Time.deltaTime;
				}
					break;

			case AIState.PlayerAround:
					GetComponent<Animator> ().SetBool ("tracking", false);
					if (TimerFinished ()) {
						_Roar.Play ();
						RoarDelay ();
					}
					
					
					TrackingLow ();
					break;

			case AIState.TrackingPlayer:
				GetComponent<Animator> ().SetBool ("tracking", true);
				TrackingHigh ();
				break;

		}
	}




	/***************************
	 * Tracking lvl attribution*
	 ***************************/

	/**
	* Method : SetLvlLow
	* Param :  void
	* Desc : Set the CurrentState to PlayerAround, AI speed to PlayerAround_speed
			 and call TrackingLow()
	* Return : Void
	* Author : Grosshenny Guillaume
	**/
	void SetLvlLow()
	{
		CurrentState = AIState.PlayerAround;
		agentAI.speed = PlayerAround_speed;
		//TrackingLow ();
	}


	/**
	* Method : SetLvlHigh
	* Param :  void
	* Desc : Set the CurrentState to TrackingPlayer, AI speed to TrackingPlayer_speed 
	* 		 and call TrackingHigh
	* Return : Void
	* Author : Grosshenny Guillaume
	**/
	void SetLvlHigh()
	{
		CurrentState = AIState.TrackingPlayer;
		agentAI.speed = TrackingPlayer_speed;
		//TrackingHigh ();
	}


	/**
	* Method : SetLvlNone
	* Param :  void
	* Desc : Set the CurrentState to Roaming, AI speed to Roaming_speed
	* 		 and call randDest
	* Return : Void
	* Author : Grosshenny Guillaume
	**/
	void SetLvlNone()
	{
		CurrentState = AIState.Roaming;
		agentAI.speed = Roaming_speed;
		//randDest (); 
	}




	/*****************************
	 * Tracking behavior methods *
	 *****************************/


	/**
	* Method : BasicBehavior
	* Param :  void
	* Desc : Call randDest if AI went to its target
	* Return : Void
	* Author : Grosshenny Guillaume
	**/
	void BasicBehavior()
	{
		randDest ();
		
	}

	/**
	* Method : randDest
	* Param :  void
	* Desc : Select random position for AI to aim and set it into targetAI
	* Return : Void
	* Author : Grosshenny Guillaume
	**/
	void randDest()
	{
		// Random positions (x and z)
		x = Random.Range (mapPosition.x, mapLimit.x); 
		z = Random.Range (mapPosition.z, mapLimit.z);

		targetAI.Set (x, y, z);
		agentAI.SetDestination (targetAI);
	}


	/**
	* Method : TrackingLow
	* Param :  void
	* Desc : Select semi-random direction for AI, concidering player postion
	* 		 and get slowly closer to him/her by average speed
	* Return : Void
	* Author : Grosshenny Guillaume
	**/
	void TrackingLow()
	{
		x = _Player.transform.position.x + Random.Range (-10f, 10f);
		z = _Player.transform.position.z + Random.Range (-10f, 10f);
		targetAI.Set (x, y, z);
		agentAI.SetDestination (targetAI);
	}


	/**
	* Method : TrackingHigh
	* Param :  void
	* Desc : seek player with a RayCast, if nothing is between AI and Player, the AI chase the Player.
	* 		 Else, AI keep trackingLow behavior.
	* Return : Void
	* Author : Grosshenny Guillaume
	**/
	void TrackingHigh()
	{
		Vector3 rayDirection =  _Player.transform.position - transform.position;
		if (Physics.Raycast (transform.position, rayDirection, out _Hit, _DetectionSphere_High.transform.localScale.z)) 
		{
			if (_Hit.collider.tag == _Player.tag) 
			{
				agentAI.speed = TrackingPlayer_speed;	
				agentAI.SetDestination (_Player.transform.position);
	
			}
			else 
			{
				agentAI.speed = PlayerAround_speed;
				x = _Player.transform.position.x + Random.Range (-10f, 10f);
				z = _Player.transform.position.z + Random.Range (-10f, 10f);
				targetAI.Set (x, y, z);
				agentAI.SetDestination (targetAI);
			}

		}
		else 
		{
			agentAI.speed = PlayerAround_speed;
			x = _Player.transform.position.x + Random.Range (-10f, 10f);
			z = _Player.transform.position.z + Random.Range (-10f, 10f);
			targetAI.Set (x, y, z);
			agentAI.SetDestination (targetAI);
		}

	}
	
	/**
	 * Method: GetTrapped()
	 * Param: None
	 * Desc: Called when the concerned object steps on a trap. The boolean _trappedTimerIsSet is set to true, so the object won't be able to move for a short time.
	 * Return: None
	 * Author: WASMER Audric
	 **/	
	public void GetTrapped()
	{
		GetComponent<Animator> ().SetBool ("isTrapped", true);
		agentAI.SetDestination (transform.position);
		_previousState = CurrentState;
		CurrentState = AIState.Trapped;
	}

	/** ----------------------------------------------------------------------------------------------------
	 * Method : RoarDelay
	 * Param : void
	 * Desc : Initialise timer 
	 * Return : void
	 * Author : Martin Genet
	 **/
	private void RoarDelay(){
		_roarTimer = ROAR_DELAY;
	}

	/** ----------------------------------------------------------------------------------------------------
	 * Method : TimerFinished
	 * Param : void
	 * Desc : Decrease the timer by deltaTime and check if it reached 0
	 * Return : bool
	 * Author : Martin Genet
	 **/
	private bool TimerFinished(){
		_roarTimer -= Time.deltaTime;
		return (_roarTimer <= 0.0f);
	}
}
