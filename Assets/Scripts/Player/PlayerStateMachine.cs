using System;
using UnityEngine;

public class PlayerStateMachine : ScriptableObject {

    private GameObject player;
    public GameObject Player { get { return player; } }

	private PlayerController playerController;
	public PlayerController PlayerController { get { return playerController; } }

	private Rigidbody playerRigidbody;
	public Rigidbody PlayerRigidbody { get { return playerRigidbody; } }

	private PlayerState previousState;
	public PlayerState PreviousState { get { return previousState; } }

	private PlayerState currentState;
	public PlayerState CurrentState { get { return currentState; } }

	private PlayerState nextState;
	public PlayerState NextState { get { return nextState; } }

	private bool forced = false;

	public void Awake()
	{
        currentState = new StandingState(this);
		player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
		playerRigidbody = player.GetComponent<Rigidbody>();
	}

	public void Update () 
    {
		if (nextState != null)
		{
			if (currentState != null)
			{
				previousState = currentState;
				previousState.Exit();
			}

			currentState = nextState;
			currentState.Enter();

			nextState = null;

			forced = false;
		}

		if (currentState != null)
		{
			currentState.Update();
		}
	}

	public void FixedUpdate()
	{
		if (currentState != null)
		{
			currentState.FixedUpdate();
		}
	}


	public void SwitchState(PlayerState nextState)
	{
		if (forced)
		{
			return;
		}

		if (nextState == null || (nextState != null && this.nextState != null && nextState.Priority < this.nextState.Priority))
		{
			return;
		}

		this.nextState = nextState;
	}

	public void ForceSwitchState(PlayerState nextState)
	{
		if (forced)
		{
			return;
		}

		this.nextState = nextState;
		forced = true;
	}

	public bool IsInState(Type type)
	{
		return currentState.GetType() == type || (nextState != null && nextState.GetType() == type);
	}
}
