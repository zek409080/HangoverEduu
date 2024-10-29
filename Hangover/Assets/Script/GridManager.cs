using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class GridManager : MonoBehaviour
{
    public int width, height;
    public float moveDuration = 0.3f;
    public GameObject[] piecePrefabs;
    private Piece[,] grid;
    private Piece selectedPiece;
    private bool isMatching = false; // Controle de estados durante matchs e descidas

    void Start()
    {
        InitializeBoard();
        StartCoroutine(CheckAndClearMatchesAtStart());
    }

    void InitializeBoard()
    {
        grid = new Piece[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SpawnNewPiece(x, y);
            }
        }
    }

    Piece SpawnNewPiece(int x, int y)
    {
        GameObject piecePrefab = piecePrefabs[Random.Range(0, piecePrefabs.Length)];
        GameObject newPieceObj = Instantiate(piecePrefab, new Vector3(x, y, 0), Quaternion.identity);
        newPieceObj.transform.parent = transform;

        Piece newPiece = newPieceObj.GetComponent<Piece>();
        newPiece.Init(x, y, this);
        grid[x, y] = newPiece;
        return newPiece; // Retorna a nova peça criada
    }


    public void SelectPiece(Piece piece)
    {
        if (isMatching) return; // Impede seleção durante os matchs e descidas

        if (selectedPiece == null)
        {
            selectedPiece = piece;
        }
        else if (IsAdjacent(selectedPiece, piece))
        {
            StartCoroutine(TrySwapPieces(selectedPiece, piece));
            selectedPiece = null;
        }
        else
        {
            selectedPiece = piece; // Se não for adjacente, seleciona a nova peça
        }
    }

    bool IsAdjacent(Piece piece1, Piece piece2)
    {
        return (Mathf.Abs(piece1.x - piece2.x) == 1 && piece1.y == piece2.y) ||
               (Mathf.Abs(piece1.y - piece2.y) == 1 && piece1.x == piece2.x);
    }

    IEnumerator TrySwapPieces(Piece piece1, Piece piece2)
    {
        // Verificar se há um match antes de realizar a troca
        SwapPieces(piece1, piece2);
        yield return new WaitForSeconds(moveDuration);

        if (CheckMatches(piece1) || CheckMatches(piece2))
        {
            yield return StartCoroutine(ClearAndFillBoard());
        }
        else
        {
            SwapPieces(piece1, piece2); // Reverte a troca se não houver match
            yield return new WaitForSeconds(moveDuration);
        }
    }

    void SwapPieces(Piece piece1, Piece piece2)
    {
        int tempX = piece1.x;
        int tempY = piece1.y;

        grid[piece1.x, piece1.y] = piece2;
        grid[piece2.x, piece2.y] = piece1;

        piece1.x = piece2.x;
        piece1.y = piece2.y;
        piece2.x = tempX;
        piece2.y = tempY;

        piece1.transform.DOMove(new Vector3(piece1.x, piece1.y, 0), moveDuration);
        piece2.transform.DOMove(new Vector3(piece2.x, piece2.y, 0), moveDuration);
    }

    bool CheckMatches(Piece piece)
    {
        if (piece == null) return false;

        List<Piece> matchedPieces = new List<Piece>();

        // Verifica matches horizontais
        matchedPieces.AddRange(CheckLineMatch(piece.x, piece.y, 1, 0)); // Direção positiva
        matchedPieces.Add(piece); // Adiciona a peça atual
        matchedPieces.AddRange(CheckLineMatch(piece.x, piece.y, -1, 0)); // Direção negativa

        // Se encontrou 3 ou mais peças do mesmo tipo
        if (matchedPieces.Count >= 3)
        {
            foreach (Piece p in matchedPieces)
            {
                p.MarkForDestruction();
            }
            return true;
        }

        // Limpa a lista para checar na vertical
        matchedPieces.Clear();

        // Verifica matches verticais
        matchedPieces.AddRange(CheckLineMatch(piece.x, piece.y, 0, 1)); // Direção positiva
        matchedPieces.Add(piece); // Adiciona a peça atual
        matchedPieces.AddRange(CheckLineMatch(piece.x, piece.y, 0, -1)); // Direção negativa

        // Se encontrou 3 ou mais peças do mesmo tipo
        if (matchedPieces.Count >= 3)
        {
            foreach (Piece p in matchedPieces)
            {
                p.MarkForDestruction();
            }
            return true;
        }

        return false;
    }

    List<Piece> CheckLineMatch(int startX, int startY, int offsetX, int offsetY)
    {
        List<Piece> matchedPieces = new List<Piece>();

        // Verifica se a posição inicial é válida
        if (grid[startX, startY] == null)
            return matchedPieces;

        FrutType startType = grid[startX, startY].frutType;

        // Verifica em uma direção
        for (int i = 1; i < 3; i++) // Verifica 2 peças adjacentes
        {
            int newX = startX + offsetX * i;
            int newY = startY + offsetY * i;

            // Verifica os limites da grade
            if (newX < 0 || newY < 0 || newX >= width || newY >= height)
                break;

            Piece nextPiece = grid[newX, newY];

            // Verifica se a próxima peça é do mesmo tipo
            if (nextPiece != null && nextPiece.frutType == startType)
            {
                matchedPieces.Add(nextPiece);
            }
            else
            {
                break; // Para de procurar se encontrar uma peça diferente
            }
        }

        return matchedPieces;
    }


    IEnumerator ClearAndFillBoard()
    {
        isMatching = true; // Bloqueia outras ações enquanto há matchs

        yield return StartCoroutine(ClearMatches());
        yield return new WaitForSeconds(1f); // Atraso para a remoção visual

        yield return StartCoroutine(FillEmptySpaces());
        yield return StartCoroutine(CheckAndClearMatchesAtStart());

        isMatching = false; // Libera as ações
    }

    IEnumerator ClearMatches()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] != null && !grid[x, y].isInvisible)
                {
                    Destroy(grid[x, y].gameObject);
                    grid[x, y] = null;
                }
            }
        }
        yield return new WaitForSeconds(moveDuration);
    }

    IEnumerator FillEmptySpaces()
    {
        for (int x = 0; x < width; x++)
        {
            // Mover as peças para baixo em cada coluna
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == null) // Se há um espaço vazio
                {
                    // Procura por uma peça acima para descer
                    for (int yAbove = y + 1; yAbove < height; yAbove++)
                    {
                        if (grid[x, yAbove] != null) // Encontrou uma peça acima
                        {
                            Piece pieceToMove = grid[x, yAbove];
                            grid[x, yAbove] = null; // Libera a posição acima
                            grid[x, y] = pieceToMove; // Move a peça para a posição vazia
                            pieceToMove.y = y; // Atualiza a posição Y da peça
                            pieceToMove.transform.DOMove(new Vector3(x, y, 0), moveDuration); // Move a peça na tela
                            break; // Sai do loop após mover uma peça
                        }
                    }
                }
            }

            // Após mover as peças, preencher os espaços vazios no topo
            for (int y = height - 1; y >= 0; y--)
            {
                if (grid[x, y] == null) // Se ainda há um espaço vazio
                {
                    // Gera nova peça na primeira posição vazia a partir do topo
                    Piece newPiece = SpawnNewPiece(x, y); // Gera nova peça na posição (x, y)
                                                          // Não é necessário atualizar grid[x, y], pois já foi atualizado no SpawnNewPiece
                }
            }
        }
        yield return new WaitForSeconds(moveDuration); // Atraso para animações
    }


    IEnumerator CheckAndClearMatchesAtStart()
    {
        bool hasMatches;

        do
        {
            hasMatches = false;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y] != null && CheckMatches(grid[x, y]))
                    {
                        hasMatches = true;
                    }
                }
            }

            if (hasMatches)
            {
                yield return StartCoroutine(ClearAndFillBoard());
            }

        } while (hasMatches);
    }
}
