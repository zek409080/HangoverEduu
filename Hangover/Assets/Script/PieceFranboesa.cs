using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceFranboesa : Piece
{
    private Board board;
    private void Start()
    {
        board = GameObject.Find("GridManager").GetComponent<Board>();
    }
    private void OnMouseDown()
    {
        // Chama o power-up passando a peça atual como amora e uma peça alvo (por exemplo, outra selecionada).
        Piece targetPiece = board.selectedPiece; // Supondo que há um método para obter a peça selecionada.
        if (targetPiece != null)
        {
            board.ActivateFranboesa(GetComponent<Piece>(), targetPiece);
        }
    }
}
