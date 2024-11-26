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
        if (isSwapping) return; // Exit if a swap is already in progress
        
        if (_selectedPiece == null)
        {
            SetSelectedPiece(piece);
        }
        else if (_selectedPiece == piece)
        {
            DeselectPiece();
        }
        else if (_gridManager.AreAdjacent(_selectedPiece, piece))
        {
            StartCoroutine(SwapPieces(_selectedPiece, piece));
        }
        else
        {
            SetSelectedPiece(piece);
        }
    }

    private void SetSelectedPiece(Piece piece)
    {
        DeselectPiece();
        _selectedPiece = piece;
        _selectedPiece.SetSelected(true);
        Debug.Log("Piece Selected: " + piece.name);
    }

    private void DeselectPiece()
    {
        if (_selectedPiece != null)
        {
            _selectedPiece.SetSelected(false);
            _selectedPiece = null;
        }
    }

    private IEnumerator SwapPieces(Piece piece1, Piece piece2)
{
    isSwapping = true;

    Vector3 pos1 = piece1.transform.position;
    Vector3 pos2 = piece2.transform.position;

    piece1.transform.DOMove(pos2, _gridManager.moveDuration).SetEase(Ease.InOutQuad);
    piece2.transform.DOMove(pos1, _gridManager.moveDuration).SetEase(Ease.InOutQuad);

    yield return new WaitForSeconds(_gridManager.moveDuration + delayDuringSwap);

    _gridManager.SwapPieces(piece1, piece2);

    // Verificação especial para Amora
    bool amoraActivated = false;
    if (piece1 is AmoraPiece)
    {
        Debug.Log("Amora encontrada no piece1. Ativando PowerUp.");
        _powerUpManager.ActivateAmora((AmoraPiece)piece1, piece2);
        amoraActivated = true;
    }
    if (piece2 is AmoraPiece)
    {
        Debug.Log("Amora encontrada no piece2. Ativando PowerUp.");
        _powerUpManager.ActivateAmora((AmoraPiece)piece2, piece1);
        amoraActivated = true;
    }

    if (amoraActivated)
    {
        yield return new WaitForSeconds(0.5f); // Aguardar um pouco para a animação do PowerUp
        StartCoroutine(_gridManager.ResetMatching()); // Reiniciando o matching após a ativação da Amora
    }
    else
    {
        bool isMatch1 = _gridManager.CheckAndProcessMatches(piece1); // Modificado para checar um de cada vez
        bool isMatch2 = _gridManager.CheckAndProcessMatches(piece2); // Modificado para checar um de cada vez
        
        bool isMatch = isMatch1 || isMatch2;

        if (!isMatch)
        {
            piece1.transform.DOMove(pos1, _gridManager.moveDuration).SetEase(Ease.InOutQuad);
            piece2.transform.DOMove(pos2, _gridManager.moveDuration).SetEase(Ease.InOutQuad);

            yield return new WaitForSeconds(_gridManager.moveDuration + delayDuringSwap);

            _gridManager.SwapPieces(piece1, piece2);
        }
        else
        {
            // Decrementar jogadas somente se houve algum match
            GameManager.DecrementJogadas();
        }
    }

    piece1.SetSelected(false);
    piece2.SetSelected(false);
    _selectedPiece = null;
    isSwapping = false;
}
}