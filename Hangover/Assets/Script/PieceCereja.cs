using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceCereja : MonoBehaviour
{
    private Board board;
    private void Start()
    {
        board = GameObject.Find("GridManager").GetComponent<Board>();
    }
    private void OnMouseDown()
    {
        // Chama o power-up Cereja passando a própria peça como parâmetro.
        board.ActivateCereja(GetComponent<Piece>());
    }
}
