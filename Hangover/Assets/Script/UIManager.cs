using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    private void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        scoreText = GameObject.Find("Text_Score").GetComponent<TextMeshProUGUI>();
    }


    public void UpdateScore(int valueScoore)
    {
        scoreText.text = valueScoore.ToString();
    }
}
