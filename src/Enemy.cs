using Godot;

public class Enemy : Node2D
{
	const int MaxHP = 100;
	const float Speed = 80.0f;
	const float BlinkTime = 0.1f;

	int hp;
	float blinkTimer = 0.0f;

	public override void _Ready()
	{
		hp = MaxHP;
	}

	public override void _PhysicsProcess(float delta)
	{
		Position += Vector2.Right * Speed * delta;
		BlinkCoroutine(delta);
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

	void Die()
	{
		QueueFree();
	}
}
