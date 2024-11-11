using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceCereja : Piece
{
    public override void Init(int x, int y, Board gridManager)
    {
        base.Init(x, y, gridManager);
        frutType = FrutType.Cereja;
    }

    private void OnMouseDown()
    {
        // Chama o power-up Cereja passando a pr�pria pe�a como par�metro.
        board.ActivateCereja(GetComponent<Piece>());
    }
}
