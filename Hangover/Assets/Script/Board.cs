using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("Tamanho do tabuleiro")]
    public int width;
    public int height;

    [SerializeField] int jogodas;
    public GameObject[] piecePrefab;
    public Piece[,] pieces;
    private Piece selectedPiece;
    Vector3 vector3Base;
    private bool canSwap = true;
    public Transform cam;
    [SerializeField] GameObject particle_popMagic;
    void Start()
    {
        vector3Base = new Vector3(0.8f, 0.8f, 0);
        GameManager.instance.UpdateJogadas(jogodas);
        pieces = new Piece[width, height];
        InitializeBoard();
    }

    private void Update()
    {
        print(HasPossibleMatches());
        StartCoroutine(GameOver());
    }

    void InitializeBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject newPiece = Instantiate(piecePrefab[RandomFrut()], new Vector3(x, y, 0), Quaternion.identity);
                if (newPiece != null)
                {
                    pieces[x, y] = newPiece.GetComponent<Piece>();
                    if (pieces[x, y] != null)
                    {
                        pieces[x, y].Init(x, y, this);
                    }
                }
            }
        }
        cam.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);
        CheckForMatches();
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
            selectedPiece.AnimateScale(vector3Base * 1.2f, 0.2f);
        }
        else
        {
            if (IsAdjacent(selectedPiece, piece))
            {
                selectedPiece.AnimateScale(vector3Base, 0.2f);
                piece.AnimateScale(vector3Base, 0.2f);
                StartCoroutine(TrySwapPieces(selectedPiece, piece));
            }
            else
            {
                selectedPiece.AnimateScale(vector3Base, 0.2f);
                selectedPiece = piece;
                selectedPiece.AnimateScale(vector3Base * 1.2f, 0.2f);
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

        SwapPieces(piece1, piece2);

        yield return new WaitForSeconds(0.1f);

        if (!HasMatches())
        {
            SwapPieces(piece1, piece2);
            piece1.AnimateScale(vector3Base, 0.2f);
            piece2.AnimateScale(vector3Base, 0.2f);
        }
        else
        {
            CheckForMatches();
        }
        GameManager.instance.UpdateJogadas(-1);
        selectedPiece = null;
        canSwap = true;
    }

    void SwapPieces(Piece piece1, Piece piece2)
    {
        int tempX = piece1.x;
        int tempY = piece1.y;

        pieces[piece1.x, piece1.y] = piece2;
        pieces[piece2.x, piece2.y] = piece1;

        piece1.Init(piece2.x, piece2.y, this);
        piece2.Init(tempX, tempY, this);

        Vector3 tempPosition = piece1.transform.position;
        piece1.transform.position = piece2.transform.position;
        piece2.transform.position = tempPosition;
    }

    bool HasMatches()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (pieces[x, y] == null) continue;

                if (x < width - 2)
                {
                    FrutType currentType = pieces[x, y].frutType;
                    if (pieces[x + 1, y] != null && pieces[x + 2, y] != null &&
                        pieces[x + 1, y].frutType == currentType && pieces[x + 2, y].frutType == currentType)
                    {
                        return true;
                    }
                }

                if (y < height - 2)
                {
                    FrutType currentType = pieces[x, y].frutType;
                    if (pieces[x, y + 1] != null && pieces[x, y + 2] != null &&
                        pieces[x, y + 1].frutType == currentType && pieces[x, y + 2].frutType == currentType)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void CheckForMatches()
    {
        List<Piece> piecesToDestroy = new List<Piece>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (pieces[x, y] == null) continue;

                // Check horizontal matches
                if (x < width - 2)
                {
                    int matchLength = 1;
                    FrutType currentType = pieces[x, y].frutType;
                    for (int k = 1; k < width - x; k++)
                    {
                        if (pieces[x + k, y] != null && pieces[x + k, y].frutType == currentType)
                        {
                            matchLength++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (matchLength >= 3)
                    {
                        for (int k = 0; k < matchLength; k++)
                        {
                            piecesToDestroy.Add(pieces[x + k, y]);
                            GameManager.instance.AddScore(10);
                        }
                    }
                }

                // Check vertical matches
                if (y < height - 2)
                {
                    int matchLength = 1;
                    FrutType currentType = pieces[x, y].frutType;
                    for (int k = 1; k < height - y; k++)
                    {
                        if (pieces[x, y + k] != null && pieces[x, y + k].frutType == currentType)
                        {
                            matchLength++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (matchLength >= 3)
                    {
                        for (int k = 0; k < matchLength; k++)
                        {
                            piecesToDestroy.Add(pieces[x, y + k]);
                            GameManager.instance.AddScore(10);
                        }
                    }
                }
            }
        }

        foreach (Piece piece in piecesToDestroy)
        {
            if (piece != null)
            {
                pieces[piece.x, piece.y] = null;
                Instantiate(particle_popMagic, new Vector3(piece.x, piece.y), Quaternion.identity);
                Destroy(piece.gameObject);
            }
        }

        if (piecesToDestroy.Count > 0)
        {
            StartCoroutine(RefillBoard());
        }
    }
        IEnumerator RefillBoard()
        {
            yield return new WaitForSeconds(0.5f);

            // Reposiciona e gera novas peças
            for (int x = 0; x < width; x++)
            {
                int emptyCount = 0;
                for (int y = 0; y < height; y++)
                {
                    if (pieces[x, y] == null)
                    {
                        emptyCount++;
                    }
                    else if (emptyCount > 0)
                    {
                        pieces[x, y - emptyCount] = pieces[x, y];
                        pieces[x, y].Init(x, y - emptyCount, this);
                        StartCoroutine(MovePiece(pieces[x, y], new Vector3(x, y - emptyCount, 0)));
                        pieces[x, y] = null;
                    }
                }

                for (int y = height - emptyCount; y < height; y++)
                {
                    GameObject newPiece = Instantiate(piecePrefab[RandomFrut()], new Vector3(x, y, 0), Quaternion.identity);
                    pieces[x, y] = newPiece.GetComponent<Piece>();
                    StartCoroutine(MovePiece(pieces[x, y], new Vector3(x, y, 0)));
                    pieces[x, y].Init(x, y, this);
                }
            }

            CheckForMatches();
        yield return new WaitForSeconds(1f); // Tempo adicional para aguardar que tudo se resolva      

        }
    IEnumerator MovePiece(Piece piece, Vector3 newPosition, float duration = 0.2f)
    {
        if (piece == null || piece.transform == null)
        {
            yield break;  // Sai da função se a peça foi destruída
        }

        Vector3 startingPosition = piece.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (piece == null || piece.transform == null)
            {
                yield break;  // Sai da função se a peça foi destruída durante o movimento
            }

            float t = elapsedTime / duration;  // Normaliza o tempo
            t = t * t * (3f - 2f * t);  // Interpolação suave
            piece.transform.position = Vector3.Lerp(startingPosition, newPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;  // Aguarda o próximo quadro
        }
    }
        bool CanMatchBeMade(int x1, int y1, int x2, int y2)
    {
        // Verifica se as coordenadas estão dentro dos limites do tabuleiro
        if (x1 < 0 || x1 >= width || y1 < 0 || y1 >= height ||
            x2 < 0 || x2 >= width || y2 < 0 || y2 >= height)
        {
            return false;
        }
        // Verifica se as peças estão adjacentes
        if (Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2) != 1)
        {
            return false; // Não são adjacentes
        }

        // Troca as peças temporariamente
        Piece temp = pieces[x1, y1];
        pieces[x1, y1] = pieces[x2, y2];
        pieces[x2, y2] = temp;

        bool matchFound = HasMatches();

        // Desfaz a troca para restaurar o estado original
        temp = pieces[x1, y1];
        pieces[x1, y1] = pieces[x2, y2];
        pieces[x2, y2] = temp;

        return matchFound;
    }


    // Novo método para verificar se existem possíveis matches no tabuleiro
    public bool HasPossibleMatches()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (pieces[x, y] == null) continue;

                // Verifica trocas adjacentes
                if (CanMatchBeMade(x, y, x + 1, y) || CanMatchBeMade(x, y, x, y + 1))
                {
                    return true;
                }
            }
        }
        return false;
    }

    IEnumerator GameOver()
    {
        if (GameManager.instance.jogadas == 0)
        {
            yield return new WaitForSeconds(1f);
            GameManager.instance.UpdateGameOver("Game Over");
        }
    }
}
