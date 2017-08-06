using System;
using UnityEngine;

public class EnemyStateMachine : ScriptableObject
{
    GameObject _enemy;
    EnemyController _enemyController;

    public void init(GameObject enemy, EnemyController enemyController) {
        _enemy = enemy;
        _enemyController = enemyController;

        if (_enemy.tag == "EnemyBox")
        {
            currentState = new AggressiveBoxState(this);
        }

        if (currentState != null)
        {
            currentState.Enter();
        }
        else
        {
            Debug.Log("No initialstate found for enemy type!");
        }
    }

    public GameObject Enemy { get { return _enemy; } }

    public EnemyController EnemyController { get { return _enemyController; } }

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
