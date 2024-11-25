using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogManager : MonoBehaviour
{
    public Image leftCharacter;
    public Image rightCharacter;
    public TextMeshProUGUI dialogText;
    public Button nextButton;
    public Image background; // Referência para a imagem de fundo

    private Queue<string> sentences;
    private Dialog currentDialog;
    private int currentDialogIndex = 0;
    private string currentCutsceneId; // Variável para rastrear o ID da cutscene atual
    private Cutscene currentCutscene; // Variável para rastrear a cutscene atual
    private Cutscene[] cutscenes;

    private void Start()
    {
        sentences = new Queue<string>();
        nextButton.onClick.AddListener(DisplayNextSentence);

        // Obter cutscenes a partir do CutsceneConfiguration
        cutscenes = FindObjectOfType<CutsceneConfiguration>().cutscenes;
        
        // Descobrir qual cutscene tocar com base no estado salvo
        LoadNextCutscene();
    }

    private void LoadNextCutscene()
    {
        string nextCutsceneId = GetNextCutsceneId();
        if (!string.IsNullOrEmpty(nextCutsceneId))
        {
            LoadCutscene(nextCutsceneId);
        }
        else
        {
            SceneManager.LoadScene("selecaoDeFase");
        }
    }

    private string GetNextCutsceneId()
    {
        foreach (var cutscene in cutscenes)
        {
            if (!PlayerPrefs.HasKey(cutscene.id))
            {
                return cutscene.id;
            }
        }
        return null;
    }

    public void LoadCutscene(string cutsceneId)
    {
        foreach (var cutscene in cutscenes)
        {
            if (cutscene.id == cutsceneId)
            {
                currentCutscene = cutscene;
                currentCutsceneId = cutsceneId;
                SetBackgroundForCutscene(cutscene); 
                StartCutscene(cutscene.dialogs);
                break;
            }
        }
    }

    private void SetBackgroundForCutscene(Cutscene cutscene)
    {
        background.sprite = cutscene.background; 
    }

    private void StartCutscene(Dialog[] dialogs)
    {
        if (dialogs.Length == 0) return;

        currentDialogIndex = 0;
        StartDialog(dialogs[currentDialogIndex]);
    }

    private void StartDialog(Dialog dialog)
    {
        sentences.Clear();

        currentDialog = dialog;
        foreach (string sentence in dialog.lines)
        {
            sentences.Enqueue(sentence);
        }

        UpdateCharacterImages();

        dialogText.text = "";
        DisplayNextSentence();
    }

    private void UpdateCharacterImages()
    {
        if (currentDialog != null)
        {
            if (currentDialog.characterName == "Antônio")
            {
                leftCharacter.sprite = currentDialog.characterSprite;
                rightCharacter.color = new Color(1, 1, 1, 0.5f); 
                leftCharacter.color = new Color(1, 1, 1, 1); 
            }
            else if (currentDialog.characterName == "Floquinho")
            {
                rightCharacter.sprite = currentDialog.characterSprite;
                leftCharacter.color = new Color(1, 1, 1, 0.5f); 
                rightCharacter.color = new Color(1, 1, 1, 1);
            }
        }
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            currentDialogIndex++;
            if (currentDialogIndex >= currentCutscene.dialogs.Length)
            {
                EndDialog();
                return;
            }
            else
            {
                StartDialog(currentCutscene.dialogs[currentDialogIndex]);
                return;
            }
        }

        string sentence = sentences.Dequeue();
        dialogText.text = sentence;
    }

    private void EndDialog()
    {
        // Marcar a cutscene atual como vista
        PlayerPrefs.SetInt(currentCutsceneId, 1);

        // Verificar se é a última cutscene
        if (currentCutsceneId == "Cutscene3")
        {
            // Carregar a cena de créditos
            SceneManager.LoadScene("Credits");
        }
        else
        {
            // Carregar a cena de seleção de mapa ao final da cutscene
            SceneManager.LoadScene("selecaoDeFase");
        }
    }
}

[System.Serializable]
public class Dialog
{
    public string characterName;
    public Sprite characterSprite;
    public string[] lines;
}

[System.Serializable]
public class Cutscene
{
    public string id;
    public Sprite background; 
    public Dialog[] dialogs;
}