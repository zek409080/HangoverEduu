using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public delegate void OnFaseStarted();
public class MenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject panelselectedFase;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private GameObject[] energyIcons;  // Array de ícones de energia
    [SerializeField] private TextMeshProUGUI regenerationTimeText;
    [SerializeField] private GameObject energyPopUp;

    public static event OnFaseStarted onFaseStarted;

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
        ConfigureButtons();

        // Atualiza os HUDs de energia e tempo inicial
        UpdateEnergyIcons(EnergyManager.instance.currentEnergy);
        UpdateRegenerationTime(EnergyManager.instance.currentTimeToNextRegeneration);

        // Conecta os eventos do EnergyManager
        EnergyManager.instance.OnEnergyChanged += UpdateEnergyIcons;
        EnergyManager.instance.OnTimeToNextRegenerationChanged += UpdateRegenerationTime;

        // Garante que o pop-up de energia comece invisível
        energyPopUp.SetActive(false);
    }

    private void OnDestroy()
    {
        if (EnergyManager.instance != null)
        {
            EnergyManager.instance.OnEnergyChanged -= UpdateEnergyIcons;
            EnergyManager.instance.OnTimeToNextRegenerationChanged -= UpdateRegenerationTime;
        }
    }

    private void ConfigureButtons()
    {
        for (int i = 0; i < faseNames.Length; i++)
        {
            GameObject buttonObject = GameObject.Find(faseNames[i]);
            if (buttonObject != null)
            {
                Button button = buttonObject.GetComponent<Button>();
                string faseSceneName = faseSceneNames[i];

                if (LevelManager.instance.IsLevelUnlocked(i + 1))
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
                Debug.LogWarning($"Botão '{faseNames[i]}' não encontrado na cena.");
            }
        }
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
            onFaseStarted?.Invoke();
            GameManager.instance.LoadScene(cena);
        }
        else
        {
            StartCoroutine(ShowEnergyPopUp());
        }
    }

    public void LoadMenu()
    {
        GameManager.instance.LoadScene("Menu");
    }

    private void SetSelectedFase(bool set, string fase)
    {
        panelselectedFase.SetActive(set);
        selectedFase = fase;

        int highScore = HighScoresManager.instance.GetHighScore(fase);
        highScoreText.text = $"High Score: {highScore}";
    }

    public void ButtonStartPanel()
    {
        if (!string.IsNullOrEmpty(selectedFase))
        {
            LoadCena(selectedFase);
        }
    }

    private IEnumerator ShowEnergyPopUp()
    {
        if (!energyPopUp.activeSelf)
        {
            energyPopUp.SetActive(true);
            yield return new WaitForSeconds(2f); // Tempo que o popup ficará visível
            energyPopUp.SetActive(false);
        }
    }

    public void UnlockAllLevels()
    {
        for (int i = 1; i <= faseNames.Length; i++)
        {
            LevelManager.instance.UnlockLevel(i);
        }
        ConfigureButtons(); // Atualiza os botões após desbloquear
    }
}
