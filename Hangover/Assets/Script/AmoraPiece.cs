using UnityEngine;

public class AmoraPiece : Piece
{
    PieceSwapper swapper;

    public override void Init(int x, int y, GridManager gridManager)
    {
        base.Init(x, y, gridManager);
        frutType = FrutType.Amora;
    }

    public override void AnimateDestruction()
    {
        base.AnimateDestruction();
        gridManager.powerUpManager.ActivateAmora(this, null); // Esta linha provavelmente deve passar a peça alvo correta
    }

    void OnMouseDown()
    {
        if (!isInvisible && gridManager != null)
        {
            Debug.Log("Amora clicada!");

            if (pieceSwapper != null)
            {
                pieceSwapper.SelectPiece(this); // Seleciona a Amora para troca
            }
            else
            {
                Debug.LogError("pieceSwapper é null na AmoraPiece");
            }
        }
    }
}