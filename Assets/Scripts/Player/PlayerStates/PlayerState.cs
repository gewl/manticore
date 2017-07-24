using UnityEngine;

public abstract class PlayerState
{
	private PlayerStateMachine machine;

	public PlayerStateMachine Machine { get { return machine; } }

	protected int priority = 1;

	public int Priority { get { return priority; } }

	public PlayerState(PlayerStateMachine machine)
	{
		this.machine = machine;
	}

	public virtual void Enter() { }

	public virtual void Exit() { }

	public virtual void Update() { }

	public virtual void FixedUpdate() { }
}