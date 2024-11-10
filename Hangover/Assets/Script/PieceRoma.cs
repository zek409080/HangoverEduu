using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceRoma : MonoBehaviour
{
    private Board board;
    private void Start()
    {
        board = GameObject.Find("GridManager").GetComponent<Board>();
    }

    private void OnMouseDown()
    {
        // Chama o power-up Roma passando a própria peça como parâmetro.
        board.ActivateRoma(GetComponent<Piece>());
    }
}
