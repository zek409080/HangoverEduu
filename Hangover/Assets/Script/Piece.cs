using UnityEngine;
using DG.Tweening;

public class Piece : MonoBehaviour
{
    public FrutType frutType;
    public int x;
    public int y;
    public GridManager gridManager;
    public bool isInvisible;
    public bool isMarkedForDestruction;

    private Renderer pieceRenderer;

    void Awake()
    {
        pieceRenderer = GetComponent<Renderer>();
    }

    public void Init(int x, int y, GridManager gridManager)
    {
        this.x = x;
        this.y = y;
        this.gridManager = gridManager;
        isInvisible = true;
        SetVisibility(isInvisible);
    }

    public void MarkForDestruction()
    {
        isMarkedForDestruction = true;
        SetVisibility(false); // Torna a peça invisível antes de destruí-la
    }

    public void AnimateDestruction()
    {
        transform.DOScale(Vector3.zero, 0.5f).OnComplete(() => Destroy(gameObject));
    }


    void OnMouseDown()
    {
        if (isInvisible)
        {
            gridManager.SelectPiece(this);
        }
    }

    public void SetVisibility(bool visible)
    {
        isInvisible = visible;
        pieceRenderer.enabled = visible;
    }
}

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