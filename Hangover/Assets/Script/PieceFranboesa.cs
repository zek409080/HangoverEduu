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
        // Chama o power-up passando a pe�a atual como amora e uma pe�a alvo (por exemplo, outra selecionada).
        Piece targetPiece = board.selectedPiece; // Supondo que h� um m�todo para obter a pe�a selecionada.
        if (targetPiece != null)
        {
            board.ActivateFranboesa(GetComponent<Piece>(), targetPiece);
        }
    }
}
