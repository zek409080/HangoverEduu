using UnityEngine;
using DG.Tweening;
using System.Collections;

public class PieceSwapper : MonoBehaviour
{
    private GridManager _gridManager;
    private PowerUpManager _powerUpManager;
    private Piece _selectedPiece = null;
    [SerializeField]
    private float delayDuringSwap = 0.2f;
    private bool isSwapping = false;

    private void Start()
    {
        _gridManager = FindObjectOfType<GridManager>();
        _powerUpManager = FindObjectOfType<PowerUpManager>();

        if (_gridManager == null)
        {
            Debug.LogError("GridManager not found in the scene. Please ensure there is a GridManager object in the scene.");
        }

        if (_powerUpManager == null)
        {
            Debug.LogError("PowerUpManager not found in the scene. Please ensure there is a PowerUpManager object in the scene.");
        }
    }

    public void SelectPiece(Piece piece)
    {
        if (isSwapping) return;
        if (_selectedPiece == null)
        {
            SelectNewPiece(piece);
        }
        else if (_selectedPiece == piece)
        {
            DeselectCurrentPiece();
        }
        else if (_gridManager != null && _gridManager.AreAdjacent(_selectedPiece, piece))
        {
            StartCoroutine(SwapAndCheckPowerUp(_selectedPiece, piece));
        }
        else
        {
            SelectNewPiece(piece);
        }
    }

    private void SelectNewPiece(Piece piece)
    {
        DeselectCurrentPiece();
        _selectedPiece = piece;
        _selectedPiece.SetSelected(true);
        Debug.Log("Piece Selected: " + piece.name);
    }

    private void DeselectCurrentPiece()
    {
        if (_selectedPiece != null)
        {
            _selectedPiece.SetSelected(false);
            _selectedPiece = null;
        }
    }

    private IEnumerator SwapAndCheckPowerUp(Piece piece1, Piece piece2)
    {
        isSwapping = true;

        Vector3 pos1 = piece1.transform.position;
        Vector3 pos2 = piece2.transform.position;
        
        piece1.transform.DOMove(pos2, _gridManager.moveDuration);
        piece2.transform.DOMove(pos1, _gridManager.moveDuration);

        yield return new WaitForSeconds(_gridManager.moveDuration);
        yield return new WaitForSeconds(delayDuringSwap);

        _gridManager.SwapPieces(piece1, piece2);

        bool matchesResolved = _gridManager.CheckAndProcessMatches(piece1, piece2);

        if (!matchesResolved)
        {
            // Revert swap if no matches
            piece1.transform.DOMove(pos1, _gridManager.moveDuration);
            piece2.transform.DOMove(pos2, _gridManager.moveDuration);

            yield return new WaitForSeconds(_gridManager.moveDuration);
            yield return new WaitForSeconds(delayDuringSwap);

            _gridManager.SwapPieces(piece1, piece2);
        }
        else
        {
            // Checar se Amora foi envolvida na troca.
            if (piece1 is AmoraPiece)
            {
                _powerUpManager.ActivateAmora(piece1, piece2);
            }
            else if (piece2 is AmoraPiece)
            {
                _powerUpManager.ActivateAmora(piece2, piece1);
            }
        }

        piece1.SetSelected(false);
        piece2.SetSelected(false);
        _selectedPiece = null;

        isSwapping = false;
    }
}