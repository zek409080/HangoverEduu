using UnityEngine;

public class RomaPiece : Piece
{
    public override void Init(int x, int y, GridManager gridManager)
    {
        base.Init(x, y, gridManager);
        frutType = FrutType.Roma;
    }

    public override void AnimateDestruction()
    {
        base.AnimateDestruction();
        gridManager.powerUpManager.ActivateRoma(this);
    }

    void OnMouseDown()
    {
        if (!isInvisible && gridManager != null)
        {
            gridManager.powerUpManager.ActivateRoma(this);  // Ativar PowerUp roma ao clicar
            gridManager.DestroyPiece(this);  // Destruir a peça roma após o uso
        }
    }
}