using System.Collections.Generic;

public struct Level
{
	public List<SpawnCmd> spawnCmds;
}

public struct SpawnCmd
{
	public float timestamp;
	public int enemyType;
	public int enemyCount;
	public float spawnPeriod;
}
