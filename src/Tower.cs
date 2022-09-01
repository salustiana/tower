using Godot;
using System.Collections.Generic;

public class Tower : Node2D
{
	const int FirePower = 20;
	const float FirePeriod = 0.2f;

	float fireTimer = 0.0f;
	Enemy target;
	LinkedList<Enemy> enemiesOnSight;
	Area2D detectionArea;

	public override void _Ready()
	{
		target = null;
		detectionArea = GetNode<Area2D>("detectionArea");
		enemiesOnSight = new LinkedList<Enemy>();

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
			LookAt(target.GlobalPosition);
			if (fireTimer >= FirePeriod)
			{
				ShootTarget();
				fireTimer = 0.0f;
			}
		}
	}

	void SightEnemy(Enemy e)
	{
		enemiesOnSight.AddLast(e);
		e.EnemyDied += OnEnemyDied;
		if (target == null)
			SetNewTarget();
	}

	void UnsightEnemy(Enemy e)
	{
		enemiesOnSight.Remove(e);
		e.EnemyDied -= OnEnemyDied;
		if (target == e)
			SetNewTarget();
	}

	void OnAreaEntered(Area2D area)
	{
		Enemy e = area.GetParentOrNull<Enemy>();
		if (e != null)
			SightEnemy(e);
	}

	void OnAreaExited(Area2D area)
	{
		Enemy e = area.GetParentOrNull<Enemy>();
		if (e != null)
			UnsightEnemy(e);
	}

	void OnEnemyDied(Enemy e)
	{
		UnsightEnemy(e);
	}

	void SetNewTarget()
	{
		if (enemiesOnSight.Count > 0)
			target = enemiesOnSight.Last.Value;
		else
			target = null;
	}

	void ShootTarget()
	{
		target.TakeDamage(FirePower);
	}
}
