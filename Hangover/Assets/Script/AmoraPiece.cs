using UnityEngine;

public class AmoraPiece : Piece
{
    public Sprite amoraSprite;

    public override void Init(int x, int y, GridManager gridManager)
    {
        base.Init(x, y, gridManager);
        frutType = FrutType.Amora;
        SetSprite();
        SetVisibility(true);
    }

    private void SetSprite()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && amoraSprite != null)
        {
            spriteRenderer.sprite = amoraSprite;

            Debug.Log($"Sprite da Amora configurado: {spriteRenderer.sprite.name}");

            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }
        else
        {
            Debug.LogWarning("SpriteRenderer ou amoraSprite não está configurado.");
        }
    }

    public override void AnimateDestruction()
    {
        base.AnimateDestruction();
        gridManager.powerUpManager.ActivateAmora(this, null);
    }

    private void OnMouseDown()
    {
        if (!isInvisible && gridManager != null)
        {
            Debug.Log("Amora clicada!");
            PieceSwapper pieceSwapper = FindObjectOfType<PieceSwapper>();
            if (pieceSwapper != null)
            {
                pieceSwapper.SelectPiece(this);
            }
            else
            {
                Debug.LogError("PieceSwapper não encontrado na cena.");
            }
        }
    }

    public override void OnSwap(Piece targetPiece)
    {
        if (MusicUI.instance?.estadoDoSom == true)
        {
            somSelect.Play();
        }
        base.OnSwap(targetPiece);

        if (targetPiece != null)
        {
            gridManager.powerUpManager.ActivateAmora(this, targetPiece);

            foreach (Piece piece in gridManager.grid)
            {
                if (piece != null && piece.frutType == targetPiece.frutType)
                {
                    gridManager.DestroyPiece(piece);
                }
            }
            gridManager.DestroyPiece(this);
        }
    }
}