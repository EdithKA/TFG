using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyData
{
    public string id;
    public Vector3 position;
    public float health;
    public bool isAlive;
}

[Serializable]
public class DoorData
{
    public string id;
    public bool isOpen;
}

[Serializable]
public class PuzzleData
{
    public string id;
    public bool isSolved;
}

[Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public int health;
    public int sanity;
    public float stamina;
    public List<string> inventoryItems = new List<string>();
    public List<EnemyData> enemies = new List<EnemyData>();
    public List<DoorData> doors = new List<DoorData>();
    public List<PuzzleData> puzzles = new List<PuzzleData>();
    public string currentScene;
}
