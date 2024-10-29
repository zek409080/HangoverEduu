using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    
    [Header("Tamanho do tabuleiro")]
    public int width;
    public int height;

    [SerializeField] int jogadas;
    public GameObject[] piecePrefab;
    public Piece[,] pieces;
    private Piece selectedPiece;
    private bool canSwap = true;
    public Transform cam;
    [SerializeField] GameObject particle_popMagic, caixaDaGrid;

    private binaryArrayBoard binaryArray;

    void Start()
    {
        pieces = new Piece[width, height];
        binaryArray = GetComponent<binaryArrayBoard>();
        InitializeBoard();
        CenterCamera();
    }

    void CenterCamera()
    {
        cam.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
    }

    private void Update()
    {
        if (GameManager.instance.jogadas == 0)
        {
            StartCoroutine(GameOver());
        }
    }

    void InitializeBoard()
    {
        bool[] initialBools = binaryArray.GetInitialBools();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int index = x + y * width;
                if (index < initialBools.Length && initialBools[index])
                {
                    pieces[x, y] = CreateEmptyPiece(x, y);
                }
                else
                {
                    SpawnPiece(x, y);
                    Instantiate(caixaDaGrid, new Vector3(x, y, 0), Quaternion.identity);
                }
            }
        }

        CheckForMatches(out _);
    }

    Piece CreateEmptyPiece(int x, int y)
    {
        GameObject emptyObject = new GameObject("EmptyPiece");
        Piece emptyPiece = emptyObject.AddComponent<Piece>();
        emptyPiece.frutType = FrutType.Vazio;
        emptyPiece.Init(x, y, this);
        emptyPiece.SetVisibility(false);
        return emptyPiece;
    }

    void SpawnPiece(int x, int y)
    {
        GameObject newPiece = Instantiate(piecePrefab[RandomFrut()], new Vector3(x, y, 0), Quaternion.identity);
        pieces[x, y] = newPiece.GetComponent<Piece>();
        pieces[x, y]?.Init(x, y, this);
    }

    int RandomFrut()
    {
        return Random.Range(0, piecePrefab.Length);
    }

    public void SelectPiece(Piece piece)
    {
        if (!canSwap || selectedPiece == piece) return;

        if (selectedPiece == null)
        {
            selectedPiece = piece;
            selectedPiece.StartScaleAnimation(new Vector2(0.8f, 0.8f) * 1.2f, 0.2f); // Cresce a peça
        }
        else
        {
            if (IsAdjacent(selectedPiece, piece))
            {
                selectedPiece.StartScaleAnimation(new Vector2(0.8f, 0.8f), 0.3f); // Volta ao tamanho normal
                piece.StartScaleAnimation(new Vector2(0.8f, 0.8f) * 1.2f, 0.2f); // Cresce a nova peça
                StartCoroutine(TrySwapPieces(selectedPiece, piece));
            }
            else
            {
                selectedPiece.StartScaleAnimation(new Vector2(0.8f, 0.8f), 0.3f);
                selectedPiece = piece;
                selectedPiece.StartScaleAnimation(new Vector2(0.8f, 0.8f) * 1.2f, 0.2f);
            }
        }
    }

    bool IsAdjacent(Piece piece1, Piece piece2)
    {
        return (Mathf.Abs(piece1.x - piece2.x) == 1 && piece1.y == piece2.y) ||
               (Mathf.Abs(piece1.y - piece2.y) == 1 && piece1.x == piece2.x);
    }

    IEnumerator TrySwapPieces(Piece piece1, Piece piece2)
    {
        canSwap = false;

        // Guarda as posições originais
        Vector3 originalPosition1 = piece1.transform.position;
        Vector3 originalPosition2 = piece2.transform.position;

        // Troca de posição inicial
        SwapPieces(piece1, piece2);

        yield return new WaitForSeconds(0.15f); // Alinha com o tempo de troca

        // Verifica se a troca gerou um match
        if (!HasMatches())
        {
            // Se não houve match, volta para as posições originais
            piece1.StartMoveAnimation(originalPosition1, 0.1f); // Retorna para posição inicial
            piece2.StartMoveAnimation(originalPosition2, 0.1f); // Retorna para posição inicial

            yield return new WaitForSeconds(0.15f); // Tempo de retorno ao original
        }
        else
        {
            // Se houve match, processa as peças e verifica novos matches
            CheckForMatches(out _);
        }

        GameManager.instance.UpdateJogadas(-1);
        selectedPiece = null;
        canSwap = true;
    }


    void SwapPieces(Piece piece1, Piece piece2)
    {
        (pieces[piece1.x, piece1.y], pieces[piece2.x, piece2.y]) = (piece2, piece1);
        piece1.Init(piece2.x, piece2.y, this);
        piece2.Init(piece1.x, piece1.y, this);

        Vector3 tempPosition = piece1.transform.position;
        piece1.StartMoveAnimation(piece2.transform.position, 0.1f);
        piece2.StartMoveAnimation(tempPosition, 0.1f);
    }

    IEnumerator RefillBoard()
    {
        yield return new WaitForSeconds(0.5f);

        bool[] initialBools = binaryArray.GetInitialBools();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int index = x + y * width;
                if (pieces[x, y] == null && (index >= initialBools.Length || !initialBools[index]))
                {
                    for (int k = y + 1; k < height; k++)
                    {
                        if (pieces[x, k] != null)
                        {
                            MovePiece(pieces[x, k], new Vector3(x, y, 0), 0.3f);
                            pieces[x, k].Init(x, y, this);
                            pieces[x, y] = pieces[x, k];
                            pieces[x, k] = null;
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
                int index = x + y * width;
                if (pieces[x, y] == null && (index >= initialBools.Length || !initialBools[index]))
                {
                    GameObject newPiece = Instantiate(piecePrefab[RandomFrut()], new Vector3(x, height, 0), Quaternion.identity);
                    pieces[x, y] = newPiece.GetComponent<Piece>();
                    pieces[x, y]?.Init(x, y, this);
                    pieces[x, y].StartMoveAnimation(new Vector3(x, y, 0), 0.3f);
                }
            }
        }
        CheckForMatches(out _);
    }

    bool CheckMatchHorizontal(int x, int y)
    {
        FrutType currentType = pieces[x, y].frutType;
        int count = 1;

        for (int i = x + 1; i < width && pieces[i, y]?.frutType == currentType; i++)
        {
            count++;
        }

        return count >= 3;
    }

    bool CheckMatchVertical(int x, int y)
    {
        FrutType currentType = pieces[x, y].frutType;
        int count = 1;

        for (int i = y + 1; i < height && pieces[x, i]?.frutType == currentType; i++)
        {
            count++;
        }

        return count >= 3;
    }
    List<Piece> CheckMatchLine(int x, int y, int dx, int dy)
    {
        List<Piece> matched = new List<Piece>();
        FrutType type = pieces[x, y].frutType;
        matched.Add(pieces[x, y]);

        for (int i = 1; i < 3; i++)
        {
            int newX = x + dx * i;
            int newY = y + dy * i;

            if (newX >= width || newY >= height || pieces[newX, newY]?.frutType != type) break;
            matched.Add(pieces[newX, newY]);
        }

        return matched;
    }
    List<Piece> GetMatchPieces(int x, int y)
    {
        List<Piece> matchPieces = new List<Piece>();
        matchPieces.AddRange(CheckMatchLine(x, y, 1, 0)); // Verifica linha horizontal
        matchPieces.AddRange(CheckMatchLine(x, y, 0, 1)); // Verifica linha vertical
        return matchPieces;
    }
    bool HasMatches()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (pieces[x, y] == null) continue;

                if (CheckMatchHorizontal(x, y) || CheckMatchVertical(x, y))
                {
                    return true;
                }
            }
        }
        return false;
    }
    List<Piece> CheckForMatches(out int totalDestroyed)
    {
        List<Piece> piecesToDestroy = new List<Piece>();
        totalDestroyed = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (pieces[x, y] == null) continue;

                List<Piece> matchPieces = GetMatchPieces(x, y);
                if (matchPieces.Count >= 3)
                {
                    piecesToDestroy.AddRange(matchPieces);
                    totalDestroyed += matchPieces.Count;
                    GameManager.instance.AddScore(10);
                }
            }
        }

        foreach (Piece piece in piecesToDestroy)
        {
            if (piece != null && piece.gameObject != null)
            {
                pieces[piece.x, piece.y] = null;
                if (piece.frutType != FrutType.Vazio)
                {
                    Instantiate(particle_popMagic, new Vector3(piece.x, piece.y), Quaternion.identity);
                }
                // Cancela os Tweens associados ao objeto antes de destruí-lo
                Destroy(piece.gameObject);
            }
        }


        StartCoroutine(RefillBoard());
        return piecesToDestroy;
    }
    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1f);
        GameManager.instance.UpdateGameOver("Game Over");
    }
    void MovePiece(Piece piece, Vector3 newPosition, float duration)
    {
        piece?.StartMoveAnimation(newPosition, duration);
    }
}

