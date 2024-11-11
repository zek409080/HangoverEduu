using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject panelselectedFase;
    [SerializeField] TextMeshProUGUI highScoreText;
    private string selectedFase;

    void FindButtonFase()
    {
        GameObject.Find("Fase1_button").GetComponent<Button>().onClick.AddListener(() => SetSelectedFase(true, "Fase 1"));
        GameObject.Find("Fase2_button").GetComponent<Button>().onClick.AddListener(() => SetSelectedFase(true, "Fase 2"));
        GameObject.Find("Fase3_button").GetComponent<Button>().onClick.AddListener(() => SetSelectedFase(true, "Fase 3"));
        GameObject.Find("Fase4_button").GetComponent<Button>().onClick.AddListener(() => SetSelectedFase(true, "Fase 4"));
        GameObject.Find("Fase5_button").GetComponent<Button>().onClick.AddListener(() => SetSelectedFase(true, "Fase 5"));
        GameObject.Find("Fase6_button").GetComponent<Button>().onClick.AddListener(() => SetSelectedFase(true, "Fase 6"));
        GameObject.Find("Fase7_button").GetComponent<Button>().onClick.AddListener(() => SetSelectedFase(true, "Fase 7"));
        GameObject.Find("Fase8_button").GetComponent<Button>().onClick.AddListener(() => SetSelectedFase(true, "Fase 8"));
        GameObject.Find("Fase9_button").GetComponent<Button>().onClick.AddListener(() => SetSelectedFase(true, "Fase 9"));
        GameObject.Find("Fase10_button").GetComponent<Button>().onClick.AddListener(() => SetSelectedFase(true, "Fase 10"));
        GameObject.Find("Fase11_button").GetComponent<Button>().onClick.AddListener(() => SetSelectedFase(true, "Fase 11"));
        GameObject.Find("Fase12_button").GetComponent<Button>().onClick.AddListener(() => SetSelectedFase(true, "Fase 12"));
        GameObject.Find("Fase13_button").GetComponent<Button>().onClick.AddListener(() => SetSelectedFase(true, "Fase 13"));
        GameObject.Find("Fase14_button").GetComponent<Button>().onClick.AddListener(() => SetSelectedFase(true, "Fase 14"));
    }

    private void Start()
    {
        FindButtonFase();
    }

    private void SetSelectedFase(bool set, string fase)
    {
        panelselectedFase.SetActive(set);
        selectedFase = fase;

        // Exibir o high score para a fase selecionada
        int highScore = HighScoresManager.instance.GetHighScore(fase);
        highScoreText.text = "High Score: " + highScore;
    }

    public void ButtonStartPanel()
    {
        if (!string.IsNullOrEmpty(selectedFase))
        {
            GameManager.instance.LoadScene(selectedFase);
        }
    }
}