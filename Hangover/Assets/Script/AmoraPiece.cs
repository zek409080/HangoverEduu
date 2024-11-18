using UnityEngine;

public class AmoraPiece : Piece
{
    public override void Init(int x, int y, GridManager gridManager)
    {
        base.Init(x, y, gridManager);
        frutType = FrutType.Amora;
        SetVisibility(true); // Garanta que a peça é visível ao inicializar
    }

    public override void AnimateDestruction()
    {
        base.AnimateDestruction();
        gridManager.powerUpManager.ActivateAmora(this, null); // Null se não houver peça alvo
    }

    void OnMouseDown()
    {
        if (!isInvisible && gridManager != null)
        {
            Debug.Log("Amora clicada!");
            gridManager.powerUpManager.ActivateAmora(this, null); // Ativar PowerUp amora ao clicar
            gridManager.DestroyPiece(this); // Destruir a peça amora após o uso
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