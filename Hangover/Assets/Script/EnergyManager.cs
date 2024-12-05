using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class EnergyManager : MonoBehaviour
{
    public static EnergyManager instance;

    [Header("Energy Settings")]
    public int maxEnergy = 5;
    public float regenerationTime = 600f; // 10 minutos em segundos

    [Header("UI Elements")]
    public TextMeshProUGUI energyText;

    public event Action<int> OnEnergyChanged;
    public event Action<float> OnTimeToNextRegenerationChanged;

    public int currentEnergy;
    public float currentTimeToNextRegeneration;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Localiza o componente de texto se não estiver atribuído no Inspector
        if (energyText == null)
        {
            var energyTextObj = GameObject.Find("EnergyText");
            if (energyTextObj != null)
            {
                energyText = energyTextObj.GetComponent<TextMeshProUGUI>();
            }
        }

        LoadEnergy();
        RecalculateEnergyOnReturn();
        StartCoroutine(RegenerateEnergy());
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ReconfigurePopUp();
    }

    private void ReconfigurePopUp()
    {
        var energyPopUp = GameObject.Find("EnergyPopUp");
        if (energyPopUp != null)
        {
            energyPopUp.SetActive(false);
        }
    }

    private void LoadEnergy()
    {
        currentEnergy = PlayerPrefs.GetInt("CurrentEnergy", maxEnergy);
        currentTimeToNextRegeneration = PlayerPrefs.GetFloat("CurrentTimeToNextRegeneration", regenerationTime);

        UpdateEnergyUI();
    }

    private void SaveEnergy()
    {
        PlayerPrefs.SetInt("CurrentEnergy", currentEnergy);
        PlayerPrefs.SetFloat("CurrentTimeToNextRegeneration", currentTimeToNextRegeneration);
        PlayerPrefs.Save();
    }

    private void RecalculateEnergyOnReturn()
    {
        string lastExitTimeStr = PlayerPrefs.GetString("LastExitTime", "");
        if (!string.IsNullOrEmpty(lastExitTimeStr) && DateTime.TryParse(lastExitTimeStr, out DateTime lastExitTime))
        {
            float elapsedSeconds = (float)(DateTime.Now - lastExitTime).TotalSeconds;

            while (elapsedSeconds > 0 && currentEnergy < maxEnergy)
            {
                if (elapsedSeconds >= currentTimeToNextRegeneration)
                {
                    currentEnergy++;
                    elapsedSeconds -= currentTimeToNextRegeneration;
                    currentTimeToNextRegeneration = regenerationTime;
                }
                else
                {
                    currentTimeToNextRegeneration -= elapsedSeconds;
                    elapsedSeconds = 0;
                }
            }

            if (currentEnergy >= maxEnergy)
            {
                currentEnergy = maxEnergy;
                currentTimeToNextRegeneration = regenerationTime;
            }

            UpdateEnergyUI();
        }
    }

    private IEnumerator RegenerateEnergy()
    {
        while (true)
        {
            if (currentEnergy < maxEnergy)
            {
                if (currentTimeToNextRegeneration > 0)
                {
                    currentTimeToNextRegeneration -= 1f;
                }
                else
                {
                    currentEnergy++;
                    currentTimeToNextRegeneration = regenerationTime;

                    if (currentEnergy >= maxEnergy)
                    {
                        currentEnergy = maxEnergy;
                        currentTimeToNextRegeneration = regenerationTime;
                    }

                    UpdateEnergyUI();
                }

                OnTimeToNextRegenerationChanged?.Invoke(currentTimeToNextRegeneration);
                OnEnergyChanged?.Invoke(currentEnergy);
                SaveEnergy();
            }

            yield return new WaitForSeconds(1f); // Atualiza a cada segundo
        }
    }

    private void UpdateEnergyUI()
    {
        if (energyText != null)
        {
            energyText.text = currentEnergy == maxEnergy
                ? $"Energy: {currentEnergy}/{maxEnergy}"
                : $"Energy: {currentEnergy}/{maxEnergy}\nTime to next regeneration: {Mathf.Ceil(currentTimeToNextRegeneration)}s";
        }

        OnEnergyChanged?.Invoke(currentEnergy);
    }

    public void UseEnergy()
    {
        if (currentEnergy > 0)
        {
            currentEnergy--;

            // Atualiza a interface de energia e salva o estado
            UpdateEnergyUI();
            SaveEnergy();
        }
    }


    public void ResetEnergy()
    {
        currentEnergy = maxEnergy;
        currentTimeToNextRegeneration = regenerationTime;
        UpdateEnergyUI();
        SaveEnergy();
    }

    public bool HasEnergy()
    {
        return currentEnergy > 0;
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("LastExitTime", DateTime.Now.ToString());
        SaveEnergy();
        PlayerPrefs.Save();
    }
}
