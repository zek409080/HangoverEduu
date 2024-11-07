using UnityEngine;
using DG.Tweening;

public class Piece : MonoBehaviour
{
    public FrutType frutType;
    public int x;
    public int y;
    public bool isInvisible;
    public bool isMarkedForDestruction;
    public AudioSource somSelect;
    public GridManager gridManager { get; private set; }
    private Renderer pieceRenderer;
    private PieceSwapper pieceSwapper;
    private float idleTime;
    private float idleThreshold = 5.0f;

    private Color originalColor;
    private static readonly Color selectedColor = Color.gray;

    public delegate void PieceEventHandler(Piece piece);
    public event PieceEventHandler OnPieceDestruction;

    private void Awake()
    {
        pieceRenderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        pieceSwapper = FindObjectOfType<PieceSwapper>();
        somSelect = GetComponent<AudioSource>();

        if (gridManager == null)
        {
            Debug.LogError("GridManager not found in the scene. Please ensure there is a GridManager object in the scene.");
        }

        if (pieceSwapper == null)
        {
            Debug.LogError("PieceSwapper not found in the scene. Please ensure there is a PieceSwapper object in the scene.");
        }

        if (pieceRenderer != null)
        {
            originalColor = pieceRenderer.material.color;
        }
    }

    private void Update()
    {
        if (MusicUI.instance != null)
        {
            if (MusicUI.instance.estadoDoSom)
            {
                if (somSelect != null)
                {
                    somSelect.enabled = true;
                }
                else
                {
                    Debug.LogWarning("somSelect is null in Piece");
                }
            }
            else
            {
                if (somSelect != null)
                {
                    somSelect.enabled = false;
                }
                else
                {
                    Debug.LogWarning("somSelect is null in Piece");
                }
            }
        }
    }

    private void ProvideHint()
    {
        // Lógica para sugerir um movimento ao jogador
    }

    public virtual void Init(int x, int y, GridManager gridManager)
    {
        SetPosition(x, y);
        this.gridManager = gridManager;
        isMarkedForDestruction = false;
        isInvisible = false;
        SetVisibility(true);
    }

    public void SetPosition(int newX, int newY)
    {
        x = newX;
        y = newY;
    }

    public Vector2 GetPosition()
    {
        return new Vector2(x, y);
    }

    public void SetVisibility(bool isVisible)
    {
        isInvisible = !isVisible;
        if (pieceRenderer != null)
        {
            pieceRenderer.enabled = isVisible;
        }
    }

    public void MarkForDestruction()
    {
        if (!isMarkedForDestruction)
        {
            isMarkedForDestruction = true;
            SetVisibility(false);
        }
    }

    public virtual void AnimateDestruction()
    {
        if (isMarkedForDestruction)
        {
            gridManager.grid[x, y] = null;

            transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                OnPieceDestruction?.Invoke(this);
                Destroy(gameObject);
            });
        }
    }

    public void SetSelected(bool isSelected)
    {
        if (pieceRenderer != null)
        {
            pieceRenderer.material.color = isSelected ? selectedColor : originalColor;
        }
    }

    private void OnMouseDown()
    {
        if (!isInvisible && gridManager != null)
        {
            somSelect.Play();
            Debug.Log("Piece clicked for selection: " + name);
            pieceSwapper.SelectPiece(this);
        }
    }

    public virtual void OnSwap(Piece targetPiece)
    {
        // Método virtual para ser sobrescrito por subclasses específicas
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
    Vazio,
    Cereja,
    Roma,
    Amora
}