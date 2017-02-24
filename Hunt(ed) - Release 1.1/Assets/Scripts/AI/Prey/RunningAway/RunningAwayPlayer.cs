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

enum PAIState {
	Roaming,
	PlayerAround,
	Trapped,
	Flee
}

public class RunningAwayPlayer : MonoBehaviour {
	// Property : current AI state
	private PAIState _currentState = PAIState.Roaming;
	private PAIState CurrentState {
		get { return _currentState; }
		set { _currentState = value; }
	}

	// Used to temporary memorise previous state
	private PAIState _previousState;

	// Global MAP
	public Terrain _map;

	// Global macros for AI speed
	private const float ROAM_SPEED = 2.0f; // Speed for low detection states
	private const float RUN_SPEED = 5.5f; // High detection run speed
	private const float TRAPPED_TIME = 3.0f; // default time of the effect of a trap

	// Used for Roaming state
	private const float ROAM_DIFF = 20.0f; // Maximum difference in x- and z-axis for roaming target
	private const float ROAM_CYCLE = 10.0f; // Maximum time (in sec) roaming to the same direction (what I call a roam cycle)

	// Tracking aim, AI and state
	public GameObject _Player; // Aim of the enemy
	public GameObject _AI; // AI of the pathfinding

	// Positions, navmesh properties, map borders info
	private Vector3 _targetPoint;
	private NavMeshAgent agentAI; // NavMeshAgent of the AI
	public GameObject _mapCornerMin; // Map corner with minimum x and z
	public GameObject _mapCornerMax; // Map corner with maximum x and z
	//private Vector3 _middle;

	// Timer
	private float _timer;


	/** ----------------------------------------------------------------------------------------------------
	 * Method : LaunchTimer
	 * Param : float value of the new timer duration
	 * Desc : Initialise _roamingTimer to the new timer duration
	 * Return : void
	 * Author : Martin Genet
	 **/
	private void LaunchTimer(float value){
		_timer = value;
	}

	/** ----------------------------------------------------------------------------------------------------
	 * Method : TimerFinished
	 * Param : void
	 * Desc : Decrease the timer by deltaTime and check if it reached 0
	 * Return : bool
	 * Author : Martin Genet
	 **/
	private bool TimerFinished(){
		_timer -= Time.deltaTime;
		return (_timer <= 0.0f);
	}

	/** ----------------------------------------------------------------------------------------------------
	 * Method : SetRandomPoint
	 * Param : void
	 * Desc : Initialise _targetPoint to a new target based on Prey's actual position
	 * Return : void
	 * Author : Martin Genet
	 **/
	private void SetRandomPoint(){
		float rx = Random.Range (transform.position.x - ROAM_DIFF, transform.position.x + ROAM_DIFF);
		float ry = transform.position.y;
		float rz = Random.Range (transform.position.z - ROAM_DIFF, transform.position.z + ROAM_DIFF);
		// Clamp between map corners
		rx = Mathf.Clamp(rx, _mapCornerMin.transform.position.x, _mapCornerMax.transform.position.x);
		rz = Mathf.Clamp (rz, _mapCornerMin.transform.position.z, _mapCornerMax.transform.position.z);
		_targetPoint = new Vector3 (rx, ry, rz);
	}

	/** ----------------------------------------------------------------------------------------------------
	 * Method : MoveRandomly
	 * Param : void
	 * Desc : Make the prey move toward the defined random point
	 * Return : true if the target point has been reached, else false
	 * Author : Martin Genet
	 **/
	private bool MoveRandomly(){
		// freeze target point elevation
		_targetPoint.y = transform.position.y;
		// Compute distances
		Vector3 toTarget = _targetPoint - transform.position;
		float remainingDistance = toTarget.magnitude;
		float distanceThisFrame = ROAM_SPEED * Time.deltaTime;

		// Arrived ?
		return (remainingDistance <= distanceThisFrame);
	}

	/**
	* Method : MoveAwayFromPlayer
	* Param :  void
	* Desc : Set _targetPoint to a semi-random direction for AI, concidering player postion
	* 		 to move away
	* Return : Void
	* Author : Martin Genet
	**/
	void MoveAwayFromPlayer()
	{
		// Store the transform in a temporary variable
		Transform tempoTransform = transform;
		// Orient away from the player
		tempoTransform.rotation = Quaternion.LookRotation(tempoTransform.position - _Player.transform.position);
		// Create a position in that direction
		Vector3 runTo = tempoTransform.position + tempoTransform.forward*5;

		// Find a point on NavMesh
		NavMeshHit hit;
		NavMesh.SamplePosition (runTo, out hit, 15, NavMesh.AllAreas);
		_targetPoint = new Vector3 (
			Random.Range (hit.position.x - 10, hit.position.x + 10),
			hit.position.y,
			Random.Range (hit.position.z - 10, hit.position.z + 10));
	}

