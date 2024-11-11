using UnityEngine;
using System.Collections.Generic;

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
        // Método para obter os nomes dos níveis. A ser implementado de acordo com seu projeto.
        return new List<string> { "Level1", "Level2", "Level3" }; 
    }
}