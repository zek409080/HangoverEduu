using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Text;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText; // Para exibir o score durante o jogo
    [SerializeField] private TextMeshProUGUI jogadasText;
    [SerializeField] private TextMeshProUGUI highScoreText; // Para exibir o high score durante o jogo
    [SerializeField] private TextMeshProUGUI finalScoreText; // Para exibir o score final no painel de vitória
    [SerializeField] private TextMeshProUGUI finalHighScoreText; // Para exibir o high score final no painel de vitória
    [SerializeField] private TextMeshProUGUI gameoverText; // Para exibir mensagem de game over
    [SerializeField] private TextMeshProUGUI victoryText; // Para exibir mensagem de vitória
    [SerializeField] private Button buttonClose;
    [SerializeField] private GameObject menuPanel; // Painel para menu/pause
    [SerializeField] private GameObject victoryPanel; // Painel para vitória
    private GameManager gameManager;
    [SerializeField] private TextMeshProUGUI objectivesText;
    private ObjectiveManager objectiveManager;
    [SerializeField] private GameObject objectiveCompletedPopup;
    [SerializeField] private TextMeshProUGUI objectiveCompletedText;

    private StringBuilder _stringBuilder;

    private void Awake()
    {
        _stringBuilder = new StringBuilder();
        menuPanel.SetActive(false);
        victoryPanel.SetActive(false);
    }

    private void Start()
    {
        objectiveManager = FindObjectOfType<ObjectiveManager>();
        if (objectiveManager != null)
        {
            objectiveManager.onObjectivesCompleted.AddListener(OnObjectivesCompleted);
            UpdateObjectivesText();
        }
        else
        {
            Debug.LogError("ObjectiveManager not found in the scene.");
        }
    }

    private void OnObjectivesCompleted()
    {
        // Podemos adicionar alguma lógica adicional se necessário
    }

    public void UpdateObjectivesText()
    {
        if (objectiveManager != null)
        {
            List<string> objectivesStatus = new List<string>();
            foreach (var objective in objectiveManager.objectives)
            {
                string status = objective.isCompleted ? "Completo" : "Em Progresso";
                objectivesStatus.Add($"{objective.type}: {objective.targetValue} ({status})");
            }
            objectivesText.text = string.Join("\n", objectivesStatus);
        }
    }

    private void OnEnable()
    {
        GameManager.onScoreChanged += UpdateScore;
        GameManager.onJogadasChanged += UpdateJogadas;
    }

    private void OnDisable()
    {
        GameManager.onScoreChanged -= UpdateScore;
        GameManager.onJogadasChanged -= UpdateJogadas;
    }

    public void RestartGame(string sceneName)
    {
        menuPanel.SetActive(false);
        victoryPanel.SetActive(false);
        ResumeGame();
        GameManager.instance.LoadScene(sceneName);
    }

    public void QuitGame(string sceneName)
    {
        GameManager.instance.LoadScene(sceneName);
    }

    public void ToggleMenu(bool isActive)
    {
        menuPanel.SetActive(isActive);
        if (isActive)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    public void ShowVictory(string victoryTextMessage)
    {
        victoryText.text = victoryTextMessage;
        victoryPanel.SetActive(true);
        menuPanel.SetActive(false);
        PauseGame();
        buttonClose.enabled = false;

        int finalScore = GameManager.GetScore();
        string currentLevel = SceneManager.GetActiveScene().name;
        HighScoresManager.instance.SetHighScore(currentLevel, finalScore);

        int highScore = HighScoresManager.instance.GetHighScore(currentLevel);
        finalScoreText.text = "Final Score: " + finalScore;
        finalHighScoreText.text = "High Score: " + highScore;
    }

    public void ShowGameOver(string textGameOver)
    {
        gameoverText.text = textGameOver;
        victoryPanel.SetActive(true);  // Ativar o painel de vitória ao invés do menu de pausa
        menuPanel.SetActive(false);    // Certifique-se de desativar o menu de pausa
        PauseGame();
        buttonClose.enabled = false;

        int finalScore = GameManager.GetScore();
        string currentLevel = SceneManager.GetActiveScene().name;
        HighScoresManager.instance.SetHighScore(currentLevel, finalScore);

        int highScore = HighScoresManager.instance.GetHighScore(currentLevel);
        finalScoreText.text = "Final Score: " + finalScore;
        finalHighScoreText.text = "High Score: " + highScore;
    }

    public void UpdateJogadas(int jogadas)
    {
        _stringBuilder.Clear();
        _stringBuilder.Append(jogadas);
        jogadasText.text = _stringBuilder.ToString();

        // Verifica se as jogadas terminaram
        if (jogadas <= 0)
        {
            CheckEndGame();
        }
    }

    public void UpdateScore(int score)
    {
        _stringBuilder.Clear();
        _stringBuilder.Append(score);
        if (scoreText != null)
        {
            scoreText.text = _stringBuilder.ToString();
        }

        if (highScoreText != null && HighScoresManager.instance != null)
        {
            int highScore = HighScoresManager.instance.GetHighScore(SceneManager.GetActiveScene().name);
            highScoreText.text = "High Score: " + highScore;
        }
    }

    private void CheckEndGame()
    {
        if (objectiveManager.AllObjectivesCompleted())
        {
            ShowGameOver("Parabéns! Você completou todos os objetivos.");
        }
        else
        {
            ShowGameOver("Game Over! Você não completou todos os objetivos.");
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void ShowObjectiveCompletedPopup(string message)
    {
        if (objectiveCompletedPopup != null && objectiveCompletedText != null)
        {
            objectiveCompletedPopup.SetActive(true);
            objectiveCompletedText.text = message;
            Invoke("HideObjectiveCompletedPopup", 2.0f); // Esconder o pop-up após 2 segundos
        }
    }

    private void HideObjectiveCompletedPopup()
    {
        if (objectiveCompletedPopup != null)
        {
            objectiveCompletedPopup.SetActive(false);
        }
    }

    public void ResetUI()
    {
        scoreText.text = "0";
        jogadasText.text = GameManager.GetJogadas().ToString();
        objectivesText.text = "";
        UpdateObjectivesText();
        finalScoreText.text = "";
        finalHighScoreText.text = "";
        victoryText.text = "";
        gameoverText.text = "";
    }
}