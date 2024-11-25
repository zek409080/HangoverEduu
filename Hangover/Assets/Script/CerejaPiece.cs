using UnityEngine;

public class CerejaPiece : Piece
{
    public override void Init(int x, int y, GridManager gridManager)
    {
        base.Init(x, y, gridManager);
        frutType = FrutType.Cereja;
    }

    public override void AnimateDestruction()
    {
        base.AnimateDestruction();
        gridManager.powerUpManager.ActivateCereja(this);
    }

    void OnMouseDown()
    {
        if (!isInvisible && gridManager != null)
        {
            Debug.Log("Cereja clicada!");
            gridManager.powerUpManager.ActivateCereja(this);  // Ativar PowerUp cereja ao clicar
            gridManager.DestroyPiece(this);    // Destruir a peça cereja após o uso
        }
    }
}