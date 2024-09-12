using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Piece : MonoBehaviour
{
    public FrutType frutType; // Tipo da fruta da peça
    public int x; // Posição X da peça no tabuleiro
    public int y; // Posição Y da peça no tabuleiro
    public Board board; // Referência ao tabuleiro
    public bool isInvisible; // Determina se a peça é invisível

    private Renderer pieceRenderer;

    void Awake()
    {
        // Obtém o componente Renderer apenas uma vez durante a inicialização
        pieceRenderer = GetComponent<Renderer>();
    }

    public void Init(int x, int y, Board board)
    {
        this.x = x;
        this.y = y;
        this.board = board;
        SetVisibility(!isInvisible); // Define a visibilidade ao inicializar
    }

    void OnMouseDown()
    {
        if (!isInvisible && frutType != FrutType.Vazio) // Impede a seleção de peças invisíveis ou vazias
        {
            board.SelectPiece(this);
        }
    }

    public void SetVisibility(bool isVisible)
    {
        if (pieceRenderer != null)
        {
            pieceRenderer.enabled = isVisible;
        }
    }

    public void AnimateScale(Vector3 targetScale, float duration)
    {
        // Usando DOTween para animação de escala
        transform.DOScale(targetScale, duration).SetEase(Ease.InOutQuad);
    }
}

// Enumeração para os tipos de frutas disponíveis
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
