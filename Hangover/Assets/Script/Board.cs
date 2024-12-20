﻿using System.Collections;
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
    public GameObject[] powerupPrefab;
    public Piece[,] pieces, powerUp;
    public Piece selectedPiece;
    private bool canSwap = true;
    public Transform cam;

    [Header("Objetivo")]
    public SpriteRenderer objetivo;
    public int maxObejetivo;
    private int currentObjective;
    public TextMeshProUGUI obejectiveText;

    [Header("Particle de exploção e o funda da fruta")]
    [SerializeField] public GameObject particle_popMagic, caixaDaGrid;
    

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
        List<Piece> piecesDestroyed = CheckForMatches(out _);
        CheckObjective(piecesDestroyed);
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
        // Verifica se a posição está vazia antes de instanciar uma nova peça
        if (IsPositionEmpty(x, y))
        {
            GameObject newPiece = Instantiate(piecePrefab[RandomFrut()], new Vector3(x, y, 0), Quaternion.identity);
            pieces[x, y] = newPiece.GetComponent<Piece>();
            pieces[x, y]?.Init(x, y, this);
        }
    }

    int RandomFrut()
    {
        return Random.Range(0, piecePrefab.Length);
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

        List<Piece> piecesDestroyed = CheckForMatches(out _);
        CheckObjective(piecesDestroyed);
    }





    IEnumerator RefillBoard()
    {
        yield return new WaitForSeconds(0.5f);
        bool boardRefilled = false;
        bool[] initialBools = binaryArray.GetInitialBools();

        // Movendo as peças para baixo onde há espaços vazios, incluindo `PowerUps`
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
                            // Move tanto `PowerUps` quanto peças normais para baixo
                            MovePiece(pieces[x, k], new Vector3(x, y, 0), 0.3f);
                            pieces[x, k].Init(x, y, this);
                            pieces[x, y] = pieces[x, k];
                            pieces[x, k] = null;
                            boardRefilled = true;
                            break;
                        }
                    }
                }
            }
        }

        // Preenchendo os espaços vazios com novas peças, sem sobrescrever `PowerUps`
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
                    boardRefilled = true;
                }
            }
        }

        if (boardRefilled)
        {
            yield return new WaitForSeconds(0.3f);
            List<Piece> piecesDestroyed = CheckForMatches(out int totalDestroyed);
            CheckObjective(piecesDestroyed);
        }
    }


    bool CheckMatchHorizontal(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height || pieces[x, y] == null) return false;
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
        if (x < 0 || x >= width || y < 0 || y >= height || pieces[x, y] == null) return false;
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

        if (!IsWithinBounds(x, y) || pieces[x, y] == null)
        {
            return matched;
        }

        FrutType type = pieces[x, y].frutType;
        matched.Add(pieces[x, y]);

        for (int i = 1; i < Mathf.Max(width, height); i++) // Certifica-se de não ultrapassar a matriz
        {
            int newX = x + dx * i;
            int newY = y + dy * i;

            if (!IsWithinBounds(newX, newY) || pieces[newX, newY] == null || pieces[newX, newY].frutType != type)
            {
                break;
            }
            matched.Add(pieces[newX, newY]);
        }

        return matched.Count >= 3 ? matched : new List<Piece>();
    }

    List<Piece> GetMatchPieces(int x, int y)
    {
        List<Piece> matchPieces = new List<Piece>();

        if (x < 0 || x >= width || y < 0 || y >= height || pieces[x, y] == null) return matchPieces;

        List<Piece> horizontalMatches = CheckMatchLine(x, y, 1, 0);
        if (horizontalMatches.Count >= 3) matchPieces.AddRange(horizontalMatches);

        List<Piece> verticalMatches = CheckMatchLine(x, y, 0, 1);
        if (verticalMatches.Count >= 3) matchPieces.AddRange(verticalMatches);

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
                if (!IsWithinBounds(x, y) || pieces[x, y] == null) continue;

                List<Piece> matchPieces = GetMatchPieces(x, y);

                if (matchPieces.Count >= 3) // Só pega os matches de 3 ou mais
                {
                    piecesToDestroy.AddRange(matchPieces);
                    totalDestroyed += matchPieces.Count;

                    // Verifique se deve instanciar um PowerUp
                    if (matchPieces.Count >= 4) // Quando a combinação for de 4 ou mais peças
                    {
                        int randomIndex = Random.Range(0, matchPieces.Count);
                        Piece powerUpPosition = matchPieces[randomIndex]; // Escolhe uma posição aleatória

                        // Verifique se a posição do PowerUp está dentro dos limites
                        if (IsWithinBounds(powerUpPosition.x, powerUpPosition.y))
                        {
                            pieces[powerUpPosition.x, powerUpPosition.y] = null; // Limpa a posição
                            InstantiateRandomPowerUp(powerUpPosition.x, powerUpPosition.y); // Instancia o PowerUp
                        }
                    }

                    GameManager.instance.AddScore(10);
                    if (MusicUI.instance.estadoDoSom)
                    {
                        audiopop.Play();
                    }
                }
            }
        }

        foreach (Piece piece in piecesToDestroy)
        {
            if (piece != null && piece.gameObject != null && IsWithinBounds(piece.x, piece.y))
            {
                pieces[piece.x, piece.y] = null;
                if (piece.frutType != FrutType.Vazio)
                {
                    Instantiate(particle_popMagic, new Vector3(piece.x, piece.y), Quaternion.identity);
                }
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

    void MovePiece(Piece piece, Vector3 newPosition, float duration)
    {
        if (piece != null)
        {
            piece.StartMoveAnimation(newPosition, duration);
            piece.x = (int)newPosition.x;
            piece.y = (int)newPosition.y;
        }
    }


    public void ActivateFranboesa(Piece amora, Piece targetPiece)
    {
        if (amora == null || targetPiece == null) return;
        Debug.Log("Ativando PowerUp Amora");

        foreach (Piece piece in pieces)
        {
            if (piece != null && piece.frutType == targetPiece.frutType)
            {
                Instantiate(particle_popMagic, piece.transform.position, Quaternion.identity);
                Destroy(piece.gameObject);
            }
        }
        Destroy(amora.gameObject);
        StartCoroutine(RefillBoard());
    }

    // Power-up que destrói peças em formato de +
    public void ActivateRoma(Piece roma)
    {
        if (roma == null) return;
        Debug.Log("Ativando PowerUp Roma");

        // Destruição na linha horizontal
        for (int x = 0; x < width; x++)
        {
            if (pieces[x, roma.y] != null)
            {
                Instantiate(particle_popMagic, pieces[x, roma.y].transform.position, Quaternion.identity);
                Destroy(pieces[x, roma.y].gameObject);
                pieces[x, roma.y] = null;
            }
        }

        // Destruição na linha vertical
        for (int y = 0; y < height; y++)
        {
            if (pieces[roma.x, y] != null)
            {
                Instantiate(particle_popMagic, pieces[roma.x, y].transform.position, Quaternion.identity);
                Destroy(pieces[roma.x, y].gameObject);
                pieces[roma.x, y] = null;
            }
        }

        Destroy(roma.gameObject);
        StartCoroutine(RefillBoard());
    }

    // Power-up que explode uma área de 3x3
    public void ActivateCereja(Piece cereja)
    {
        if (cereja == null) return;
        Debug.Log("Ativando PowerUp Cereja");

        int explosionRadius = 1; // 1 para 3x3

        for (int dx = -explosionRadius; dx <= explosionRadius; dx++)
        {
            for (int dy = -explosionRadius; dy <= explosionRadius; dy++)
            {
                int newX = cereja.x + dx;
                int newY = cereja.y + dy;

                if (IsWithinBounds(newX, newY) && pieces[newX, newY] != null)
                {
                    Instantiate(particle_popMagic, pieces[newX, newY].transform.position, Quaternion.identity);
                    Destroy(pieces[newX, newY].gameObject);
                    pieces[newX, newY] = null;
                }
            }
        }

        Destroy(cereja.gameObject);
        StartCoroutine(RefillBoard());
    }


    void InstantiateRandomPowerUp(int x, int y)
    {
        // Verifica se a posição está dentro dos limites e se está vazia
        if (!IsWithinBounds(x, y) || pieces[x, y] != null) return;

        // Seleciona um PowerUp aleatório do array `powerupPrefab`
        GameObject powerUpPrefab = powerupPrefab[Random.Range(0, powerupPrefab.Length)];
        GameObject powerUp = Instantiate(powerUpPrefab, new Vector3(x, y, 0), Quaternion.identity);

        // Configura o `PowerUp` para agir como uma peça
        Piece powerUpPiece = powerUp.GetComponent<Piece>();
        powerUpPiece.Init(x, y, this);
        powerUpPiece.isPowerUp = true;
        pieces[x, y] = powerUpPiece;  // Adiciona o `PowerUp` à matriz `pieces`

        // Inicia a animação de movimento do `PowerUp`
        powerUpPiece.StartMoveAnimation(new Vector3(x, y, 0), 0.3f);
    }
    bool IsPositionEmpty(int x, int y)
    {
        // Verifica se a posição está dentro dos limites e se não há peça na posição
        return IsWithinBounds(x, y) && pieces[x, y] == null;
    }
    bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

}