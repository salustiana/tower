using Godot;

using System.IO;

using Newtonsoft.Json;

public class Main : Node
{
	// config
	const int maxHP = 5;
	PackedScene[] enemyTypes = {
		GD.Load<PackedScene>("res://enemies/SimpleEnemy.tscn"),
		GD.Load<PackedScene>("res://enemies/FastEnemy.tscn"),
	};

	// internals
	int hp;
	int enemiesSpawned = 0;
	int enemiesOnScreen = 0;
	Path2D path;
	Label hpLabel;
	Control UI;
	Level level;

	public override void _Ready()
	{
		// setup children
		UI = GetNode<Control>("UI");
		UI.GetNode<Button>("playButton").Connect(
			"pressed", this, nameof(PlayMode)
		);
		UI.GetNode<Button>("towerButton").Connect(
			"pressed", this, nameof(PlaceTower)
		);
		UI.GetNode<Button>("towerButton").ToggleMode = true;
		path = GetNode<Path2D>("Path2D");
		hpLabel = GetNode<Label>("HUD/hpLabel");

		// housekeeping
		hp = maxHP;
		hpLabel.Text = hp.ToString();

		// begin
		SetupMode();
	}

	void LoadLevel()
	{
		using (StreamReader r = new StreamReader("./levels/l1.json"))
		{
			JsonSerializerSettings s = new JsonSerializerSettings();
			s.MissingMemberHandling = MissingMemberHandling.Error;

			string json = r.ReadToEnd();
			level = JsonConvert.DeserializeObject<Level>(json, s);
		}

		foreach (SpawnCmd sc in level.spawnCmds)
			CreateSpawner(sc);
	}

	async void CreateSpawner(SpawnCmd sc)
	{
		await ToSignal(GetTree().CreateTimer(sc.timestamp), "timeout");
		for (int i = 0; i < sc.enemyCount; i++)
		{
			Enemy e = enemyTypes[sc.enemyType].Instance<Enemy>();
			e.EnemyDied += OnEnemyDied;
			e.EnemyWin += OnEnemyWin;
			path.AddChild(e);
			enemiesSpawned++;
			enemiesOnScreen++;
			await ToSignal(GetTree().CreateTimer(
				sc.spawnPeriod
			), "timeout");
		}
	}

	void PlayMode()
	{
		UI.Hide();
		enemiesSpawned = 0;
		enemiesOnScreen = 0;
		LoadLevel();
	}

	void SetupMode()
	{
		UI.Show();
	}

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

	void LevelEnd()
	{
		GD.Print("won!");
		SetPhysicsProcess(false);
	}

	void GameOver()
	{
		GD.Print("game over!");
	}
}
