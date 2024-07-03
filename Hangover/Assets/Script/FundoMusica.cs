using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FundoMusica : MonoBehaviour
{
    private bool estadoDoSom = true;
    [SerializeField] private AudioSource fundoMusical;
    [SerializeField] private Sprite somLigado, somDesligado;
    [SerializeField] private AudioSource fundoMusica;

    public void LigarOuDesligarMusica()
    {
        estadoDoSom = !estadoDoSom;
        fundoMusical.enabled = estadoDoSom;

    }
}
