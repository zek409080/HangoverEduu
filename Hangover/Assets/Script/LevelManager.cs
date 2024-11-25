using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            EnsureLevel1Unlocked();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Garante que a fase 1 esteja sempre desbloqueada
    private void EnsureLevel1Unlocked()
    {
        PlayerPrefs.SetInt("Level1_unlocked", 1);
        PlayerPrefs.Save();
    }

    // Verifica se a fase está desbloqueada
    public bool IsLevelUnlocked(int level)
    {
        return PlayerPrefs.GetInt($"Level{level}_unlocked", level == 1 ? 1 : 0) == 1;
    }

    // Desbloqueia a próxima fase
    public void UnlockNextLevel(int completedLevel)
    {
        int nextLevel = completedLevel + 1;
        PlayerPrefs.SetInt($"Level{nextLevel}_unlocked", 1);
        PlayerPrefs.Save();
        Debug.Log($"Nível {nextLevel} foi desbloqueado.");
    }

    // Desbloqueia uma fase específica
    public void UnlockLevel(int level)
    {
        PlayerPrefs.SetInt($"Level{level}_unlocked", 1);
        PlayerPrefs.Save();
        Debug.Log($"Nível {level} foi desbloqueado.");
    }

    // Reseta o progresso (utilizado para debug e testes)
    public void ResetLevels()
    {
        PlayerPrefs.DeleteAll();
        EnsureLevel1Unlocked();
        Debug.Log("Progresso de fases foi resetado.");
    }
}