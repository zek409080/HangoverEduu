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
    public int scorePlayer;
    public float fadeDuration = 1f;

    [Header("UI Elements")]
    public CanvasGroup faderCanvasGroup;
    public Text scoreText;

    private UIManager managerUI;

    // Eventos disponíveis para outros componentes se inscreverem
    public static event System.Action<int> onScoreChanged;
    public static event System.Action<int> onJogadasChanged;
    
    private static int score = 0;
    private static int jogadas = 20;
    
    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Initialize();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void StartJogadas()
    {
        jogadas = 20;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeBase();
        scorePlayer = initialScore;
        FindFaderCanvasGroup();
        StartCoroutine(FadeIn());
    }

    private void Initialize()
    {
        InitializeBase();
    }
    
    public static void DecrementJogadas()
    {
        jogadas--;
        onJogadasChanged?.Invoke(jogadas); // Usamos o operador null-condicional para invocar o evento se não for nulo
        
        if (jogadas <= 0)
        {
            TriggerGameOver();
        }
    }
    
    private static void TriggerGameOver()
    {
        // Lógica para Game Over
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowGameOver("Game Over");
        }
    }

    private void InitializeBase()
    {
        FindButtons();
        managerUI = FindObjectOfType<UIManager>();
        if (managerUI == null)
        {
            Debug.LogWarning("UIManager not found in the scene. Please ensure there is a UIManager object in the scene.");
        }
        Time.timeScale = 1;
    }

    private void FindButtons()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Menu")
        {
            BindButton("Play", () => LoadScene("selecaoDeFase"));
            BindButton("Exit", ExitGame);
        }
        else if (sceneName == "selecaoDeFase")
        {
            BindButton("Return_button", () => LoadScene("Menu"));
            DisplayHighScores();
        }
        else if (sceneName == "Jogo")
        {
            BindButton("Play", () => LoadScene("Jogo"));
            BindButton("Exit", () => LoadScene("Menu"));
        }
    }

    private void BindButton(string buttonName, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObj = GameObject.Find(buttonName);
        if (buttonObj != null)
        {
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(action);
            }
            else
            {
                Debug.LogWarning($"Button component not found on {buttonName}.");
            }
        }
        else
        {
            Debug.LogWarning($"Button {buttonName} not found in the scene.");
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
        SceneManager.LoadScene(sceneName);
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

        faderCanvasGroup.blocksRaycasts = true; // Já que o fade pode estar ainda sendo usado para nova tela.

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

    public void CompleteLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        int highScore = PlayerPrefs.GetInt(sceneName + "_highscore", 0);

        // Verifica se a pontuação atual é maior que a maior pontuação salva
        if (score > highScore)
        {
            PlayerPrefs.SetInt(sceneName + "_highscore", score);
        }
        
        LoadScene("selecaoDeFase");
    }

    private void DisplayHighScores()
    {
        // Encontra todos os botões de fase e exibe a maior pontuação
        foreach (Button button in FindObjectsOfType<Button>())
        {
            string sceneName = button.name; // O nome do botão deve corresponder ao nome da cena
            int highScore = PlayerPrefs.GetInt(sceneName + "_highscore", 0);

            Text scoreText = button.gameObject.GetComponentInChildren<Text>();
            if (scoreText != null)
            {
                scoreText.text = "High Score: " + highScore;
            }
        }
    }

    public static void AddScore(int points)
    {
        score += points;
        onScoreChanged?.Invoke(score); // Usamos o operador null-condicional para invocar o evento se não for nulo

        // Atualiza a interface com a nova pontuação
        if (GameObject.Find("ScoreText") != null)
        {
            GameObject.Find("ScoreText").GetComponent<Text>().text = "Score: " + score;
        }
    }
    
    public static int GetScore()
    {
        return score;
    }
    
    public static int GetJogadas()
    {
        return jogadas;
    }

    public void UpdateJogadas(int jogadas)
    {
        onJogadasChanged?.Invoke(jogadas);
    }

    public void UpdateGameOver(string textGameover)
    {
        if (managerUI != null)
        {
            managerUI.ShowGameOver(textGameover);
        }
        else
        {
            Debug.LogWarning("UIManager not found in the scene. Cannot display game over.");
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}