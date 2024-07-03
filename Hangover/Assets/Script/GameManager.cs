using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    /*#region Singleton

    public static GameManager instance;

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

    #endregion*/
   
    
    private void Start()
    {
        Initialize();
        SceneManager.sceneLoaded += Initialize;
    }
    private void Initialize()
    {
        InitializeBase();
    }

    private void Initialize(Scene scene, LoadSceneMode mode)
    {
        InitializeBase();
    }

    private void InitializeBase()
    {
        FindButtons();
        Time.timeScale = 1;
    }


    private void FindButtons()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            GameObject.Find("Play").GetComponent<Button>().onClick.AddListener(() => LoadScene("seleçãoDeFase"));
            GameObject.Find("Exit").GetComponent<Button>().onClick.AddListener(ExitGame);
        }

        else if (SceneManager.GetActiveScene().name == "seleçãoDeFase")
        {
            GameObject.Find("Return_button").GetComponent<Button>().onClick.AddListener(() => LoadScene("Menu"));
            FindButtonFase();
        }
        else if (SceneManager.GetActiveScene().name == "Jogo")
        {
            GameObject.Find("Play").GetComponent<Button>().onClick.AddListener(() => LoadScene("Jogo"));
            GameObject.Find("Exit").GetComponent<Button>().onClick.AddListener(() => LoadScene("Menu"));
        }
    }

    void FindButtonFase()
    {
        GameObject.Find("Fase1_button").GetComponent<Button>().onClick.AddListener(() => LoadScene("Fase 1"));
        GameObject.Find("Fase2_button").GetComponent<Button>().onClick.AddListener(() => LoadScene("Fase 2"));
        GameObject.Find("Fase3_button").GetComponent<Button>().onClick.AddListener(() => LoadScene("Fase 3"));
        GameObject.Find("Fase4_button").GetComponent<Button>().onClick.AddListener(() => LoadScene("Fase 4"));
        GameObject.Find("Fase5_button").GetComponent<Button>().onClick.AddListener(() => LoadScene("Fase 5"));
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
