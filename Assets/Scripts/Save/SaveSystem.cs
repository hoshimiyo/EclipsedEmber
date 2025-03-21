using UnityEngine;
using System.IO;
using System;

public static class SaveSystem
{
    private static readonly string SaveDir = Path.Combine(Application.persistentDataPath, "Saves");
    private static readonly string SaveFile = "gamesave.json";
    private static readonly string SavePath = Path.Combine(SaveDir, SaveFile);

    public static void SaveGame(SaveData data)
    {
        if (!Directory.Exists(SaveDir))
            Directory.CreateDirectory(SaveDir);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"Game saved to {SavePath}");
    }

    public static SaveData LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("No save file found. Creating new save data.");
            return new SaveData();
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log($"Game loaded from {SavePath}");
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load save file: {e.Message}");
            return new SaveData();
        }
    }

    public static bool SaveExists()
    {
        return File.Exists(SavePath);
    }

    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("Save file deleted");
        }
    }
}