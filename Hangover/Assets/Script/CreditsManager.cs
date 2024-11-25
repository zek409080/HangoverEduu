using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    public float creditsDuration = 30f; // Duração dos créditos em segundos

    private void Start()
    {
        Invoke("ReturnToMenu", creditsDuration);
    }

    private void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu"); // Ajuste o nome da cena de menu principal
    }
}