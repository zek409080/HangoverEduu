using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PowerUpPiece
{
    void Activate();
    void SwapAndActivate(Piece otherPiece);
}