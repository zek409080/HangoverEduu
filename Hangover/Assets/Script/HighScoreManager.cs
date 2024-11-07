using UnityEngine;

public class HighScoresManager : MonoBehaviour
{
    public static HighScoresManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public int GetHighScore(string levelName)
    {
        return PlayerPrefs.GetInt($"{levelName}_highscore", 0);
    }

    public void SetHighScore(string levelName, int score)
    {
        int currentHighScore = GetHighScore(levelName);
        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt($"{levelName}_highscore", score);
        }
    }
}