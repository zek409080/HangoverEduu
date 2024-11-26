using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class GridManager : MonoBehaviour
{
    public int width, height;
    public float moveDuration = 0.5f;
    public GameObject[] fruitPrefabs;
    public Piece[,] grid;
    public GameObject destructionEffectPrefab;
    [SerializeField] GameObject caixaFundo;
    private UIManager uiManager;
    private MatchManager matchManager;
    public PowerUpManager powerUpManager;
    private ObjectiveManager objectiveManager;

    private void Start()
    {
        grid = new Piece[width, height];
        InitializeGrid();
        
        matchManager = GetComponent<MatchManager>();
        powerUpManager = GetComponent<PowerUpManager>();

        matchManager.CheckAndClearMatchesAtStart();
        
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.UpdateJogadas(GameManager.GetJogadas());
            uiManager.UpdateScore(GameManager.GetScore());
        }
        
        GameManager.onScoreChanged += OnScoreChanged;
        GameManager.onJogadasChanged += OnJogadasChanged;
        
        objectiveManager = FindObjectOfType<ObjectiveManager>();
        if (objectiveManager == null)
        {
            Debug.LogError("ObjectiveManager not found in the scene.");
        }
    }

    private void OnDisable()
    {
        GameManager.onScoreChanged -= OnScoreChanged;
        GameManager.onJogadasChanged -= OnJogadasChanged;
    }

    private void OnScoreChanged(int newScore)
    {
        uiManager?.UpdateScore(newScore);
    }

    private void OnJogadasChanged(int newJogadas)
    {
        uiManager?.UpdateJogadas(newJogadas);
    }

    private void InitializeGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Piece newPiece = CreateNewPiece(x, y, true);
                grid[x, y] = newPiece;
                Instantiate(caixaFundo, new Vector3(x, y, 0), Quaternion.identity);
            }
        }
    }

    private Piece CreateNewPiece(int x, int y, bool animateFromTop = false)
    {
        GameObject piecePrefab = GetRandomPiecePrefab();
        Vector3 startPosition = animateFromTop ? new Vector3(x, height, 0) : new Vector3(x, y, 0);
        GameObject newPieceObj = Instantiate(piecePrefab, startPosition, Quaternion.identity);

        Piece newPiece = newPieceObj.GetComponent<Piece>();
        newPiece.Init(x, y, this);

        if (animateFromTop)
        {
            newPiece.transform.DOMove(new Vector3(x, y, 0), moveDuration);
        }

        return newPiece;
    }

    private GameObject GetRandomPiecePrefab()
    {
        return fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];
    }

    public void SwapPieces(Piece piece1, Piece piece2)
    {
        int tempX = piece1.x;
        int tempY = piece1.y;
        piece1.SetPosition(piece2.x, piece2.y);
        piece2.SetPosition(tempX, tempY);

        grid[piece1.x, piece1.y] = piece1;
        grid[piece2.x, piece2.y] = piece2;

        piece1.OnSwap(piece2);
        piece2.OnSwap(piece1);
    }

    public bool AreAdjacent(Piece piece1, Piece piece2)
    {
        int deltaX = Mathf.Abs(piece1.x - piece2.x);
        int deltaY = Mathf.Abs(piece1.y - piece2.y);
        return (deltaX == 1 && deltaY == 0) || (deltaX == 0 && deltaY == 1);
    }

    public void DestroyPiece(Piece piece)
    {
        if (piece == null) return;

        piece.MarkForDestruction();

        if (destructionEffectPrefab != null)
        {
            Instantiate(destructionEffectPrefab, piece.transform.position, Quaternion.identity);
        }

        piece.AnimateDestruction();
        grid[piece.x, piece.y] = null;
        Destroy(piece.gameObject);
    }

    public IEnumerator ResetMatching()
    {
        yield return new WaitForSeconds(0.5f); // Temporizador para animações intermediárias
        yield return StartCoroutine(ClearAndFillBoard());
    }

    public IEnumerator ClearAndFillBoard()
    {
        yield return StartCoroutine(ClearMatches());
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FillEmptySpaces());
    }

    public IEnumerator ClearMatches()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] != null && grid[x, y].isMarkedForDestruction)
                {
                    DestroyPiece(grid[x, y]);
                }
            }
        }
        yield return new WaitForSeconds(0.5f); // Temporizador para animações intermediárias
    }

    private IEnumerator FillEmptySpaces()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == null)
                {
                    for (int ny = y + 1; ny < height; ny++)
                    {
                        if (grid[x, ny] != null)
                        {
                            MovePiece(grid[x, ny], x, y);
                            break;
                        }
                    }
                }
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == null)
                {
                    Piece newPiece = CreateNewPiece(x, y, true);
                    while (matchManager.GetAllMatchesForPiece(newPiece).Count >= 3)
                    {
                        Destroy(newPiece.gameObject);
                        newPiece = CreateNewPiece(x, y, true);
                    }
                    grid[x, y] = newPiece;
                    newPiece.transform.DOMove(new Vector3(x, y, 0), moveDuration);
                }
            }
        }

        yield return new WaitForSeconds(moveDuration);
    }

    private void MovePiece(Piece piece, int newX, int newY)
    {
        grid[piece.x, piece.y] = null;
        piece.transform.DOMove(new Vector3(newX, newY, 0), moveDuration);
        piece.SetPosition(newX, newY);
        grid[newX, newY] = piece;
    }

    public bool CheckAndProcessMatches(Piece piece)
    {
        List<Piece> matches = matchManager.GetAllMatchesForPiece(piece);

        if (matches.Count >= 3)
        {
            if (matches.Count >= 4) powerUpManager.CreatePowerUp(piece);

            StartCoroutine(HandleMatches(matches));
            return true;
        }
        return false;
    }

    private IEnumerator HandleMatches(List<Piece> matches)
    {
        foreach (var match in matches)
        {
            GameManager.AddScore(10);
            match.MarkForDestruction();
            objectiveManager.AddScore(10);
            objectiveManager.AddPieceCount(match.frutType);
        
            // Criar power-up para matches de 4 peças
            if (matches.Count >= 4)
            {
                powerUpManager.CreatePowerUp(match);
            }
        }
        yield return StartCoroutine(ClearAndFillBoard());

        // Não decrementa jogadas aqui: GameManager.DecrementJogadas();

        // Verificar novas combinações depois que as peças caem
        yield return StartCoroutine(ResolveAllMatches());
    }

    public IEnumerator ResolveAllMatches()
    {
        bool hasMatches = false;
        do
        {
            hasMatches = false;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y] != null)
                    {
                        // Pegando todas as combinações (matches) na posição atual da grade
                        List<Piece> matches = matchManager.GetAllMatchesForPiece(grid[x, y]);
                        if (matches.Count >= 3)
                        {
                            hasMatches = true;
                            foreach (Piece match in matches)
                            {
                                // Marca a peça para destruição
                                match.MarkForDestruction();

                                // Adiciona pontuação ao GameManager
                                GameManager.AddScore(10);

                                // Atualiza o objetivo
                                objectiveManager.AddScore(10);
                                objectiveManager.AddPieceCount(match.frutType);
                            }

                            // Limpa o tabuleiro e preenche conforme os matchs são detectados
                            yield return StartCoroutine(ClearAndFillBoard());
                        }
                    }
                }
            }
            // Repetição do loop enquanto encontrarem matchs
        } while (hasMatches);
    }
    
    public void CheckEndGameConditions()
    {
        GameManager.CheckEndGameConditions(objectiveManager);
    }
}