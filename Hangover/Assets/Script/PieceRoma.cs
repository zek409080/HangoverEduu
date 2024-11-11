using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceRoma : Piece
{
    private Board board;
    private void Start()
    {
        board = GameObject.Find("GridManager").GetComponent<Board>();
    }

    private void OnMouseDown()
    {
        // Chama o power-up Roma passando a pr�pria pe�a como par�metro.
        board.ActivateRoma(GetComponent<Piece>());
    }
}
