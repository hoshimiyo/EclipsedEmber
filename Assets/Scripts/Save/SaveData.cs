using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    // Player data
    public float playerHealth = 3f;
    public float playerMana = 1f;
    public Vector3Data playerPosition = new Vector3Data();
    public int playerLevel = 1;
    public int playerExperience = 0;
    public int healthCap = 3;
    
    // Player state
    public bool isFacingRight = true;
    public Vector3Data respawnPoint = new Vector3Data();
    public int availableJumps = 1;
    public bool canDash = true;
    public bool isCasting = false;
    public bool iFrameActive = false;
    
    // Scene data
    public string currentSceneName = string.Empty;
    public int currentSceneBuildIndex = -1;
    public Vector3Data playerSpawnPosition = new Vector3Data();

    // Game progress
    public List<string> unlockedLevels = new List<string>();
    public List<string> collectedItems = new List<string>();
    public Dictionary<string, bool> completedQuests = new Dictionary<string, bool>();

    // Settings
    public float masterVolume = 1f;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
}

// Helper classes for Unity types serialization
[Serializable]
public class Vector3Data
{
    public float x, y, z;
    
    public Vector3Data() { }
    
    public Vector3Data(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }
    
    public Vector3Data(Vector2 vector)
    {
        x = vector.x;
        y = vector.y;
        z = 0;
    }
    
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
    
    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }
}