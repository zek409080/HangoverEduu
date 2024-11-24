using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class MusicTrack
{
    public AudioClip clip;
    public float volume = 1.0f;
}
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

        // Inscrever-se no evento para detectar quando uma fase começa
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
        // Adicione lógica para determinar se a cena atual é uma cena de jogo
        // Exemplo:
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
            musicToggle.isOn = estadoMusica;
            musicToggle.onValueChanged.AddListener(delegate { LigarOuDesligarMusica(); });
        }
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

        if (!fundoMusical.isPlaying)
        {
            fundoMusical.Play();
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

    public void LigarOuDesligarMusica()
    {
        estadoMusica = !estadoMusica;
        fundoMusical.enabled = estadoMusica;

        if (estadoMusica && !fundoMusical.isPlaying)
        {
            fundoMusical.Play();
        }
        else if (!estadoMusica)
        {
            fundoMusical.Pause();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        MenuManager.onFaseStarted -= OnFaseStarted; // Desinscrever-se do evento
    }
}

