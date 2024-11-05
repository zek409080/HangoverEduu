using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Board : MonoBehaviour
{
    // Parâmetros do tabuleiro
    [Header("Tamanho do tabuleiro")]
    public int width;
    public int height;
    private binaryArrayBoard binaryArray;
    [SerializeField] int jogadas;
    public GameObject[] piecePrefab;
    public Piece[,] pieces;
    private Piece selectedPiece;
    private bool canSwap = true;
    public Transform cam;

    [Header("Objetivo")]
    public SpriteRenderer objetivo;
    public int maxObejetivo;
    private int currentObjective;
    public TextMeshProUGUI obejectiveText;

    [Header("Particle de exploção e o funda da fruta")]
    [SerializeField] GameObject particle_popMagic, caixaDaGrid;
    

    [Header("Audio")]
    AudioSource audiopop;


    void Start()
    {
        currentObjective = 0;
        audiopop = GetComponent<AudioSource>();
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
        if (MusicUI.instance.estadoDoSom)    
        {
          audiopop.enabled = false;
        }
        else
        {
            audiopop.enabled = true;
        }

        if (GameManager.instance.jogadas == 0)
        {
            //checar se foi completado o objetivo 
            if (currentObjective >= maxObejetivo)
            {
                StartCoroutine(GameVitoria());
            }
            else
            {
                StartCoroutine(GameOver());
            }
           
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
        List<Piece> piecesDestroyed = CheckForMatches(out _);
        CheckObjective(piecesDestroyed);
    }

    Piece CreateEmptyPiece(int x, int y)
    {
        GameObject emptyObject = new GameObject("EmptyPiece");
        Piece emptyPiece = emptyObject.AddComponent<Piece>();
        emptyPiece.frutType = FrutType.Vazio;
        emptyPiece.Init(x, y, this);
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
        return Random.Range(0, piecePrefab.Length - 3);
    }
    bool IsAdjacent(Piece piece1, Piece piece2)
    {
        return (Mathf.Abs(piece1.x - piece2.x) == 1 && piece1.y == piece2.y) ||
               (Mathf.Abs(piece1.y - piece2.y) == 1 && piece1.x == piece2.x);
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
                StartCoroutine(TrySwapPieces(selectedPiece, piece));
                selectedPiece = null; // Reseta a variável após iniciar a troca
            }
            else
            {
                selectedPiece.StartScaleAnimation(new Vector2(0.8f, 0.8f), 0.2f);
                selectedPiece = piece;
                selectedPiece.StartScaleAnimation(new Vector2(0.8f, 0.8f) , 0.2f);
            }
        }
    }
    private void UpdateObjectiveText()
    {
        obejectiveText.text = $"{currentObjective}/{maxObejetivo}";
    }

    void CheckObjective(List<Piece> piecesDestroyed)
    {
        FrutType objetivoFrutType = objetivo.GetComponent<Piece>().frutType;
        int destroyedCount = 0;

        foreach (Piece piece in piecesDestroyed)
        {
            if (piece.frutType == objetivoFrutType)
            {
                destroyedCount++;
            }
        }

        if (destroyedCount > 0)
        {
            currentObjective += destroyedCount;
        }
        UpdateObjectiveText();
    }
    public IEnumerator TrySwapPieces(Piece piece1, Piece piece2)
    {
        // Verifica novamente a existência das peças no início
        if (piece1 == null || piece2 == null || piece1.gameObject == null || piece2.gameObject == null)
        {
            Debug.LogWarning("Tentativa de trocar peças que foram destruídas.");
            yield break;
        }

        canSwap = false;

        // Guarda as posições originais
        Vector3 originalPosition1 = piece1.transform.position;
        Vector3 originalPosition2 = piece2.transform.position;

        // Troca de posição inicial
        SwapPieces(piece1, piece2);

        yield return new WaitForSeconds(0.15f);

        // Verifica se a troca gerou um match
        if (!HasMatches())
        {
            // Se não houver match, restaura a posição na matriz antes de animar o retorno
            SwapPieces(piece1, piece2);

            // Retorna visualmente para posição original
            if (piece1 != null && piece2 != null && piece1.gameObject != null && piece2.gameObject != null)
            {
                piece1.StartMoveAnimation(originalPosition1, 0.1f);
                piece2.StartMoveAnimation(originalPosition2, 0.1f);
                yield return new WaitForSeconds(0.15f);
            }
        }
        else
        {
            // Se houve match, processa as peças e verifica novos matches
            CheckForMatches(out _);
        }

        // Atualiza o número de jogadas e redefine a peça selecionada após a troca completa
        GameManager.instance.UpdateJogadas(-1);
        selectedPiece = null;
        canSwap = true;
    }


    void SwapPieces(Piece piece1, Piece piece2)
    {
        // Verifica se as peças ainda existem antes de realizar a troca
        if (piece1 == null || piece2 == null || piece1.gameObject == null || piece2.gameObject == null)
        {
            Debug.LogWarning("Tentativa de trocar peças que foram destruídas.");
            return;
        }

        // Troca as peças na matriz
        (pieces[piece1.x, piece1.y], pieces[piece2.x, piece2.y]) = (piece2, piece1);

        // Atualiza os índices das peças
        int tempX = piece1.x, tempY = piece1.y;
        piece1.Init(piece2.x, piece2.y, this);
        piece2.Init(tempX, tempY, this);

        // Animação de troca somente se ambas as peças ainda existirem
        if (piece1 != null && piece2 != null && piece1.gameObject != null && piece2.gameObject != null)
        {
            Vector3 tempPosition = piece1.transform.position;
            piece1.StartMoveAnimation(piece2.transform.position, 0.1f);
            piece2.StartMoveAnimation(tempPosition, 0.1f);
        }

        List<Piece> piecesDestroyed = CheckForMatches(out int totalDestroyed);
        CheckObjective(piecesDestroyed);
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

        yield return new WaitForSeconds(0.3f); // Aguarda as animações

        CheckForMatches(out _);
        List<Piece> piecesDestroyed = CheckForMatches(out int totalDestroyed);
        CheckObjective(piecesDestroyed);
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

    List<Piece> GetMatchPieces(int x, int y)
    {
        List<Piece> matchPieces = new List<Piece>();

        // Checa combinações horizontais
        if (CheckMatchHorizontal(x, y))
        {
            // Adiciona a peça atual
            matchPieces.Add(pieces[x, y]);

            // Adiciona as peças à direita
            for (int i = x + 1; i < width && pieces[i, y] != null && pieces[i, y].frutType == pieces[x, y].frutType; i++)
            {
                matchPieces.Add(pieces[i, y]);
            }

            // Adiciona as peças à esquerda
            for (int i = x - 1; i >= 0 && pieces[i, y] != null && pieces[i, y].frutType == pieces[x, y].frutType; i--)
            {
                matchPieces.Add(pieces[i, y]);
            }
        }

        // Checa combinações verticais
        if (CheckMatchVertical(x, y))
        {
            // Adiciona a peça atual
            matchPieces.Add(pieces[x, y]);

            // Adiciona as peças acima
            for (int i = y + 1; i < height && pieces[x, i] != null && pieces[x, i].frutType == pieces[x, y].frutType; i++)
            {
                matchPieces.Add(pieces[x, i]);
            }

            // Adiciona as peças abaixo
            for (int i = y - 1; i >= 0 && pieces[x, i] != null && pieces[x, i].frutType == pieces[x, y].frutType; i--)
            {
                matchPieces.Add(pieces[x, i]);
            }
        }

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
    void CreatePowerPiece(int x, int y, FrutType powerType)
    {
        GameObject powerPiece = Instantiate(piecePrefab[(int)powerType], new Vector3(x, y, 0), Quaternion.identity);
        pieces[x, y] = powerPiece.GetComponent<Piece>();
        pieces[x, y]?.Init(x, y, this);
    }

    void InstantiatePowerIfMatch(List<Piece> matchPieces)
    {
        if (matchPieces.Count >= 4)
        {
            Piece lastPiece = matchPieces[matchPieces.Count - 1];
            FrutType powerType = matchPieces.Count == 4 ? FrutType.Roma :
                                (matchPieces.Count == 5 ? FrutType.Cereja : FrutType.Framboesa);
            CreatePowerPiece(lastPiece.x, lastPiece.y, powerType);
        }
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
                    InstantiatePowerIfMatch(matchPieces);
                    piecesToDestroy.AddRange(matchPieces);
                    totalDestroyed += matchPieces.Count;
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
        GameManager.instance.UpdateGameOver("Perdeu");
    }
    IEnumerator GameVitoria()
    {
        yield return new WaitForSeconds(1f);
        GameManager.instance.UpdateGameOver("Vitória");

    }

    public void DestroyPiece(int x, int y)
    {
        if (pieces[x, y] != null)
        {
            Instantiate(particle_popMagic, new Vector3(x, y), Quaternion.identity);
            Destroy(pieces[x, y].gameObject);
            pieces[x, y] = null;
        }
    }


    void MovePiece(Piece piece, Vector3 newPosition, float duration)
    {
        piece?.StartMoveAnimation(newPosition, duration);
    }
}

