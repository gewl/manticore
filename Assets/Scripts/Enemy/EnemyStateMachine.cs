using System;
using UnityEngine;

public class EnemyStateMachine : ScriptableObject
{
    GameObject _enemy;
    EnemyController _enemyController;
    Rigidbody _enemyRigidbody;
    Collider _enemyCollider;

    GameObject _player;

	public GameObject Enemy { get { return _enemy; } }
	public EnemyController EnemyController { get { return _enemyController; } }
	public Rigidbody EnemyRigidbody { get { return _enemyRigidbody; } }
    public Collider EnemyCollider { get { return _enemyCollider; } }

    public GameObject Player { get { return _player; }}

	public void Init(GameObject enemy, EnemyController enemyController, Rigidbody enemyRigidbody) {
        _enemy = enemy;
        _enemyController = enemyController;
        _enemyRigidbody = enemyRigidbody;

        _enemyCollider = enemy.GetComponent<Collider>();

        _player = GameObject.FindWithTag("Player");

        if (_enemy.tag == "EnemyBox")
        {
            currentState = new BoxPatrolState(this);
        }

        if (currentState != null)
        {
            currentState.Enter();
            Debug.Log(currentState);
        }
        else
        {
            Debug.Log("No initialstate found for enemy type!");
        }
    }

	private EnemyState previousState;
    public EnemyState PreviousState { get { return previousState; } }

    private EnemyState currentState;
    public EnemyState CurrentState { get { return currentState; } }

    private EnemyState nextState;
    public EnemyState NextState { get { return nextState; } }

    private bool forced = false;

    public void Update()
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
            Debug.Log("Enemy state:");
            Debug.Log(currentState);

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


    public void SwitchState(EnemyState nextState)
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

    public void ForceSwitchState(EnemyState nextState)
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

    public void HandleTriggerEnter(Collider co)
    {
        if (currentState != null)
        {
            currentState.HandleTriggerEnter(co);
        }
    }
}
