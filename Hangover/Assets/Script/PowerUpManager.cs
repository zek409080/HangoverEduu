using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;

public class PowerUpManager : MonoBehaviour
{
    public GameObject destructionEffectPrefab;
    private GridManager gridManager;
    public GameObject cerejaPrefab;
    public GameObject romaPrefab;
    public GameObject amoraPrefab;

    private void Start()
    {
        gridManager = GetComponent<GridManager>();
    }

    public void ActivateCereja(Piece cereja)
    {
        if (cereja == null || cereja.isMarkedForDestruction) return;
        Debug.Log("Ativando PowerUp Cereja");

        // Instanciar efeito de explosão
        Instantiate(destructionEffectPrefab, cereja.transform.position, Quaternion.identity);

        // Definindo o raio da explosão como 1, o que resulta em uma área 3x3
        int explosionRadius = 1;

        // Percorrendo em torno da peça Cereja
        for (int dx = -explosionRadius; dx <= explosionRadius; dx++)
        {
            for (int dy = -explosionRadius; dy <= explosionRadius; dy++)
            {
                int newX = cereja.x + dx;
                int newY = cereja.y + dy;

                if (IsWithinBounds(newX, newY))
                {
                    Piece neighbor = gridManager.grid[newX, newY];
                    if (neighbor != null && !neighbor.isMarkedForDestruction)
                    {
                        neighbor.MarkForDestruction();
                        neighbor.AnimateDestruction();
                    }
                }
            }
        }
        StartCoroutine(gridManager.ResetMatching());
    }

    public void ActivateRoma(Piece roma)
    {
        if (roma == null || roma.isMarkedForDestruction) return;
        Debug.Log("Ativando PowerUp Roma");

        for (int x = 0; x < gridManager.width; x++)
        {
            if (gridManager.grid[x, roma.y] != null && !gridManager.grid[x, roma.y].isMarkedForDestruction)
            {
                Instantiate(destructionEffectPrefab, gridManager.grid[x, roma.y].transform.position, Quaternion.identity);
                gridManager.grid[x, roma.y].MarkForDestruction();
                gridManager.grid[x, roma.y].AnimateDestruction();
            }
        }

        for (int y = 0; y < gridManager.height; y++)
        {
            if (gridManager.grid[roma.x, y] != null && !gridManager.grid[roma.x, y].isMarkedForDestruction)
            {
                Instantiate(destructionEffectPrefab, gridManager.grid[roma.x, y].transform.position, Quaternion.identity);
                gridManager.grid[roma.x, y].MarkForDestruction();
                gridManager.grid[roma.x, y].AnimateDestruction();
            }
        }

        StartCoroutine(gridManager.ResetMatching());
    }

    public void ActivateAmora(Piece amora, Piece targetPiece)
    {
        if (amora == null || amora.isMarkedForDestruction || targetPiece == null) return;
        Debug.Log("Ativando PowerUp Amora");

        foreach (Piece piece in gridManager.grid)
        {
            if (piece != null && piece.frutType == targetPiece.frutType && !piece.isMarkedForDestruction)
            {
                Instantiate(destructionEffectPrefab, piece.transform.position, Quaternion.identity);
                piece.MarkForDestruction();
                piece.AnimateDestruction();
            }
        }

        gridManager.DestroyPiece(amora);
        StartCoroutine(gridManager.ResetMatching());
    }

    private bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < gridManager.width && y >= 0 && y < gridManager.height;
    }

    public void CreatePowerUp(Piece piece)
    {
        if (piece == null || piece.isMarkedForDestruction) return;

        float randomValue = Random.value;

        if (randomValue < 0.33f)
        {
            InstantiatePowerUp(cerejaPrefab, piece.x, piece.y);
        }
        else if (randomValue < 0.66f)
        {
            InstantiatePowerUp(romaPrefab, piece.x, piece.y);
        }
        else
        {
            InstantiatePowerUp(amoraPrefab, piece.x, piece.y);
        }
    }

    private void InstantiatePowerUp(GameObject powerUpPrefab, int x, int y)
    {
        GameObject powerUpObject = Instantiate(powerUpPrefab, new Vector3(x, y, 0), Quaternion.identity);
        Piece powerUpPiece = powerUpObject.GetComponent<Piece>();
        powerUpPiece.Init(x, y, gridManager);
        gridManager.grid[x, y] = powerUpPiece;
    }
}