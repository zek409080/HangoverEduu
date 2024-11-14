using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class MatchManager : MonoBehaviour
{
    private GridManager gridManager;

    private void Start()
    {
        gridManager = GetComponent<GridManager>();
    }

    public void CheckAndClearMatchesAtStart()
    {
        StartCoroutine(CheckAndClearMatchesCoroutine());
    }

    private IEnumerator CheckAndClearMatchesCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(gridManager.ClearAndFillBoard());
        
    }

    private bool HasAnyMatches()
    {
        for (int x = 0; x < gridManager.width; x++)
        {
            for (int y = 0; y < gridManager.height; y++)
            {
                if (CheckForMatchAt(x, y))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool CheckForMatchAt(int x, int y)
    {
        if (gridManager.grid[x, y] == null) return false;

        return (CheckForMatchInDirection(gridManager.grid[x, y], Vector2.left, 2) ||
                CheckForMatchInDirection(gridManager.grid[x, y], Vector2.right, 2) ||
                CheckForMatchInDirection(gridManager.grid[x, y], Vector2.up, 2) ||
                CheckForMatchInDirection(gridManager.grid[x, y], Vector2.down, 2));
    }

    private bool CheckForMatchInDirection(Piece piece, Vector2 direction, int length)
    {
        List<Piece> matchingPieces = new List<Piece> { piece };
        for (int i = 1; i <= length; i++)
        {
            int checkX = piece.x + (int)direction.x * i;
            int checkY = piece.y + (int)direction.y * i;
            if (checkX < 0 || checkX >= gridManager.width || checkY < 0 || checkY >= gridManager.height)
                break;
            if (gridManager.grid[checkX, checkY] != null && gridManager.grid[checkX, checkY].frutType == piece.frutType)
                matchingPieces.Add(gridManager.grid[checkX, checkY]);
            else
                break;
        }
        return matchingPieces.Count > length;
    }

    public List<Piece> GetAllMatchesForPiece(Piece piece)
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

            if (newX < 0 || newX >= gridManager.width || newY < 0 || newY >= gridManager.height) break;

            Piece nextPiece = gridManager.grid[newX, newY];
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
}