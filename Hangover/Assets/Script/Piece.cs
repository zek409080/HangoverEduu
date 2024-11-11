﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public FrutType frutType;
    public int x;
    public int y;
    public Board board;
    public bool isInvisible;
    public bool isPowerUp = false;
    public AudioSource audioSelect;


    private Vector3 targetPosition;
    private Vector3 targetScale;
    private float moveDuration;
    private float scaleDuration;
    private bool isScaling = false;
    private bool isMoving = false;
    private Renderer pieceRenderer;
    public void SetVisibility(bool isVisible)
    {
        if (pieceRenderer != null)
        {
            pieceRenderer.enabled = isVisible;
        }
    }
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
        if (isScaling)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime / scaleDuration);
            if (Vector3.Distance(transform.localScale, targetScale) < 0.01f)
            {
                transform.localScale = targetScale;
                isScaling = false;
            }
        }
    }


    void OnMouseDown()
    {
        if (!isInvisible && frutType != FrutType.Vazio)
        {
            if (MusicUI.instance.estadoDoSom)
            {
                audioSelect.Play();
            }
            board.SelectPiece(this);
        }
    }
    public void MoveToPosition(Vector3 targetPosition, float duration)
    {
        StartCoroutine(MoveCoroutine(targetPosition, duration));
    }

    private IEnumerator MoveCoroutine(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = transform.position;
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // Garante que a peça chegue à posição final
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

    public void StartScaleAnimation(Vector3 newScale, float duration)
    {
        targetScale = newScale;
        scaleDuration = duration;
        isScaling = true;
    }
}
// Enumeração para os tipos de frutas disponíveis
public enum FrutType
{
    Abacaxi,
    Banana,
    Manga,
    Maca,
    Melancia,
    Pinha,
    Uva,
    Roma,     // Poder
    Cereja,   // Poder
    Franboesa, // Poder
    PowerUp,  // Novo tipo para `PowerUps`
    Vazio
}
