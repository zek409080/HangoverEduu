using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PieceSwapper : MonoBehaviour
{
    private GridManager _gridManager;
    private Piece _selectedPiece = null;
    [SerializeField]
    private float delayDuringSwap = 0.2f;
    private bool isSwapping = false; // Estado para controlar se a troca está em andamento

    private void Start()
    {
        _gridManager = FindObjectOfType<GridManager>();

        if (_gridManager == null)
        {
            Debug.LogError("GridManager not found in the scene. Please ensure there is a GridManager object in the scene.");
        }
    }

    public void SelectPiece(Piece piece)
    {
        if (isSwapping) return; // Se uma troca está em andamento, não permite cliques
        if (_selectedPiece == null)
        {
            SelectNewPiece(piece);
        }
        else if (_selectedPiece == piece)
        {
            DeselectCurrentPiece();
        }
        else if (_gridManager.AreAdjacent(_selectedPiece, piece))
        {
            StartCoroutine(SwapPieces(_selectedPiece, piece));
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
            Debug.Log("Deselected Piece: " + _selectedPiece.name);
            _selectedPiece = null;
        }
    }

    private IEnumerator SwapPieces(Piece piece1, Piece piece2)
    {
        isSwapping = true; // Iniciando a troca, desativar detecção de cliques

        Vector3 pos1 = piece1.transform.position;
        Vector3 pos2 = piece2.transform.position;

        // Animate the swap
        piece1.transform.DOMove(pos2, _gridManager.moveDuration);
        piece2.transform.DOMove(pos1, _gridManager.moveDuration);

        yield return new WaitForSeconds(_gridManager.moveDuration);

        // Delay during swap
        yield return new WaitForSeconds(delayDuringSwap);

        // Actually swap the pieces in the grid
        _gridManager.SwapPieces(piece1, piece2);

        // Check for matches after swapping pieces
        if (!_gridManager.CheckAndProcessMatches(piece1, piece2))
        {
            // If no match, swap them back
            piece1.transform.DOMove(pos1, _gridManager.moveDuration);
            piece2.transform.DOMove(pos2, _gridManager.moveDuration);

            yield return new WaitForSeconds(_gridManager.moveDuration);
            yield return new WaitForSeconds(delayDuringSwap);

            _gridManager.SwapPieces(piece1, piece2);
        }

        piece1.SetSelected(false);
        piece2.SetSelected(false);
        _selectedPiece = null;

        isSwapping = false; // Troca concluída, reativar detecção de cliques
    }
}