using Godot;

public class Enemy : PathFollow2D
{

	protected int MaxHP = 100;
	protected float Speed = 350.0f;
	const float BlinkTime = 0.1f;

	int hp;
	float blinkTimer = 0.0f;

	public delegate void HandleEnemyDied(Enemy e);
	public event HandleEnemyDied EnemyDied;
	void Die()
	{
		if (EnemyDied != null)
			EnemyDied(this);
	}

	public delegate void HandleEnemyWin(Enemy e);
	public event HandleEnemyWin EnemyWin;
	void Win()
	{
		if (EnemyWin != null)
			EnemyWin(this);
	}

	public override void _Ready()
	{
		Rotate = false;
		Loop = false;
		hp = MaxHP;
	}

	public override void _PhysicsProcess(float delta)
	{
		BlinkCoroutine(delta);
		Offset += Speed * delta;
		if (UnitOffset >= 0.99f)
			Win();
	}

	void BlinkCoroutine(float delta)
	{
		if (Visible == false)
			blinkTimer += delta;
		if (blinkTimer >= BlinkTime)
		{
			Visible = true;
			blinkTimer = 0.0f;
		}
	}

	public void TakeDamage(int firePower)
	{
		Visible = false;
		hp -= firePower;

		if (hp <= 0)
			Die();
	}
}
