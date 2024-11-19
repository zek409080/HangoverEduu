using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject panelselectedFase;
    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] GameObject[] energyIcons;  // Array de ícones de energia
    [SerializeField] TextMeshProUGUI regenerationTimeText;
    [SerializeField] GameObject energyPopUp;

    private string selectedFase;

    private readonly string[] faseNames = new string[]
    {
        "Fase1_button", "Fase2_button", "Fase3_button", "Fase4_button", 
        "Fase5_button", "Fase6_button", "Fase7_button", "Fase8_button", 
        "Fase9_button", "Fase10_button", "Fase11_button", "Fase12_button", 
        "Fase13_button", "Fase14_button"
    };

    private readonly string[] faseSceneNames = new string[]
    {
        "Fase 1", "Fase 2", "Fase 3", "Fase 4", "Fase 5", 
        "Fase 6", "Fase 7", "Fase 8", "Fase 9", "Fase 10", 
        "Fase 11", "Fase 12", "Fase 13", "Fase 14"
    };

    private void Start()
    {
        FindButtonFase();
        UpdateEnergyIcons(EnergyManager.instance.currentEnergy);
        UpdateRegenerationTime(EnergyManager.instance.currentTimeToNextRegeneration);

        EnergyManager.instance.OnEnergyChanged += UpdateEnergyIcons;
        EnergyManager.instance.OnTimeToNextRegenerationChanged += UpdateRegenerationTime;
        energyPopUp.SetActive(false);
    }

    private void OnDestroy()
    {
        EnergyManager.instance.OnEnergyChanged -= UpdateEnergyIcons;
        EnergyManager.instance.OnTimeToNextRegenerationChanged -= UpdateRegenerationTime;
    }

    private void UpdateEnergyIcons(int currentEnergy)
    {
        for (int i = 0; i < energyIcons.Length; i++)
        {
            energyIcons[i].SetActive(i < currentEnergy);
        }
    }

    private void UpdateRegenerationTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        regenerationTimeText.text = $"Próxima Energia: {minutes:D2}:{seconds:D2}";
    }

    public void LoadCena(string cena)
    {
        if (EnergyManager.instance.HasEnergy())
        {
            GameManager.instance.LoadScene(cena);
        }
        else
        {
            StartCoroutine(ShowEnergyPopUp());
        }
    }


    public void OpenSite(string url)
    {
        Application.OpenURL(url);
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
            LoadCena(selectedFase);
        }
    }

    private void FindButtonFase()
    {
        for (int i = 0; i < faseNames.Length; i++)
        {
            GameObject buttonObject = GameObject.Find(faseNames[i]);
            if (buttonObject != null)
            {
                string faseSceneName = faseSceneNames[i];
                Button button = buttonObject.GetComponent<Button>();
                if (LevelManager.instance.IsLevelUnlocked(faseSceneName))
                {
                    button.onClick.AddListener(() => SetSelectedFase(true, faseSceneName));
                    button.interactable = true;
                }
                else
                {
                    button.interactable = false;
                }
            }
            else
            {
                Debug.LogWarning($"Botão '{faseNames[i]}' não encontrado");
            }
        }
    }
    
    public IEnumerator ShowEnergyPopUp()
    {
        energyPopUp.SetActive(true);
        yield return new WaitForSeconds(2f); // Tempo que o popup ficará visível
        energyPopUp.SetActive(false);
    }
    
}