	/**
	* Method : RunAway
	* Param :  void
	* Desc : Set _targetPoint to the opposite of player's position
	* Return : Void
	* Author : Martin Genet
	**/
	void RunAway()
	{
		// Store the transform in a temporary variable
		Transform tempoTransform = transform;
		// Orient away from the player
		tempoTransform.rotation = Quaternion.LookRotation(tempoTransform.position - _Player.transform.position);
		// Create a position in that direction
		Vector3 runTo = tempoTransform.position + tempoTransform.forward*5;

		// Find a point on NavMesh
		NavMeshHit hit;
		NavMesh.SamplePosition (runTo, out hit, 5, NavMesh.AllAreas);
		_targetPoint = hit.position;
	}

	// Use this for initialization
	// Get NavMeshAgent of Player or create it if it does not exist
	// Get AI NavMeshAgent
	// Get actual position of AI
	void Start () 
	{
		/*// Compute middle of the map
		_middle = new Vector3(
			(_mapCornerMax.transform.position.x + _mapCornerMin.transform.position.x)/2,
			0,
			(_mapCornerMax.transform.position.z + _mapCornerMin.transform.position.z)/2);
		*/
		_AI.AddComponent<NavMeshAgent>();

		// Get NavMeshAgent of AI
		agentAI = _AI.GetComponent<NavMeshAgent>();
		agentAI.speed = ROAM_SPEED;

		// Initialize a random target 
		SetRandomPoint();
		LaunchTimer (ROAM_CYCLE);
	}

	// Update is called once per frame
	void Update () 
	{
		switch (CurrentState) {
		case PAIState.Roaming: // Roaming randomly while Player is not in the area
			// If Prey travelled long enough
			if (TimerFinished () || MoveRandomly ()) {
				LaunchTimer (ROAM_CYCLE);
				SetRandomPoint ();
				agentAI.SetDestination (_targetPoint);
			}
			break;

		case PAIState.PlayerAround: // If Player is in the larger detection sphere
			// We have to re-compute target position beacause the player is moving
			MoveAwayFromPlayer();
			agentAI.SetDestination (_targetPoint);
			break;

		case PAIState.Trapped: // If Prey stepped on a trap
			if (TimerFinished ()) {
				// If trap duration passed, go back to previous state
				CurrentState = _previousState;
			}
			break;

		case PAIState.Flee: // If Player has been seen 
			// We have to re-compute target position beacause the player is moving
			RunAway();
			agentAI.SetDestination (_targetPoint);
			break;
		}
	}


	/*****************************
	 * Detection lvl attribution *
	 *****************************/

	/**
	* Method : SetLvlLow
	* Param :  void
	* Desc : Set CurrentState to PlayerAround
	* Return : Void
	* Author : Grosshenny Guillaume
	**/
	void SetLvlLow()
	{
		Debug.Log ("niveau d'alerte LOW");
		CurrentState = PAIState.PlayerAround;
	}


	/**
	* Method : SetLvlHigh
	* Param :  void
	* Desc : Set CurrentState to Flee, AI speed to RUN_SPEED 
	* Return : Void
	* Author : Grosshenny Guillaume
	**/
	void SetLvlHigh()
	{
		Debug.Log ("Alerte niveau HIGH");
		CurrentState = PAIState.Flee;
		agentAI.speed = RUN_SPEED;
	}


	/**
	* Method : SetLvlNone
	* Param :  void
	* Desc : Set CurrentState to Roaming, AI speed to ROAM_SPEED
	* Return : Void
	* Author : Grosshenny Guillaume
	**/
	void SetLvlNone()
	{
		Debug.Log ("Alerte niveau NONE");
		CurrentState = PAIState.Roaming;
		agentAI.speed = ROAM_SPEED;
	}
	
	/**
	 * Method: GetTrapped()
	 * Param: None
	 * Desc: Method called via SendMessage when stepping on a trap. Sets _previousState, CurrentState and launch a timer.
	 * Return: None
	 * Author: GENET Martin
	 **/
	public void GetTrapped(){
		_previousState = CurrentState;
		LaunchTimer (TRAPPED_TIME);
		CurrentState = PAIState.Trapped;
	}
	
}
