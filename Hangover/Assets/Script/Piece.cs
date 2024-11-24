using UnityEngine;
using DG.Tweening;

public class Piece : MonoBehaviour
{
    public FrutType frutType;
    public int x;
    public int y;
    public bool isInvisible;
    public bool isMarkedForDestruction;
    private AudioSource somSelect;
    public GridManager gridManager { get; private set; }
    private Renderer pieceRenderer;
    protected PieceSwapper pieceSwapper;
    private float idleTime;
    private float idleThreshold = 5.0f;

    private Color originalColor;
    private static readonly Color selectedColor = Color.gray;
    protected int scoreValue = 10;

    public delegate void PieceEventHandler(Piece piece);
    public event PieceEventHandler OnPieceDestruction;

    private void Awake()
    {
        pieceRenderer = GetComponent<Renderer>();
        somSelect = GetComponent<AudioSource>();

        if (somSelect == null)
        {
            Debug.LogWarning("AudioSource not found in Piece. Please ensure an AudioSource component is attached.");
        }
    }

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        pieceSwapper = FindObjectOfType<PieceSwapper>();

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
        UpdateSoundState();
    }

    private void UpdateSoundState()
    {
        if (PieceSoundManager.instance != null && somSelect != null)
        {
            somSelect.enabled = PieceSoundManager.instance.estadoSom;
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
                GameManager.AddScore(GetScoreValue());
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
            if (somSelect != null && somSelect.enabled)
            {
                somSelect.Play();
            }
            Debug.Log("Piece clicked for selection: " + name);
            pieceSwapper.SelectPiece(this);
        }
    }

    public virtual void OnSwap(Piece targetPiece)
    {
        // Método virtual para ser sobrescrito por subclasses específicas
    }
    
    // Adiciona pontuação para a destruição da peça
    public virtual int GetScoreValue()
    {
        return scoreValue;
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