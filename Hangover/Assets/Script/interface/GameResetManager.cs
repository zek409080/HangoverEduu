using UnityEngine;

public class GameResetManager : MonoBehaviour
{
    public void ResetAllData()
    {
        // Resetar dados de energia
        EnergyManager.instance.ResetEnergy();

        // Resetar níveis desbloqueados
        LevelManager.instance.ResetLevels();

        // Resetar pontuações altas
        HighScoresManager.instance.ResetHighScores();
    }

    [ContextMenu("ResetAllData")]
    public void ResetAllDataContextMenu()
    {
        ResetAllData();
    }
}