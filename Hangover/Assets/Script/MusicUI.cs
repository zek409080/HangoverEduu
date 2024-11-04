using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MusicUI : MonoBehaviour
{
    

    #region Singleton

    public static MusicUI instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    #endregion
    Toggle music, som;
    [SerializeField] private GameObject musicContainer, somContainer;
    public bool estadoDomusica = true, estadoDoSom = false;
    [SerializeField] private AudioSource fundoMusical;

    private void Start()
    {
        Initialize();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Initialize()
    {
        if (fundoMusical == null)
        {
            GameObject musicaUI = GameObject.Find("MusicaUI");
            if (musicaUI != null)
            {
                fundoMusical = musicaUI.GetComponent<AudioSource>();
                fundoMusical.enabled = true;  
            }
        }

       
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Initialize();
    }

    private void Update()
    {
        if (musicContainer == null)
        {
            musicContainer = GameObject.Find("Panel_config");
        }


        if (musicContainer != null && musicContainer.activeSelf && music == null)
        {
            GameObject musicOn = GameObject.Find("MusicOn");
            if (musicOn != null)
            {
                music = musicOn.GetComponent<Toggle>();
                if (music != null)
                {
                    music.isOn = estadoDomusica; 
                    music.onValueChanged.AddListener(delegate { LigarOuDesligarMusica(); });
                }
            }
        }



        if (somContainer == null)
        {
            somContainer = GameObject.Find("Panel_config");
        }

        if (somContainer != null && somContainer.activeSelf && som == null)
        {
            GameObject somOn = GameObject.Find("SoundOn");
            if (somOn != null)
            {
                som = somOn.GetComponent<Toggle>();
                if (som != null)
                {
                    som.isOn = estadoDomusica;
                    som.onValueChanged.AddListener(delegate { LigarOuDesligarSom(); });
                }
            }
        }
    }
    public void LigarOuDesligarMusica()
    {
        estadoDomusica = !estadoDomusica;
        fundoMusical.enabled = estadoDomusica;
        
    }

    public void LigarOuDesligarSom()
    {
        estadoDoSom = !estadoDoSom;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}