using UnityEngine;
using System.Collections.Generic;

  public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    private List<string> unlockedLevels = new List<string>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadUnlockedLevels();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadUnlockedLevels()
    {
        unlockedLevels = new List<string>();
        int totalLevels = 14; // Por exemplo, 14 níveis
        for (int i = 1; i <= totalLevels; i++)
        {
            string key = $"Fase{i}_unlocked";
            if (PlayerPrefs.GetInt(key, i == 1 ? 1 : 0) == 1)
                unlockedLevels.Add($"Fase {i}");
        }
        Debug.Log("Loaded unlocked levels: " + string.Join(", ", unlockedLevels.ToArray()));
    }

    public bool IsLevelUnlocked(string levelName)
    {
        return unlockedLevels.Contains(levelName);
    }

    public void UnlockNextLevel(string completedLevelName)
    {
        Debug.Log($"Attempting to unlock next level after completing {completedLevelName}");

        int index = unlockedLevels.IndexOf(completedLevelName);
        if (index != -1)
        {
            string nextLevel = $"Fase {index + 2}";
            if (!unlockedLevels.Contains(nextLevel))
            {
                unlockedLevels.Add(nextLevel);
                PlayerPrefs.SetInt($"{nextLevel}_unlocked", 1);
                PlayerPrefs.Save();
                Debug.Log($"Unlocked level: {nextLevel}");
            }
        }
        else
        {
            Debug.LogWarning($"Completed level {completedLevelName} not found in unlockedLevels.");
        }
    }

    public void UnlockLevel(string levelName)
    {
        if (!unlockedLevels.Contains(levelName))
        {
            unlockedLevels.Add(levelName);
            PlayerPrefs.SetInt($"{levelName}_unlocked", 1);
            PlayerPrefs.Save();
            Debug.Log($"Unlocked level: {levelName}");
        }
    }
}