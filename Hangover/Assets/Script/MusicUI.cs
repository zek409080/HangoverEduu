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

    private bool estadoDoSom = true;
    [SerializeField] private AudioSource fundoMusical;
    [SerializeField] private Sprite somLigado, somDesligado;

    public void LigarOuDesligarMusica()
    {
        estadoDoSom = !estadoDoSom;
        fundoMusical.enabled = estadoDoSom;

    }
}
