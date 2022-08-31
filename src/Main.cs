using Godot;

public class Main : Node
{
	const int maxHP = 5;
	const float SpawnPeriod = 0.2f;
	PackedScene enemyScene = GD.Load<PackedScene>("res://Enemy.tscn");

	int hp;
	int enemiesSpawned = 0;
	int enemiesOnScreen = 0;
	bool spawning = true;
	float spawnTimer = 0.0f;
	Path2D path;
	Label hpLabel;
	VideoPlayer videoPlayer;

	void OnEnemyDied(Enemy e)
	{
		e.QueueFree();
		enemiesOnScreen--;
	}

	void OnEnemyWin(Enemy e)
	{
		e.EnemyWin -= OnEnemyWin;
		e.QueueFree();
		enemiesOnScreen--;
		hpLabel.Text = (--hp).ToString();
		if (hp <= 0)
			GameOver();
	}

	public override void _Ready()
	{
		hp = maxHP;
		path = GetNodeOrNull<Path2D>("Path2D");
		hpLabel = GetNodeOrNull<Label>("UI/hpLabel");
		hpLabel.Text = hp.ToString();

		videoPlayer = GetNodeOrNull<VideoPlayer>("UI/VideoPlayer");
		videoPlayer.Expand = false;
		videoPlayer.Visible = false;
	}

	public override void _PhysicsProcess(float delta)
	{
		SpawnCoroutine(delta);
		if (!spawning && enemiesOnScreen == 0)
			LevelEnd();
	}

	void SpawnCoroutine(float delta)
	{
		if (!spawning)
			return;

		spawnTimer += delta;
		if (spawnTimer >= SpawnPeriod)
		{
			Enemy e = enemyScene.Instance<Enemy>();
			e.EnemyDied += OnEnemyDied;
			e.EnemyWin += OnEnemyWin;
			path.AddChild(e);
			enemiesSpawned++;
			enemiesOnScreen++;
			spawnTimer = 0.0f;
			if (enemiesSpawned >= 30)
				spawning = false;
		}
	}

	void LevelEnd()
	{
		GD.Print("won!");
		SetPhysicsProcess(false);
		videoPlayer.Visible = true;
		videoPlayer.Play();
	}

	void GameOver()
	{
		GD.Print("game over!");
	}
}
