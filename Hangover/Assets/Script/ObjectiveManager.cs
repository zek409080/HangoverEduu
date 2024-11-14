using System.Collections.Generic;
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
        public FrutType targetPiece; // Usado para o objetivo de contagem de peças
        public bool isCompleted;
    }

    public Objective[] objectives;
    private int currentScore;
    private Dictionary<FrutType, int> pieceCounts;

    public UnityEvent onObjectivesCompleted;

    // Mapa de imagens para cada tipo de fruta
    public Dictionary<FrutType, Sprite> fruitSprites = new Dictionary<FrutType, Sprite>();
    public Image fruitImage; // Referência ao componente Image na UI
    public Text piecesLeftText; // Referência ao componente Text na UI

    private void Start()
    {
        currentScore = 0;
        pieceCounts = new Dictionary<FrutType, int>();

        foreach (FrutType frutType in System.Enum.GetValues(typeof(FrutType)))
        {
            pieceCounts[frutType] = 0;
        }

        if (fruitSprites == null)
        {
            fruitSprites = new Dictionary<FrutType, Sprite>();
        }

        // Atualiza a imagem da UI e o texto para o primeiro objetivo que precisa ser completado
        UpdateUIForNextObjective();
    }

    public void AddScore(int score)
    {
        currentScore += score;
        CheckObjectives();
    }
    
    public bool AreAllObjectivesCompleted()
    {
        return AllObjectivesCompleted();
    }

    public void AddPieceCount(FrutType frutType)
    {
        if (pieceCounts.ContainsKey(frutType))
        {
            pieceCounts[frutType]++;
            CheckObjectives();
            UpdateUIForNextObjective();
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

        // Atualiza a imagem da UI e notifica o UIManager depois de verificar os objetivos
        UpdateUIForNextObjective();
        FindObjectOfType<UIManager>()?.UpdateObjectivesText();
    }

    private void OnObjectiveCompleted(Objective objective)
    {
        objective.isCompleted = true;
        Debug.Log($"Objetivo completado! Tipo: {objective.type}, Valor Alvo: {objective.targetValue}");

        // Mostrar um pop-up para o jogador indicando que o objetivo foi completado
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowObjectiveCompletedPopup($"Objetivo completado: {objective.type} {objective.targetValue}");
        }

        // Verifica se todos os objetivos foram concluídos
        if (AllObjectivesCompleted())
        {
            Debug.Log("Todos os objetivos foram cumpridos!");
            onObjectivesCompleted?.Invoke();
        }
    }

    public bool AllObjectivesCompleted()
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

    public void UpdateFruitImage(FrutType frutType)
    {
        if (fruitSprites.ContainsKey(frutType) && fruitImage != null)
        {
            fruitImage.sprite = fruitSprites[frutType];
        }
    }

    private void UpdateUIForNextObjective()
    {
        foreach (var objective in objectives)
        {
            if (!objective.isCompleted && objective.type == ObjectiveType.PieceCount)
            {
                UpdateFruitImage(objective.targetPiece);
                UpdatePiecesLeftText(objective.targetPiece, objective.targetValue - pieceCounts[objective.targetPiece]);
                break;
            }
        }
    }

    private void UpdatePiecesLeftText(FrutType frutType, int piecesLeft)
    {
        if (piecesLeftText != null)
        {
            piecesLeftText.text = $"Faltam {piecesLeft} peças de {frutType}";
        }
    }

    public void ResetObjectives()
    {
        foreach (var objective in objectives)
        {
            objective.isCompleted = false;
        }

        // Reiniciar as contagens de peças
        currentScore = 0;
        foreach (FrutType frutType in System.Enum.GetValues(typeof(FrutType)))
        {
            pieceCounts[frutType] = 0;
        }

        // Atualizar a UI para refletir o estado reiniciado
        UpdateUIForNextObjective();
    }
    
    public int GetPieceCount(FrutType frutType)
    {
        if (pieceCounts.ContainsKey(frutType))
        {
            return pieceCounts[frutType];
        }
        return 0;
    }
    
}