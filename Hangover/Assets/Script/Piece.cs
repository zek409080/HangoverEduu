using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public FrutType frutType;
    public int x;
    public int y;
    public Board board;
    public bool isInvisible;
    public AudioSource audioSelect;
    private Vector3 targetPosition;
    private float moveDuration;
    private bool isMoving = false;
    private Vector3 targetScale;
    private bool isScaling = false;
    private float scaleDuration;
    public void Init(int x, int y, Board board)
    {
        audioSelect = GetComponent<AudioSource>();
        this.x = x;
        this.y = y;
        this.board = board;
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / moveDuration);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }
    public void StartScaleAnimation(Vector3 newScale, float duration)
    {
        targetScale = newScale;
        scaleDuration = duration;
        isScaling = true;
    }

    void OnMouseDown()
    {
        if (!isInvisible && frutType != FrutType.Vazio)
        {
            audioSelect.Play();
            board.SelectPiece(this);
        }
    }

    public void StartMoveAnimation(Vector3 newPosition, float duration)
    {
        targetPosition = newPosition;
        moveDuration = duration;
        isMoving = true;
        StartCoroutine(MoveToPosition());
    }

    private IEnumerator MoveToPosition()
    {
        float elapsedTime = 0;
        Vector3 startingPos = transform.position;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startingPos, targetPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }

    public void ActivatePower()
    {
        switch (frutType)
        {
            case FrutType.Framboesa:
                ExplodeHorizontal();
                break;
            case FrutType.Roma:
                ExplodeVertical();
                break;
            case FrutType.Cereja:
                ExplodeSquare();
                break;
            default:
                break;
        }
    }

    private void ExplodeHorizontal()
    {
        for (int i = 0; i < board.width; i++)
        {
            Piece piece = board.pieces[i, y];
            
        }
    }

    private void ExplodeVertical()
    {
        for (int j = 0; j < board.height; j++)
        {
            Piece piece = board.pieces[x, j];
            
        }
    }

    private void ExplodeSquare()
    {
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i >= 0 && i < board.width && j >= 0 && j < board.height)
                {
                    Piece piece = board.pieces[i, j];
                }
            }
        }
    }
}

// Enumeração para os tipos de frutas disponíveis
public enum FrutType
{
    Vazio,
    Roma,      
    Cereja,    
    Framboesa, 
    Abacaxi,
    Banana,
    Manga,
    Maca,
    Melancia,
    Pinha,
    Uva
}