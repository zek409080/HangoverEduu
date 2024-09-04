using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText, jogadasText, gameoverText;
    [SerializeField]Button buttonExit, buttonRestart, buttonclose, buttonConfig;
    [SerializeField]GameObject menuPanel;

    public void SetMenuPanel()
    {
        menuPanel.active = true;
        Time.timeScale = 0f;
    }

    public void OffMenuPanel()
    {
        menuPanel.active = false;
        Time.timeScale = 1f;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void UpdateTextGameOver(string textGameover)
    {
        gameoverText.text = textGameover;
        SetMenuPanel();
        buttonclose.enabled = false;
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
