using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string saveFile => Path.Combine(Application.persistentDataPath, "savegame.json");

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFile, json);
    }

    public static SaveData Load()
    {
        if (File.Exists(saveFile))
        {
            string json = File.ReadAllText(saveFile);
            return JsonUtility.FromJson<SaveData>(json);
        }
        return new SaveData();
    }

    public static bool SaveExists()
    {
        return File.Exists(saveFile);
    }
}
