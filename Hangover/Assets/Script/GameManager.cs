using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    #endregion

    [Header("Game Settings")]
    public int initialScore = 0;
    public float fadeDuration = 1f;
    public int initialJogadas = 10;

    [Header("UI Elements")]
    public CanvasGroup faderCanvasGroup;

    public static event System.Action<int> onScoreChanged;
    public static event System.Action<int> onJogadasChanged;

    private int currentScore;
    private int jogadas;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Initialize();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeScene();
        StartCoroutine(FadeIn());
    }

    private void Initialize()
    {
        ResetScore();
        ResetJogadas();
    }

    private void InitializeScene()
    {
        FindFaderCanvasGroup();
        Time.timeScale = 1;

        
    }

    public static void DecrementJogadas()
    {
        if (instance == null) return;

        instance.jogadas--;
        GameManager.onJogadasChanged?.Invoke(instance.jogadas);

        if (instance.jogadas <= 0)
        {
            instance.TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            ObjectiveManager objectiveManager = FindObjectOfType<ObjectiveManager>();
            if (objectiveManager != null && objectiveManager.AllObjectivesCompleted())
            {
                uiManager.ShowVictory("Victory!");
                LevelManager.instance.UnlockNextLevel(SceneManager.GetActiveScene().name);
                Debug.Log("Level completed, attempting to unlock next level.");
            }
            else
            {
                uiManager.ShowGameOver("Game Over!");
            }
        }
    }

    public void LoadScene(string sceneName)
    {
        if (SceneExistsInBuildSettings(sceneName))
        {
            StartCoroutine(FadeAndLoadScene(sceneName));
        }
        else
        {
            Debug.LogError($"Scene '{sceneName}' could not be found in the Build Settings.");
        }
    }

    private bool SceneExistsInBuildSettings(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameInSettings = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneNameInSettings == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        yield return StartCoroutine(FadeOut());
        ResetGameStates();
        SceneManager.LoadScene(sceneName);
    }

    private void ResetGameStates()
    {
        ResetScore();
        ResetJogadas();
        
        ObjectiveManager objectiveManager = FindObjectOfType<ObjectiveManager>();
        if (objectiveManager != null)
        {
            objectiveManager.ResetObjectives();
        }
    }

    private IEnumerator FadeOut()
    {
        FindFaderCanvasGroup();
        if (faderCanvasGroup == null)
        {
            yield break;
        }

        faderCanvasGroup.gameObject.SetActive(true);
        faderCanvasGroup.blocksRaycasts = true;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            faderCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        FindFaderCanvasGroup();
        if (faderCanvasGroup == null)
        {
            yield break;
        }

        faderCanvasGroup.blocksRaycasts = true;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            faderCanvasGroup.alpha = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        faderCanvasGroup.blocksRaycasts = false;

        yield return new WaitForSeconds(fadeDuration);

        faderCanvasGroup.gameObject.SetActive(false);
    }

    private void FindFaderCanvasGroup()
    {
        GameObject fadePanel = GameObject.Find("FadePainel");

        if (fadePanel != null)
        {
            faderCanvasGroup = fadePanel.GetComponent<CanvasGroup>();

            if (faderCanvasGroup == null)
            {
                Debug.LogWarning("CanvasGroup component not found on FadePainel.");
            }
        }
        else
        {
            Debug.LogWarning("FadePainel GameObject not found in the new scene!");
        }
    }

    public static void AddScore(int points)
    {
        if (instance == null) return;

        instance.currentScore += points;
        GameManager.onScoreChanged?.Invoke(instance.currentScore);
    }

    public static int GetScore()
    {
        if (instance == null) return 0;
        return instance.currentScore;
    }

    public static int GetJogadas()
    {
        if (instance == null) return 0;
        return instance.jogadas;
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("saiu");
    }

    public void ResetScore()
    {
        currentScore = initialScore;
        onScoreChanged?.Invoke(currentScore);
    }

    public void ResetJogadas()
    {
        jogadas = initialJogadas;
        onJogadasChanged?.Invoke(jogadas);
    }

    // Método para recarregar a cena atual

    public void RestartCurrentLevel()
    {
        Debug.Log("Restarting current level: " + SceneManager.GetActiveScene().name);
        StartCoroutine(FadeAndReloadCurrentScene());
    }

    private IEnumerator FadeAndReloadCurrentScene()
    {
        yield return StartCoroutine(FadeOut());
        ResetGameStates();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}