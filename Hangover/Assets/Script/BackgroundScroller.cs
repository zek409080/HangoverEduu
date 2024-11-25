using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroller : MonoBehaviour
{
    public float scrollSpeed = 0.5f; // Velocidade do movimento do fundo

    private RectTransform rectTransform;
    private Vector2 initialPosition;
    private float direction = 1.0f; // 1 para direita, -1 para esquerda
    private float movementRange;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.anchoredPosition;

        // Supondo que a imagem cubra mais do que a tela, calculando o tamanho do movimento
        movementRange = rectTransform.rect.width - Screen.width;
    }

    void Update()
    {
        float newPositionX = rectTransform.anchoredPosition.x + scrollSpeed * Time.deltaTime * direction;

        // Verifica se atingiu o corpo do movimento à direita 
        if (newPositionX > initialPosition.x + movementRange / 2)
        {
            newPositionX = initialPosition.x + movementRange / 2;
            direction = -1.0f; // Inverte a direção para esquerda
        }
        // Verifica se atingiu o final do movimento à esquerda
        else if (newPositionX < initialPosition.x - movementRange / 2)
        {
            newPositionX = initialPosition.x - movementRange / 2;
            direction = 1.0f; // Inverte a direção para direita
        }

        rectTransform.anchoredPosition = new Vector2(newPositionX, rectTransform.anchoredPosition.y);
    }
}