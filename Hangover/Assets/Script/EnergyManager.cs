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
        UpdateEnergyUI();
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
                SaveEnergy();
            }

            yield return null;
        }
    }

    private void UpdateEnergyUI()
    {
        if (energyText != null)
        {
            energyText.text = $"Energy: {currentEnergy}/{maxEnergy}";
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
}