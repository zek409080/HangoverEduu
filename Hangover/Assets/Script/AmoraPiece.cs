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
        base.AnimateDestruction();
        // Amora não é destruída imediatamente
    }

    void OnMouseDown()
    {
        // Amora só ativada quando for trocada com outra peça
    }

    public override void OnSwap(Piece targetPiece)
    {
        if (MusicUI.instance.estadoDoSom)
        {
            somSelect.Play();
        }
        base.OnSwap(targetPiece);
        gridManager.ActivateAmora(this, targetPiece);
        gridManager.DestroyPiece(this); // Destruir a própria Amora após o uso
    }
}