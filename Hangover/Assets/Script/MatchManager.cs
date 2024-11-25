using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchManager : MonoBehaviour
{
    private GridManager _gridManager;

    private void Start()
    {
        _gridManager = GetComponent<GridManager>();
        if (_gridManager == null)
        {
            Debug.LogError("GridManager not found on the GameObject. Please ensure the GridManager script is attached to the same GameObject as MatchManager.");
        }
        else
        {
            CheckAndClearMatchesAtStart();
        }
    }

    public void CheckAndClearMatchesAtStart()
    {
        StartCoroutine(CheckAndClearMatchesCoroutine());
    }

    private IEnumerator CheckAndClearMatchesCoroutine()
    {
        yield return new WaitForSeconds(0.5f); // Espera o tempo necessário para inicializar o grid
        yield return StartCoroutine(_gridManager.ClearAndFillBoard());
        CheckMatchesOnBoard();
    }

    public void CheckMatchesOnBoard()
    {
        for (int x = 0; x < _gridManager.width; x++)
        {
            for (int y = 0; y < _gridManager.height; y++)
            {
                CheckForMatchAt(x, y);
            }
        }
    }

    public bool CheckForMatchAt(int x, int y)
    {
        if (!IsValidPosition(x, y) || _gridManager.grid[x, y] == null)
            return false;

        return HasMatchInDirections(_gridManager.grid[x, y]);
    }

    private bool HasMatchInDirections(Piece piece)
    {
        Vector2[] directions = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };
        foreach (Vector2 dir in directions)
        {
            if (CheckForMatchInDirection(piece, dir, 2))
                return true;
        }
        return false;
    }

    private bool CheckForMatchInDirection(Piece piece, Vector2 direction, int length)
    {
        List<Piece> matchingPieces = new List<Piece> { piece };
        for (int i = 1; i <= length; i++)
        {
            int checkX = piece.x + (int)direction.x * i;
            int checkY = piece.y + (int)direction.y * i;

            if (!IsValidPosition(checkX, checkY))
                break;

            Piece nextPiece = _gridManager.grid[checkX, checkY];
            if (nextPiece != null && nextPiece.frutType == piece.frutType)
            {
                matchingPieces.Add(nextPiece);
            }
            else
            {
                break;
            }
        }
        return matchingPieces.Count > length;
    }

    private bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < _gridManager.width && y >= 0 && y < _gridManager.height;
    }

    public List<Piece> GetAllMatchesForPiece(Piece piece)
    {
        List<Piece> horizontalMatches = GetMatchesInDirection(piece, 1, 0);
        horizontalMatches.AddRange(GetMatchesInDirection(piece, -1, 0).Where(p => p != piece));

        List<Piece> verticalMatches = GetMatchesInDirection(piece, 0, 1);
        verticalMatches.AddRange(GetMatchesInDirection(piece, 0, -1).Where(p => p != piece));

        List<Piece> allMatches = new List<Piece>();
        if (horizontalMatches.Count >= 3) allMatches.AddRange(horizontalMatches);
        if (verticalMatches.Count >= 3) allMatches.AddRange(verticalMatches);

        return allMatches.Distinct().ToList();
    }

    private List<Piece> GetMatchesInDirection(Piece piece, int dx, int dy)
    {
        List<Piece> matchingPieces = new List<Piece> { piece };
        int newX = piece.x + dx;
        int newY = piece.y + dy;

        while (IsValidPosition(newX, newY))
        {
            Piece nextPiece = _gridManager.grid[newX, newY];
            if (nextPiece != null && nextPiece.frutType == piece.frutType)
            {
                matchingPieces.Add(nextPiece);
            }
            else
            {
                break;
            }

            newX += dx;
            newY += dy;
        }

        return matchingPieces;
    }
}