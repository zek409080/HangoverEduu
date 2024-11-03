using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class GridManager : MonoBehaviour
{
    public int width, height;
    public float moveDuration = 0.5f;
    public GameObject[] fruitPrefabs;
    public GameObject cerejaPrefab;
    public GameObject romaPrefab;
    public GameObject amoraPrefab;
    public Piece[,] grid;

    // Variáveis de controle de jogadas e pontuação
    private UIManager uiManager;

    private void Start()
    {
        grid = new Piece[width, height];
        InitializeGrid();
        CheckAndClearMatchesAtStart();
        uiManager = FindObjectOfType<UIManager>();

        if (uiManager != null)
        {
            // Atualizar UI com valores iniciais
            uiManager.UpdateJogadas(GameManager.GetJogadas());
            uiManager.UpdateScore(GameManager.GetScore());
        }

        // Assinar eventos do GameManager
        GameManager.onScoreChanged += OnScoreChanged;
        GameManager.onJogadasChanged += OnJogadasChanged;
    }

    private void OnDisable()
    {
        // Desinscrever de eventos para evitar erros quando o objeto for destruído
        GameManager.onScoreChanged -= OnScoreChanged;
        GameManager.onJogadasChanged -= OnJogadasChanged;
    }

    private void OnScoreChanged(int newScore)
    {
        if (uiManager != null)
        {
            uiManager.UpdateScore(newScore);
        }
    }

    private void OnJogadasChanged(int newJogadas)
    {
        if (uiManager != null)
        {
            uiManager.UpdateJogadas(newJogadas);
        }
    }

    // Restante da implementação do GridManager

    private void InitializeGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Piece newPiece = CreateNewPiece(x, y, true);
                grid[x, y] = newPiece;
            }
        }
    }

    private Piece CreateNewPiece(int x, int y, bool animateFromTop = false)
    {
        GameObject piecePrefab = GetRandomPiecePrefab();
        Vector3 startPosition = animateFromTop ? new Vector3(x, height, 0) : new Vector3(x, y, 0);
        GameObject newPieceObj = Instantiate(piecePrefab, startPosition, Quaternion.identity);

        Piece newPiece = newPieceObj.GetComponent<Piece>();
        newPiece.SetPosition(x, y);

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

    private bool CheckInitialMatches(Piece piece)
    {
        List<Piece> matchedPieces = GetAllMatchesForPiece(piece);
        return matchedPieces.Count > 2;
    }

    private void CheckAndClearMatchesAtStart()
    {
        StartCoroutine(CheckAndClearMatchesCoroutine());
    }

    private IEnumerator CheckAndClearMatchesCoroutine()
    {
        bool matchesFound;
        do
        {
            matchesFound = false;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y] != null && CheckForMatchAt(x, y))
                    {
                        grid[x, y].MarkForDestruction();
                        matchesFound = true;
                    }
                }
            }
            if (matchesFound)
            {
                yield return StartCoroutine(ClearAndFillBoard());
            }
        } while (matchesFound);
    }

    private bool CheckForMatchAt(int x, int y)
    {
        return (CheckForMatchInDirection(grid[x, y], Vector2.left, 2) ||
                CheckForMatchInDirection(grid[x, y], Vector2.right, 2) ||
                CheckForMatchInDirection(grid[x, y], Vector2.up, 2) ||
                CheckForMatchInDirection(grid[x, y], Vector2.down, 2));
    }

    private List<Piece> GetMatch(Piece startPiece, Vector2 direction)
    {
        List<Piece> match = new List<Piece> { startPiece };
        FrutType frutType = startPiece.frutType;

        int nextX = startPiece.x + (int)direction.x;
        int nextY = startPiece.y + (int)direction.y;

        while (nextX >= 0 && nextX < width && nextY >= 0 && nextY < height)
        {
            Piece nextPiece = grid[nextX, nextY];
            if (nextPiece != null && nextPiece.frutType == frutType)
            {
                match.Add(nextPiece);
                nextX += (int)direction.x;
                nextY += (int)direction.y;
            }
            else
            {
                break;
            }
        }

        return match;
    }

    private bool CheckForMatchInDirection(Piece piece, Vector2 direction, int length)
    {
        List<Piece> matchingPieces = new List<Piece> { piece };
        for (int i = 1; i <= length; i++)
        {
            int checkX = piece.x + (int)direction.x * i;
            int checkY = piece.y + (int)direction.y * i;
            if (checkX < 0 || checkX >= width || checkY < 0 || checkY >= height)
                break;
            if (grid[checkX, checkY] != null && grid[checkX, checkY].frutType == piece.frutType)
                matchingPieces.Add(grid[checkX, checkY]);
            else
                break;
        }
        return matchingPieces.Count > length;
    }

    private List<Piece> GetAllMatchesForPiece(Piece piece)
    {
        List<Piece> horizontalMatches = GetMatches(piece, +1, 0).ToList();
        horizontalMatches.AddRange(GetMatches(piece, -1, 0).Where(p => p != piece));

        List<Piece> verticalMatches = GetMatches(piece, 0, +1).ToList();
        verticalMatches.AddRange(GetMatches(piece, 0, -1).Where(p => p != piece));

        List<Piece> allMatches = new List<Piece>();

        if (horizontalMatches.Count >= 3) allMatches.AddRange(horizontalMatches);
        if (verticalMatches.Count >= 3) allMatches.AddRange(verticalMatches);

        return allMatches.Distinct().ToList();
    }

    private List<Piece> GetMatches(Piece piece, int dx, int dy)
    {
        List<Piece> matchingPieces = new List<Piece> { piece };

        int newX = piece.x;
        int newY = piece.y;

        while (true)
        {
            newX += dx;
            newY += dy;

            if (newX < 0 || newX >= width || newY < 0 || newY >= height) break;

            Piece nextPiece = grid[newX, newY];
            if (nextPiece != null && nextPiece.frutType == piece.frutType)
            {
                matchingPieces.Add(nextPiece);
            }
            else
            {
                break;
            }
        }

        return matchingPieces;
    }

    private IEnumerator ClearAndFillBoard()
    {
        yield return StartCoroutine(ClearMatches());
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FillEmptySpaces());
    }

    private IEnumerator ClearMatches()
    {
        // Destruir peças marcadas para destruição
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
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator FillEmptySpaces()
    {
        // Fazer as peças caírem nos espaços vazios
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

        // Criar novas peças para os espaços restantes
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == null)
                {
                    Piece newPiece = CreateNewPiece(x, y, true);
                    while (CheckInitialMatches(newPiece))
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

    public bool CheckAndProcessMatches(Piece piece1, Piece piece2)
    {
        List<Piece> match1 = GetAllMatchesForPiece(piece1);
        List<Piece> match2 = GetAllMatchesForPiece(piece2);

        if (match1.Count >= 3 || match2.Count >= 3)
        {
            if (match1.Count >= 4) CreatePowerUp(piece1);
            if (match2.Count >= 4) CreatePowerUp(piece2);

            if (match1.Count >= 3) StartCoroutine(HandleMatches(match1));
            if (match2.Count >= 3) StartCoroutine(HandleMatches(match2));
            return true;
        }
        return false;
    }

    private IEnumerator HandleMatches(List<Piece> matches)
    {
        foreach (var match in matches)
        {
            GameManager.AddScore(10); // Atualiza a pontuação
            match.MarkForDestruction();
        }
        yield return StartCoroutine(ClearAndFillBoard());
        GameManager.DecrementJogadas(); // Decrementa jogadas ao processar matches
    }

    public void SwapPieces(Piece piece1, Piece piece2)
    {
        int tempX = piece1.x;
        int tempY = piece1.y;
        piece1.SetPosition(piece2.x, piece2.y);
        piece2.SetPosition(tempX, tempY);

        grid[piece1.x, piece1.y] = piece1;
        grid[piece2.x, piece2.y] = piece2;

        piece1.OnSwap(piece2);  // Chama OnSwap para a peça 1
        piece2.OnSwap(piece1);  // Chama OnSwap para a peça 2
    }

    public bool AreAdjacent(Piece piece1, Piece piece2)
    {
        int deltaX = Mathf.Abs(piece1.x - piece2.x);
        int deltaY = Mathf.Abs(piece1.y - piece2.y);
        return (deltaX == 1 && deltaY == 0) || (deltaX == 0 && deltaY == 1);
    }

    // Método para criar um Power-up aleatório (Cereja, Roma ou Amora)
    private void CreatePowerUp(Piece piece)
    {
        GameObject powerUpPrefab = GetRandomPowerUpPrefab();
        Vector3 position = new Vector3(piece.x, piece.y, 0);
        GameObject powerUpObj = Instantiate(powerUpPrefab, position, Quaternion.identity);

        Piece powerUpPiece = powerUpObj.GetComponent<Piece>();
        powerUpPiece.Init(piece.x, piece.y, this);
        grid[piece.x, piece.y] = powerUpPiece;

        Destroy(piece.gameObject);
    }

    private GameObject GetRandomPowerUpPrefab()
    {
        GameObject[] powerUps = { cerejaPrefab, romaPrefab, amoraPrefab };
        int randomIndex = Random.Range(0, powerUps.Length);
        return powerUps[randomIndex];
    }

    // Métodos para os Power-ups: cereja, roma e amora

    public void ActivateCereja(Piece cereja)
    {
        if (cereja == null || cereja.isMarkedForDestruction) return;
        Debug.Log("Ativando PowerUp Cereja");

        int explosionRadius = 2;  // Expansão de 2 peças em todas as direções, resultando em um quadrado 5x5

        for (int dx = -explosionRadius; dx <= explosionRadius; dx++)
        {
            for (int dy = -explosionRadius; dy <= explosionRadius; dy++)
            {
                int newX = cereja.x + dx;
                int newY = cereja.y + dy;

                if (IsWithinBounds(newX, newY))
                {
                    Piece neighbor = grid[newX, newY];
                    if (neighbor != null && !neighbor.isMarkedForDestruction)
                    {
                        neighbor.MarkForDestruction();
                        neighbor.AnimateDestruction();
                    }
                }
            }
        }
        StartCoroutine(ResetMatching());
    }

    // Função utilitária para verificar se uma posição está dentro dos limites da grade
    private bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public void ActivateRoma(Piece roma)
    {
        if (roma == null || roma.isMarkedForDestruction) return;
        Debug.Log("Ativando PowerUp Roma");

        // Destruir peças em toda a linha e coluna da peça roma
        for (int x = 0; x < width; x++)
        {
            if (grid[x, roma.y] != null && !grid[x, roma.y].isMarkedForDestruction)
            {
                grid[x, roma.y].MarkForDestruction();
                grid[x, roma.y].AnimateDestruction();
            }
        }

        for (int y = 0; y < height; y++)
        {
            if (grid[roma.x, y] != null && !grid[roma.x, y].isMarkedForDestruction)
            {
                grid[roma.x, y].MarkForDestruction();
                grid[roma.x, y].AnimateDestruction();
            }
        }

        StartCoroutine(ResetMatching());
    }

    public void ActivateAmora(Piece amora, Piece targetPiece)
    {
        if (amora == null || amora.isMarkedForDestruction || targetPiece == null) return;
        Debug.Log("Ativando PowerUp Amora");

        // Destruir todas as peças do mesmo tipo que a peça alvo
        foreach (Piece piece in grid)
        {
            if (piece != null && piece.frutType == targetPiece.frutType && !piece.isMarkedForDestruction)
            {
                piece.MarkForDestruction();
                piece.AnimateDestruction();
            }
        }

        // Destruir a própria Amora por ela mesma
        DestroyPiece(amora);

        StartCoroutine(ResetMatching());
    }

    // Função para resetar as combinações após a ativação do PowerUp
    private IEnumerator ResetMatching()
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(ClearAndFillBoard()); // Garantir que a grade seja atualizada
    }

    // Função para destruir uma peça
    public void DestroyPiece(Piece piece)
    {
        if (piece == null) return;
        piece.MarkForDestruction();
        piece.AnimateDestruction();
        grid[piece.x, piece.y] = null;
        Destroy(piece.gameObject);
    }

    // Função para obter os vizinhos de uma peça
    private List<Piece> GetNeighbors(int x, int y)
    {
        List<Piece> neighbors = new List<Piece>();

        if (x > 0) neighbors.Add(grid[x - 1, y]);
        if (x < width - 1) neighbors.Add(grid[x + 1, y]);
        if (y > 0) neighbors.Add(grid[x, y - 1]);
        if (y < height - 1) neighbors.Add(grid[x, y + 1]);

        return neighbors;
    }
   
}