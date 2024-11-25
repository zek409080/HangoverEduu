using UnityEngine;

[System.Serializable]
public class FruitSprite
{
    public FrutType frutType;
    public Sprite sprite;
}

public class FruitSpriteManager : MonoBehaviour
{
    public FruitSprite[] fruitSpritesArray;
    private ObjectiveManager objectiveManager;

    private void Awake()
    {
        objectiveManager = GetComponent<ObjectiveManager>();

        // Inicializa fruitSprites com os valores do array
        foreach (var fruitSprite in fruitSpritesArray)
        {
            objectiveManager.fruitSprites[fruitSprite.frutType] = fruitSprite.sprite;
        }
    }
}