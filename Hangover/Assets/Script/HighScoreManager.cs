using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class HighScoresManager : MonoBehaviour
{
    public static HighScoresManager instance;

    private Dictionary<string, int> highScores = new Dictionary<string, int>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadHighScores();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetHighScore(string levelName)
    {
        if (highScores.ContainsKey(levelName))
            return highScores[levelName];
        return 0;
    }

    public void SetHighScore(string levelName, int score)
    {
        if (score > GetHighScore(levelName))
        {
            highScores[levelName] = score;
            PlayerPrefs.SetInt($"{levelName}_highscore", score);
            PlayerPrefs.Save();
        }
    }

    private void LoadHighScores()
    {
        foreach (string levelName in GetLevelNames())
        {
            highScores[levelName] = PlayerPrefs.GetInt($"{levelName}_highscore", 0);
        }
    }

    private IEnumerable<string> GetLevelNames()
    {
        List<string> levelNames = new List<string>();
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            levelNames.Add(sceneName);
        }
        return levelNames;
    }

    [ContextMenu("ResetHighScores")]
    public void ResetHighScores()
    {
        foreach (string levelName in GetLevelNames())
        {
            PlayerPrefs.DeleteKey($"{levelName}_highscore");
        }
        PlayerPrefs.Save();
        LoadHighScores();
    }
}