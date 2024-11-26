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

    [Header("Configurações do Jogo")]
    public int initialScore = 0;
    public float fadeDuration = 1f;
    public int initialJogadas = 10;

    [Header("Elementos UI")]
    public CanvasGroup faderCanvasGroup;
    public Image faderImage;

    public static event System.Action<int> onScoreChanged;
    public static event System.Action<int> onJogadasChanged;

    private int currentScore;
    private int jogadas;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Verifique se devemos iniciar com o fade in
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            // Configurações iniciais para a primeira cena
            Initialize();
            StartCoroutine(FadeIn());
        }
        else
        {
            Initialize();
        }
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

        ConfigureQuitButton(); // Adiciona a configuração do botão QUITAR para cada cena carregada
    }
    
    public void StartButtonClicked()
    {
        CheckAndLoadCutsceneOrSelection();
    }

    private void ConfigureMenuScene()
    {
        Button startButton = GameObject.Find("PlayGame").GetComponent<Button>();
        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners(); // Remover listeners antigos se houver
            startButton.onClick.AddListener(StartButtonClicked); // Adicionar o novo método
        }
    }

    private void CheckAndLoadCutsceneOrSelection()
    {
        // Verificar se a cutscene 1 já foi vista
        if (PlayerPrefs.GetInt("Cutscene1", 0) == 1)
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
        FindFaderCanvasGroup();
        Time.timeScale = 1; // Garantir que o jogo está rodando no tempo normal
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
            uiManager.ShowVictory("Vitória!");
        }

        int currentLevel = GetCurrentLevel();
        LevelManager.instance.UnlockNextLevel(currentLevel);
        Debug.Log("Fase concluída, tentando desbloquear a próxima fase.");
    }

    public static void HandleGameOver()
    {
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowGameOver("Fim de Jogo!");
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
            Debug.LogError($"Cena '{sceneName}' não pode ser encontrada nas Configurações de Build.");
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
        Debug.Log("FadeAndLoadScene iniciado para " + sceneName);
        yield return StartCoroutine(FadeOut());

        Debug.Log("Cena está prestes a carregar: " + sceneName);
        ResetGameStates();
        SceneManager.LoadScene(sceneName);
        yield return StartCoroutine(FadeIn());

        Time.timeScale = 1; // Garantir rodagem do jogo no tempo normal
        Debug.Log("FadeAndLoadScene completado para " + sceneName);
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

    private IEnumerator FadeIn()
    {
        FindFaderCanvasGroup();
        if (faderCanvasGroup == null)
        {
            Debug.LogWarning("faderCanvasGroup não encontrado no FadeIn");
            yield break;
        }

        Debug.Log("FadeIn iniciado.");
        faderCanvasGroup.alpha = 1f;
        faderCanvasGroup.blocksRaycasts = true; // Inicialmente bloquear interações
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            faderCanvasGroup.alpha = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        faderCanvasGroup.alpha = 0f;
        faderCanvasGroup.blocksRaycasts = false; // Permitir interações
        Debug.Log("FadeIn completado.");
    }

    private IEnumerator FadeOut()
    {
        FindFaderCanvasGroup();
        if (faderCanvasGroup == null)
        {
            Debug.LogWarning("faderCanvasGroup não encontrado no FadeOut");
            yield break;
        }

        Debug.Log("FadeOut iniciado.");
        faderCanvasGroup.alpha = 0f;
        faderCanvasGroup.blocksRaycasts = true; // Bloquear interações
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            faderCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        faderCanvasGroup.alpha = 1f;
        Debug.Log("FadeOut completado.");
    }

    private void FindFaderCanvasGroup()
    {
        if (faderCanvasGroup == null)
        {
            GameObject fadePanel = GameObject.Find("FadePanel");

            if (fadePanel != null)
            {
                faderCanvasGroup = fadePanel.GetComponent<CanvasGroup>();
                faderImage = fadePanel.GetComponent<Image>();

                if (faderCanvasGroup == null)
                {
                    Debug.LogWarning("Componente CanvasGroup não encontrado no FadePanel.");
                }
            }
            else
            {
                Debug.LogWarning("GameObject FadePanel não encontrado na nova cena!");
            }
        }
    }

    private void ConfigureQuitButton()
    {
        Button quitButton = GameObject.Find("QUITAR")?.GetComponent<Button>();
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners(); // Remover listeners antigos se houver
            quitButton.onClick.AddListener(ExitGame); // Adicionar o método de saída de jogo
        }
    }

    public static void AddScore(int points)
    {
        if (instance == null) return;

        instance.currentScore += points;
        GameManager.onScoreChanged?.Invoke(instance.currentScore);

        // Atualiza a pontuação no ObjectiveManager
        ObjectiveManager objectiveManager = FindObjectOfType<ObjectiveManager>();
        if (objectiveManager != null)
        {
            objectiveManager.SetCurrentScore(instance.currentScore);
        }
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
            Debug.Log("Reiniciando a fase atual: " + SceneManager.GetActiveScene().name);
            StartCoroutine(FadeAndReloadCurrentScene());
        }
        else
        {
            Debug.LogWarning("Sem Energia!");
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                uiManager.ShowGameOver("Sem Energia!");
            }
        }
    }

    private IEnumerator FadeAndReloadCurrentScene()
    {
        yield return StartCoroutine(FadeOut());
        ResetGameStates();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        yield return StartCoroutine(FadeIn());
        Time.timeScale = 1; // Garantir rodagem do jogo no tempo normal após reiniciar

        Debug.Log("Cena recarregada: " + SceneManager.GetActiveScene().name);
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