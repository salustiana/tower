using Godot;
using System.Collections.Generic;

public class Tower : Node2D
{
	const int FirePower = 20;
	const float FirePeriod = 0.5f;

	float fireTimer = 0.0f;
	Enemy target;
	List<Enemy> enemiesOnSight;
	Area2D detectionArea;

	public override void _Ready()
	{
		target = null;
		detectionArea = GetNodeOrNull<Area2D>("detectionArea");
		enemiesOnSight = new List<Enemy>();

		GetNode<Area2D>("detectionArea").Connect(
			"area_entered",
			this,
			nameof(OnAreaEntered)
		);
		GetNode<Area2D>("detectionArea").Connect(
			"area_exited",
			this,
			nameof(OnAreaExited)
		);

	}

	public override void _PhysicsProcess(float delta)
	{
		fireTimer += delta;

		if (target != null)
		{
			if (!IsInstanceValid(target) || target.IsQueuedForDeletion())
				SetNewTarget();
			else
			{
				LookAt(target.GlobalPosition);
				if (fireTimer >= FirePeriod)
				{
					ShootTarget();
					fireTimer = 0.0f;
				}
			}
		}
	}

	void OnAreaEntered(Area2D area)
	{
		Enemy e = area.GetParentOrNull<Enemy>();
		if (e != null)
		{
			enemiesOnSight.Add(e);
			if (target == null)
				SetNewTarget();
		}
	}

	void OnAreaExited(Area2D area)
	{
		Enemy e = area.GetParentOrNull<Enemy>();
		if (e != null)
		{
			enemiesOnSight.Remove(e);
			if (target == e)
				SetNewTarget();
		}
	}

	void SetNewTarget()
	{
		if (enemiesOnSight.Count > 0)
			target = enemiesOnSight[0];
		else
			target = null;
	}

	void ShootTarget()
	{
		target.TakeDamage(FirePower);
	}
}
