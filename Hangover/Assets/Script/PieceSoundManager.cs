using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PieceSoundManager : MonoBehaviour
{
    #region Singleton
    public static PieceSoundManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion

    [SerializeField] private Toggle soundToggle;
    public bool estadoSom = true;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeToggles();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeToggles();
    }

    private void InitializeToggles()
    {
        StartCoroutine(FindSoundToggle());
    }

    private IEnumerator FindSoundToggle()
    {
        yield return new WaitUntil(() => GameObject.Find("SoundOn") != null);

        soundToggle = GameObject.Find("SoundOn")?.GetComponent<Toggle>();

        if (soundToggle != null)
        {
            soundToggle.onValueChanged.RemoveAllListeners();
            soundToggle.isOn = estadoSom; // inicializar estado do toggle
            soundToggle.onValueChanged.AddListener(OnSoundToggleChanged);
        }

        // Garantir que o som esteja no estado correto ao inicializar
        LigarOuDesligarSom();
    }

    private void OnSoundToggleChanged(bool isOn)
    {
        estadoSom = isOn;
        LigarOuDesligarSom();
    }

    public void LigarOuDesligarSom()
    {
        AudioListener.pause = !estadoSom;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}