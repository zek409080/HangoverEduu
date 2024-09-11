using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{

    public FrutType frutType; // Tipo da fruta da pe�a
    public int x; // Posi��o X da pe�a no tabuleiro
    public int y; // Posi��o Y da pe�a no tabuleiro
    public Board board; // Refer�ncia ao tabuleiro
    public bool isInvisible; // Determina se a pe�a � invis�vel

    public void Init(int x, int y, Board board)
    {
        this.x = x;
        this.y = y;
        this.board = board;
        SetVisibility(!isInvisible); // Define a visibilidade ao inicializar
    }



    void OnMouseDown()
    {
        if (!isInvisible && frutType != FrutType.Vazio) // Impede a sele��o de pe�as vazias
        {
            board.SelectPiece(this);
        }
    }

    public void SetVisibility(bool isVisible)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = isVisible;
        }
    }



    public void AnimateScale(Vector3 targetScale, float duration) // Anima a escala da pe�a
    {
        StartCoroutine(ScaleCoroutine(targetScale, duration)); // Inicia a rotina de anima��o de escala
    }

    private IEnumerator ScaleCoroutine(Vector3 targetScale, float duration) // Rotina de anima��o de escala
    {
        Vector3 startScale = transform.localScale; // Escala inicial da pe�a
        float time = 0; // Tempo de anima��o

        while (time < duration) // Enquanto o tempo de anima��o n�o atingir a dura��o
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, time / duration); // Interpola a escala da pe�a
            time += Time.deltaTime; // Incrementa o tempo com base no tempo real do jogo
            yield return null; // Aguarda o pr�ximo quadro
        }

        transform.localScale = targetScale; // Garante que a escala final seja exatamente a desejada
    }
}

// Enumera��o para os tipos de frutas dispon�veis
public enum FrutType
{
    Abacaxi,
    Banana,
    Manga,
    Maca,
    Melancia,
    Pinha,
    Uva,
    Poder,
    Obstacle,
    Vazio
}