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

    private void OnMouseDown()
    {
        if (!isInvisible && gridManager != null)
        {
            Debug.Log("Roma clicada!");
            gridManager.powerUpManager.ActivateRoma(this);  // Ativar PowerUp roma ao clicar
            gridManager.DestroyPiece(this);  // Destruir a peça roma após o uso
        }
    }

    public override void OnSwap(Piece targetPiece)
    {
        if (MusicUI.instance?.estadoDoSom == true)
        {
            somSelect.Play();
        }
        base.OnSwap(targetPiece);
        
        if (!isMarkedForDestruction)
        {
            gridManager.powerUpManager.ActivateRoma(this);
            gridManager.DestroyPiece(this);  // Destruir a própria Roma após o uso
        }
    }
}