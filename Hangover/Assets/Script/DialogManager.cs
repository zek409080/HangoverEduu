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

    // Carrega a próxima cutscene com base no estado salvo
    private void LoadNextCutscene()
    {
        string nextCutsceneId = GetNextCutsceneId();
        if (!string.IsNullOrEmpty(nextCutsceneId))
        {
            LoadCutscene(nextCutsceneId);
        }
        else
        {
            // Carregar a cena de seleção de fase se não houver mais cutscenes
            SceneManager.LoadScene("selecaoDeFase");
        }
    }

    // Descobre qual é a próxima cutscene a ser tocada
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
        // Encontrar e carregar a cutscene correspondente ao ID fornecido
        foreach (var cutscene in cutscenes)
        {
            if (cutscene.id == cutsceneId)
            {
                PlayerPrefs.SetInt(cutsceneId, 1); // Marcar a cutscene como vista

                currentCutscene = cutscene;
                currentCutsceneId = cutsceneId;
                SetBackgroundForCutscene(cutscene); // Define a imagem de fundo para a cutscene
                StartCutscene(cutscene.dialogs);
                break;
            }
        }
    }

    private void SetBackgroundForCutscene(Cutscene cutscene)
    {
        background.sprite = cutscene.background; // Define o sprite do fundo a partir da cutscene
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

        UpdateCharacterImages(); // Atualizar as imagens dos personagens ao iniciar um novo diálogo

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
                rightCharacter.color = new Color(1, 1, 1, 0.5f); // Deixa o personagem da direita transparente
                leftCharacter.color = new Color(1, 1, 1, 1); // Personagem da esquerda totalmente opaco
            }
            else if (currentDialog.characterName == "Floquinho")
            {
                rightCharacter.sprite = currentDialog.characterSprite;
                leftCharacter.color = new Color(1, 1, 1, 0.5f); // Deixa o personagem da esquerda transparente
                rightCharacter.color = new Color(1, 1, 1, 1); // Personagem da direita totalmente opaco
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
        // Verificar se é a última cutscene
        if (currentCutsceneId == "Cutscene3")
        {
            // Carregar a cena de créditos
            SceneManager.LoadScene("Credits");
        }
        else
        {
            // Salvar o estado atual da cutscene
            PlayerPrefs.SetInt(currentCutsceneId, 1);

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
    public string id; // ID único para identificar a cutscene
    public Sprite background; // Fundo para a cutscene
    public Dialog[] dialogs; // Array de diálogos que compõem a cutscene
}