using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    private StringBuilder _stringBuilder;

    private void Awake()
    {
        _stringBuilder = new StringBuilder();

        // Garantir que os painéis estejam desativados ao iniciar o jogo.
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
        ShowVictory("Victory");
    }

    public void UpdateObjectivesText()
    {
        if (objectiveManager != null)
        {
            objectivesText.text = "";
            foreach (var objective in objectiveManager.objectives)
            {
                if (!objective.isCompleted)
                {
                    if (objective.type == ObjectiveManager.ObjectiveType.Score)
                    {
                        objectivesText.text = $"Alcance {objective.targetValue} pontos.";
                        HideFruitImage();
                    }
                    else if (objective.type == ObjectiveManager.ObjectiveType.PieceCount)
                    {
                        int remaining = objective.targetValue - objectiveManager.pieceCounts[objective.targetPiece];
                        objectivesText.text = $"Colete mais {remaining} frutas.";
                        ShowFruitImage(objective.targetPiece);
                    }
                    break;
                }
            }
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
        GameManager.instance.StartJogadas();
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
        menuPanel.SetActive(false); // Assegura que o menu de pause não está visível
        PauseGame();
        buttonClose.enabled = false;

        // Atualizar pontuação final e high score
        int finalScore = GameManager.GetScore();
        int highScore = HighScoresManager.instance.GetHighScore(SceneManager.GetActiveScene().name);

        finalScoreText.text = "Final Score: " + finalScore;
        finalHighScoreText.text = "High Score: " + highScore;
    }

    public void ShowGameOver(string textGameOver)
    {
        gameoverText.text = textGameOver;
        menuPanel.SetActive(true);
        victoryPanel.SetActive(false); // Assegura que o painel de vitória não está visível
        PauseGame();
        buttonClose.enabled = false;

        // Atualizar pontuação final e high score
        int finalScore = GameManager.GetScore();
        int highScore = HighScoresManager.instance.GetHighScore(SceneManager.GetActiveScene().name);

        finalScoreText.text = "Final Score: " + finalScore;
        finalHighScoreText.text = "High Score: " + highScore;
    }

    public void UpdateJogadas(int jogadas)
    {
        _stringBuilder.Clear();
        _stringBuilder.Append(jogadas);
        jogadasText.text = _stringBuilder.ToString();
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

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    private void HideFruitImage()
    {
        if (objectiveManager.fruitImage != null)
        {
            objectiveManager.fruitImage.gameObject.SetActive(false);
        }
    }

    private void ShowFruitImage(FrutType frutType)
    {
        if (objectiveManager.fruitImage != null)
        {
            objectiveManager.fruitImage.gameObject.SetActive(true);
            objectiveManager.UpdateFruitImage(frutType);
        }
    }
}