using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    public GameObject[] piecePrefabs; // Array de prefabs das peças
    public int width; // Largura da grade
    public int height; // Altura da grade
    public Piece[,] pieces; // Array 2D para armazenar as peças
    public binaryArray binaryArrayInstance; // Instância do seu script binaryArray

    private Piece selectedPiece; // A peça atualmente selecionada
    private Vector2Int selectedPieceIndex; // Posição da peça selecionada

    void Start()
    {
        // Inicializa a grade
        pieces = new Piece[width, height];
        InitializeBoard();
    }


    void InitializeBoard()
    {
        pieces = new Piece[width, height]; // Cria a matriz para as peças

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Escolhe um tipo de fruta aleatório
                FrutType randomType = (FrutType)Random.Range(0, piecePrefabs.Length);

                // Cria uma nova peça a partir do prefab correspondente ao tipo de fruta
                GameObject pieceObject = Instantiate(GetPrefabByType(randomType), new Vector3(x, y, 0), Quaternion.identity);
                Piece piece = pieceObject.GetComponent<Piece>();
                piece.Init(x, y, this); // Inicializa a peça com sua posição e referência ao GridManager

                pieces[x, y] = piece; // Armazena a peça na matriz
            }
        }
    }

    // Método para obter o prefab correspondente ao tipo de fruta
    GameObject GetPrefabByType(FrutType type)
    {
        // Verifica se o tipo está dentro dos limites do array de prefabs
        int index = (int)type; // O enum é usado como índice
        if (index >= 0 && index < piecePrefabs.Length)
        {
            return piecePrefabs[index]; // Retorna o prefab correspondente
        }
        return null; // Retorna nulo se o tipo não corresponder a nenhum prefab
    }



    public void SelectPiece(Piece piece)
    {
        if (selectedPiece == null)
        {
            selectedPiece = piece;
            selectedPieceIndex = new Vector2Int(piece.x, piece.y);
        }
        else
        {
            // Tentar trocar as peças
            TrySwapPieces(selectedPiece, piece);
            DeselectPiece();
        }
    }

    void DeselectPiece()
    {
        selectedPiece = null;
    }

    void TrySwapPieces(Piece a, Piece b)
    {
        if (IsAdjacent(a, b))
        {
            // Troca as posições
            SwapPieces(a, b);
            // Verifica se há match após a troca
            CheckMatches();
        }
        else
        {
            // Se não forem adjacentes, talvez queira fazer algo, como desmarcá-las
            DeselectPiece();
        }
    }

    bool IsAdjacent(Piece a, Piece b)
    {
        // Verifica se as peças estão adjacentes
        return (Mathf.Abs(a.x - b.x) == 1 && a.y == b.y) || (Mathf.Abs(a.y - b.y) == 1 && a.x == b.x);
    }

    void SwapPieces(Piece a, Piece b)
    {
        // Troca as posições das peças
        int tempX = a.x;
        int tempY = a.y;

        a.x = b.x;
        a.y = b.y;

        b.x = tempX;
        b.y = tempY;

        // Atualiza o array de peças
        pieces[a.x, a.y] = a;
        pieces[b.x, b.y] = b;
    }

    void CheckMatches()
    {
        // Aqui você pode implementar a lógica para verificar se houve matches
        // e como lidar com eles (remover, animar, etc.)
    }
}
