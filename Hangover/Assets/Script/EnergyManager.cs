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

    public delegate void EnergyChangedHandler(int currentEnergy);
    public event EnergyChangedHandler OnEnergyChanged;

    public delegate void TimeToNextRegenerationHandler(float time);
    public event TimeToNextRegenerationHandler OnTimeToNextRegenerationChanged;

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
        LoadEnergy();
        StartCoroutine(RegenerateEnergy());
        ReconfigurePopUp();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ReconfigurePopUp();
    }

    private void ReconfigurePopUp()
    {
        GameObject energyPopUp = GameObject.Find("EnergyPopUp");
        
        if (energyPopUp != null)
        {
            energyPopUp.SetActive(false);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void LoadEnergy()
    {
        currentEnergy = PlayerPrefs.GetInt("CurrentEnergy", maxEnergy);
        currentTimeToNextRegeneration = PlayerPrefs.GetFloat("CurrentTimeToNextRegeneration", regenerationTime);

        float elapsedTime = GetElapsedTimeInSeconds();

        while (elapsedTime > 0 && currentEnergy < maxEnergy)
        {
            if (elapsedTime >= currentTimeToNextRegeneration)
            {
                currentEnergy++;
                elapsedTime -= currentTimeToNextRegeneration;
                currentTimeToNextRegeneration = regenerationTime;
            }
            else
            {
                currentTimeToNextRegeneration -= elapsedTime;
                elapsedTime = 0;
            }
        }

        if (currentEnergy == maxEnergy)
        {
            currentTimeToNextRegeneration = regenerationTime;
        }

        PlayerPrefs.DeleteKey("LastExitTime");
        UpdateEnergyUI();
        OnEnergyChanged?.Invoke(currentEnergy);  // Garante atualização dos ícones ao carregar a energia
    }

    private float GetElapsedTimeInSeconds()
    {
        string lastExitTimeStr = PlayerPrefs.GetString("LastExitTime", "");
        if (string.IsNullOrEmpty(lastExitTimeStr))
        {
            return 0;
        }

        DateTime lastExitTime = DateTime.Parse(lastExitTimeStr);
        return (float)(DateTime.Now - lastExitTime).TotalSeconds;
    }

    private void SaveEnergy()
    {
        PlayerPrefs.SetInt("CurrentEnergy", currentEnergy);
        PlayerPrefs.SetFloat("CurrentTimeToNextRegeneration", currentTimeToNextRegeneration);
        PlayerPrefs.Save();
    }

    private IEnumerator RegenerateEnergy()
    {
        while (true)
        {
            if (currentEnergy < maxEnergy)
            {
                currentTimeToNextRegeneration -= Time.deltaTime;
                if (currentTimeToNextRegeneration <= 0)
                {
                    currentEnergy++;
                    currentTimeToNextRegeneration = regenerationTime;
                    UpdateEnergyUI();
                }

                OnTimeToNextRegenerationChanged?.Invoke(currentTimeToNextRegeneration);
                OnEnergyChanged?.Invoke(currentEnergy);  // Atualiza os ícones de energia
                SaveEnergy();
            }

            yield return null;
        }
    }

    private void UpdateEnergyUI()
    {
        if (energyText != null)
        {
            if (currentEnergy == maxEnergy)
            {
                energyText.text = $"Energy: {currentEnergy}/{maxEnergy}";
            }
            else
            {
                energyText.text = $"Energy: {currentEnergy}/{maxEnergy}\nTime to next regeneration: {currentTimeToNextRegeneration:F0}s";
            }
        }
        OnEnergyChanged?.Invoke(currentEnergy);
    }

    public void UseEnergy()
    {
        if (currentEnergy > 0)
        {
            currentEnergy--;
            UpdateEnergyUI();
            SaveEnergy();
        }
    }

    [ContextMenu("ResetEnergy")]
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
    }
}