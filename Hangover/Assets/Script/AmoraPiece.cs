using UnityEngine;

public class AmoraPiece : Piece
{
    public override void Init(int x, int y, GridManager gridManager)
    {
        base.Init(x, y, gridManager);
        frutType = FrutType.Amora;
    }

    public override void AnimateDestruction()
    {
        // Primeiro ativa o PowerUp Amora
        gridManager.powerUpManager.ActivateAmora(this, null); // Nenhuma peça alvo necessária para a amora
        base.AnimateDestruction();
    }

    void OnMouseDown()
    {
        if (!isInvisible && gridManager != null)
        {
            gridManager.powerUpManager.ActivateAmora(this, null); // Nenhuma peça alvo necessária para a amora
            gridManager.DestroyPiece(this);    // Destruir a peça amora após o uso
        }
    }

    public override void OnSwap(Piece targetPiece)
    {
        if (MusicUI.instance?.estadoDoSom == true)
        {
            somSelect.Play();
        }
        base.OnSwap(targetPiece);
        gridManager.powerUpManager.ActivateAmora(this, targetPiece);
        gridManager.DestroyPiece(this); // Destruir a própria Amora após o uso
    }
}