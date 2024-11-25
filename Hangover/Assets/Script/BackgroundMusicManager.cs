using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackgroundMusicManager : MonoBehaviour
{
    #region Singleton
    public static BackgroundMusicManager instance;

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

    [SerializeField] private Toggle musicToggle;
    [SerializeField] private AudioSource fundoMusical;
    [SerializeField] private List<MusicTrack> menuMusics;
    [SerializeField] private List<MusicTrack> gameMusics;

    private bool estadoMusica = true;
    private int currentMenuMusicIndex = 0;
    private MusicTrack currentGameMusic;
    private System.Random random = new System.Random();
    private string lastSceneName;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeToggles();

        MenuManager.onFaseStarted += OnFaseStarted;

        lastSceneName = SceneManager.GetActiveScene().name;
        PlayAppropriateMusic(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeToggles();
        PlayAppropriateMusic(scene.name);
    }

    private void PlayAppropriateMusic(string sceneName)
    {
        if (IsInGameScene(sceneName))
        {
            if (!fundoMusical.isPlaying || !IsSameScene(sceneName))
            {
                PlayGameMusic();
            }
        }
        else
        {
            if (!fundoMusical.isPlaying || !IsSameScene(sceneName))
            {
                PlayMenuMusic();
            }
        }

        lastSceneName = sceneName;
    }

    private bool IsInGameScene(string sceneName)
    {
        return sceneName.StartsWith("Fase");
    }

    private bool IsSameScene(string sceneName)
    {
        return lastSceneName == sceneName;
    }

    private void InitializeToggles()
    {
        StartCoroutine(FindMusicToggle());
    }

    private IEnumerator FindMusicToggle()
    {
        yield return new WaitUntil(() => GameObject.Find("MusicOn") != null);

        musicToggle = GameObject.Find("MusicOn")?.GetComponent<Toggle>();

        if (musicToggle != null)
        {
            musicToggle.onValueChanged.RemoveAllListeners();
            musicToggle.isOn = estadoMusica; // inicializar estado do toggle
            musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
        }

        // Garantir que a música esteja no estado correto ao inicializar
        LigarOuDesligarMusica();
    }

    private void OnMusicToggleChanged(bool isOn)
    {
        estadoMusica = isOn;
        LigarOuDesligarMusica();
    }

    private void OnFaseStarted()
    {
        PlayGameMusic();
    }

    public void PlayGameMusic()
    {
        if (gameMusics.Count > 0)
        {
            fundoMusical.loop = true;
            currentGameMusic = gameMusics[random.Next(gameMusics.Count)];
            SetAndPlayMusic(currentGameMusic);
        }
    }

    public void PlayMenuMusic()
    {
        if (menuMusics.Count > 0)
        {
            fundoMusical.loop = false;
            currentMenuMusicIndex = 0;
            MusicTrack menuMusic = menuMusics[currentMenuMusicIndex];
            SetAndPlayMusic(menuMusic);
        }
    }

    private void SetAndPlayMusic(MusicTrack musicTrack)
    {
        fundoMusical.clip = musicTrack.clip;
        fundoMusical.volume = musicTrack.volume;

        if (estadoMusica && !fundoMusical.isPlaying)
        {
            fundoMusical.Play();
        }
    }

    private void LigarOuDesligarMusica()
    {
        if (estadoMusica)
        {
            fundoMusical.UnPause();
            if (!fundoMusical.isPlaying)
            {
                fundoMusical.Play();
            }
        }
        else
        {
            fundoMusical.Pause();
        }
    }

    private void Update()
    {
        if (!fundoMusical.isPlaying && !IsInGameScene(SceneManager.GetActiveScene().name))
        {
            currentMenuMusicIndex = (currentMenuMusicIndex + 1) % menuMusics.Count;
            SetAndPlayMusic(menuMusics[currentMenuMusicIndex]);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        MenuManager.onFaseStarted -= OnFaseStarted; // Desinscrever-se do evento
    }
}

[System.Serializable]
public class MusicTrack
{
    public AudioClip clip;
    public float volume = 1.0f;
}