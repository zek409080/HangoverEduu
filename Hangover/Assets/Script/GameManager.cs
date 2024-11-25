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
        else if (instance != this)
        {
            Destroy(gameObject);
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

        // Inicializa a aplicação na cena de Menu.
        LoadScene("Menu");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeScene();
        StartCoroutine(FadeIn());

        if (scene.name == "Menu")
        {
            ConfigureMenuScene();
        }
    }

    private void ConfigureMenuScene()
    {
        Button startButton = GameObject.Find("PlayGame").GetComponent<Button>();
        if (startButton != null)
        {
            startButton.onClick.AddListener(() =>
            {
                CheckAndLoadCutsceneOrSelection(); // Verifique se a cutscene foi vista antes de carregá-la ou não.
            });
        }
    }

    private void CheckAndLoadCutsceneOrSelection()
    {
        // Verificar se a cutscene já foi vista
        if (PlayerPrefs.HasKey("Cutscene1"))
        {
            // Se já foi vista, vá direto para a cena de seleção de fase
            LoadScene("selecaoDeFase");
        }
        else
        {
            // Caso contrário, vá para a cutscene
            LoadScene("Cutscene");
        }
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
        ObjectiveManager objectiveManager = FindObjectOfType<ObjectiveManager>();
        if (objectiveManager != null)
        {
            CheckEndGameConditions(objectiveManager);
        }
    }

    public static void CheckEndGameConditions(ObjectiveManager objectiveManager)
    {
        if (objectiveManager.AllObjectivesCompleted())
        {
            HandleWin();
        }
        else
        {
            HandleGameOver();
        }
    }

    public static void HandleWin()
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowVictory("Victory!");
        }

        int currentLevel = GetCurrentLevel();
        LevelManager.instance.UnlockNextLevel(currentLevel);
        Debug.Log("Level completed, attempting to unlock next level.");
    }

    public static void HandleGameOver()
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowGameOver("Game Over!");
        }

        if (EnergyManager.instance != null && EnergyManager.instance.HasEnergy())
        {
            EnergyManager.instance.UseEnergy();
            Debug.Log("Jogador falhou na fase e perdeu uma vida.");
        }
        else
        {
            Debug.LogWarning("Jogador tentou perder uma vida, mas não há energia disponível.");
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
        // Verifica se há energia suficiente antes de reiniciar
        if (EnergyManager.instance != null && EnergyManager.instance.HasEnergy())
        {
            // Consome uma unidade de energia
            EnergyManager.instance.UseEnergy();
            Debug.Log("Restarting current level: " + SceneManager.GetActiveScene().name);
            StartCoroutine(FadeAndReloadCurrentScene());
        }
        else
        {
            Debug.LogWarning("Not enough energy to restart the level.");
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                uiManager.ShowGameOver("Not enough energy to restart!");
            }
        }
    }

    private IEnumerator FadeAndReloadCurrentScene()
    {
        yield return StartCoroutine(FadeOut());
        ResetGameStates();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Adiciona um método auxiliar para obter o número da fase atual
    private static int GetCurrentLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        int level;
        if (int.TryParse(sceneName.Replace("Fase ", ""), out level))
        {
            return level;
        }
        return 1; // Retorna 1 se não conseguir parsear, como fallback
    }
}