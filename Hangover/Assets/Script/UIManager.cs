using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI jogadasText;
    [SerializeField] private TextMeshProUGUI gameoverText;
    [SerializeField] private Button buttonClose;
    [SerializeField] private GameObject menuPanel;
    private GameManager gameManager;
    [SerializeField] private TextMeshProUGUI objectivesText;
    private ObjectiveManager objectiveManager;


    private StringBuilder _stringBuilder;

    private void Awake()
    {
        _stringBuilder = new StringBuilder();
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
        // Lógica para quando os objetivos são completados, por exemplo, mostrar uma mensagem de vitória
        ShowGameOver("Parabéns! Você completou todos os objetivos!");
    }

    private void UpdateObjectivesText()
    {
        // Atualize a interface do usuário para refletir os objetivos atuais
        if (objectiveManager != null)
        {
            objectivesText.text = "";
            foreach (var objective in objectiveManager.objectives)
            {
                objectivesText.text += $"Objetivo: {objective.type} - Alvo: {objective.targetValue}\n";
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

    public void ShowGameOver(string textGameOver)
    {
        gameoverText.text = textGameOver;
        menuPanel.SetActive(true);
        PauseGame();
        buttonClose.enabled = false;
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
        scoreText.text = _stringBuilder.ToString();
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}