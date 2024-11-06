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

    private StringBuilder _stringBuilder;

    private void Awake()
    {
        _stringBuilder = new StringBuilder();
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