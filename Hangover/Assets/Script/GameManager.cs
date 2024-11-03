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

    private UIManager managerUI;

    // Eventos disponíveis para outros componentes se inscreverem
    public static event System.Action<int> onScoreChanged;
    public static event System.Action<int> onJogadasChanged;
    
    private static int score = 0;
    private static int jogadas = 20;
    
    private void Start()
    {
        StartCoroutine(FadeIn());
        SceneManager.sceneLoaded += OnSceneLoaded;
        Initialize();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeBase();
        scorePlayer = initialScore;
        FindFaderCanvasGroup();
    }

    private void Initialize()
    {
        InitializeBase();
    }
    
    public static void DecrementJogadas()
    {
        jogadas--;
        onJogadasChanged?.Invoke(jogadas); // Usamos o operador null-conditional para invocar o evento se não for nulo

        if (jogadas <= 0)
        {
            TriggerGameOver();
        }
    }
    
    private static void TriggerGameOver()
    {
        // Lógica para Game Over
        // Pode ser alguma coisa como encontrar `UIManager` e mostrar o Game Over.
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
            Debug.LogError("UIManager not found in the scene. Please ensure there is a UIManager object in the scene.");
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
        StartCoroutine(FadeAndLoadScene(sceneName));
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(sceneName);
        yield return StartCoroutine(FadeIn());
    }

    private IEnumerator FadeOut()
    {
        if (faderCanvasGroup == null)
        {
            yield break;
        }

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
        if (faderCanvasGroup == null)
        {
            yield break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            faderCanvasGroup.alpha = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        faderCanvasGroup.blocksRaycasts = false;
    }

    private void FindFaderCanvasGroup()
    {
        faderCanvasGroup = GameObject.Find("FadePainel")?.GetComponent<CanvasGroup>();

        if (faderCanvasGroup == null)
        {
            Debug.LogWarning("FaderCanvasGroup not found in the new scene!");
        }
    }

    public static void AddScore(int points)
    {
        score += points;
        onScoreChanged?.Invoke(score); // Usamos o operador null-conditional para invocar o evento se não for nulo
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
        managerUI?.ShowGameOver(textGameover);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}