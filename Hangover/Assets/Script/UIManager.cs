using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText, jogadasText;


    public void UpdateJogadas(int jogadas)
    {
        jogadasText.text = jogadas.ToString(); ;
    }

    public void UpdateScore(int valueScoore)
    {
        scoreText.text = valueScoore.ToString();
    }
}
