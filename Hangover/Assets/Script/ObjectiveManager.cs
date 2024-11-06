using System.Collections.Generic; // Certifique-se de incluir isto
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ObjectiveManager : MonoBehaviour
{
    public enum ObjectiveType
    {
        Score,
        PieceCount
    }

    [System.Serializable]
    public class Objective
    {
        public ObjectiveType type;
        public int targetValue;
        public FrutType targetPiece; // Use para o objetivo de contagem de peças
        public bool isCompleted; // Adiciona um campo para marcar objetivos como completos
    }

    public Objective[] objectives;
    private int currentScore;
    private Dictionary<FrutType, int> pieceCounts;

    public UnityEvent onObjectivesCompleted;

    // Mapa de imagens para cada tipo de fruta
    public Dictionary<FrutType, Sprite> fruitSprites = new Dictionary<FrutType, Sprite>();
    public Image fruitImage; // Referência ao componente Image na UI

    private void Start()
    {
        currentScore = 0;
        pieceCounts = new Dictionary<FrutType, int>();

        foreach (FrutType frutType in System.Enum.GetValues(typeof(FrutType)))
        {
            pieceCounts[frutType] = 0;
        }
    }

    public void AddScore(int score)
    {
        currentScore += score;
        CheckObjectives();
    }

    public void AddPieceCount(FrutType frutType)
    {
        if (pieceCounts.ContainsKey(frutType))
        {
            pieceCounts[frutType]++;
            CheckObjectives();
            UpdateFruitImage(frutType);
        }
    }

    private void CheckObjectives()
    {
        foreach (var objective in objectives)
        {
            if (!objective.isCompleted)
            {
                if (objective.type == ObjectiveType.Score && currentScore >= objective.targetValue)
                {
                    OnObjectiveCompleted(objective);
                }
                else if (objective.type == ObjectiveType.PieceCount && pieceCounts[objective.targetPiece] >= objective.targetValue)
                {
                    OnObjectiveCompleted(objective);
                }
            }
        }
    }

    private void OnObjectiveCompleted(Objective objective)
    {
        objective.isCompleted = true;
        Debug.Log($"Objetivo completado! Tipo: {objective.type}, Valor Alvo: {objective.targetValue}");

        // Checar se todos os objetivos foram completados
        if (AllObjectivesCompleted())
        {
            onObjectivesCompleted.Invoke();
        }
    }

    private bool AllObjectivesCompleted()
    {
        foreach (var objective in objectives)
        {
            if (!objective.isCompleted)
            {
                return false;
            }
        }
        return true;
    }

    private void UpdateFruitImage(FrutType frutType)
    {
        if (fruitSprites.ContainsKey(frutType) && fruitImage != null)
        {
            fruitImage.sprite = fruitSprites[frutType];
        }
    }
}