using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Text;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText; 
    [SerializeField] private TextMeshProUGUI jogadasText;
    [SerializeField] private TextMeshProUGUI highScoreText; 
    [SerializeField] private TextMeshProUGUI finalScoreText; 
    [SerializeField] private TextMeshProUGUI finalHighScoreText; 
    [SerializeField] private TextMeshProUGUI gameoverText; 
    [SerializeField] private TextMeshProUGUI victoryText;
    [SerializeField] private Button buttonClose;
    [SerializeField] private Button restartButton; // Adicionando referência ao botão de reinício
    [SerializeField] private GameObject menuPanel; 
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TextMeshProUGUI objectivesText;
    [SerializeField] private GameObject objectiveCompletedPopup;
    [SerializeField] private TextMeshProUGUI objectiveCompletedText;
    private GameManager gameManager;
    private ObjectiveManager objectiveManager;
    private StringBuilder _stringBuilder;
    private EnergyManager energyManager;

    private void Awake()
    {
        _stringBuilder = new StringBuilder();
        menuPanel.SetActive(false);
        victoryPanel.SetActive(false);
        
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartLevel);
        }
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
        // Podemos adicionar lógica adicional se necessário
    }

    private void RestartLevel()
    {
        GameManager.instance.RestartCurrentLevel();
    }
    
    public void UpdateObjectivesText()
    {
        if (objectiveManager != null)
        {
            _stringBuilder.Clear();
            foreach (var objective in objectiveManager.objectives)
            {
                string status = "";
                string objectiveTypeText = objective.type == ObjectiveManager.ObjectiveType.PieceCount ? "Coletar" : objective.type.ToString();

                if (objective.type == ObjectiveManager.ObjectiveType.Score)
                {
                    status = $"{GameManager.GetScore()}/{objective.targetValue}";
                }
                else if (objective.type == ObjectiveManager.ObjectiveType.PieceCount)
                {
                    status = $"{objectiveManager.GetPieceCount(objective.targetPiece)}/{objective.targetValue}";
                }

                _stringBuilder.AppendLine($"{objectiveTypeText}: {status} {(objective.isCompleted ? "(Completo)" : "")}");
            }
            objectivesText.text = _stringBuilder.ToString();
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

    public void QuitinGame(string sceneName)
    {
        EnergyManager.instance.UseEnergy();
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

    public void UpdateJogadas(int jogadas)
    {
        _stringBuilder.Clear();
        _stringBuilder.Append(jogadas);
        jogadasText.text = _stringBuilder.ToString();

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
            Invoke("HideObjectiveCompletedPopup", 2.0f);
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