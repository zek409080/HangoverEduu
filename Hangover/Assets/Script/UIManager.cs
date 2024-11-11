using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panel De Pause")]
    [SerializeField] TextMeshProUGUI scoreText, jogadasText;
    

    [Header("Panel De vitoria ou derrota")]
    [SerializeField] TextMeshProUGUI gameoverText;
    [SerializeField] GameObject panel_GameOver;

    public void RestartScene()
    {
        GameManager.instance.RestartGame();
    }

    public void QuitGame(string sceneName)
    {
        GameManager.instance.LoadScene(sceneName);
    }
    public void UpdateTextGameOver(string textGameover)
    {
        gameoverText.text = textGameover;
        panel_GameOver.SetActive(true);
        Time.timeScale = 0f;
    }
    public void UpdateJogadas(int jogadas)
    {
        jogadasText.text = jogadas.ToString(); ;
    }

    public void UpdateScore(int valueScoore)
    {
        scoreText.text = valueScoore.ToString();
    }
}
