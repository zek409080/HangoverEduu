using UnityEngine;

public class CutsceneConfiguration : MonoBehaviour
{
    public Sprite Faz1, Faz2;
    public Sprite FLo;
    public Sprite Cutscene1Background; // Adiciona referência ao fundo da Cutscene1

    public Cutscene[] cutscenes;

    void Awake()
    {
        cutscenes = new Cutscene[]
        {
            new Cutscene
            {
                id = "Cutscene1",
                background = Cutscene1Background, // Define o fundo da cutscene
                dialogs = new Dialog[]
                {
                    new Dialog
                    {
                        characterName = "Antônio",
                        characterSprite = Faz1,
                        lines = new string[]
                        {
                            "Ah, meu amigo... Que tragédia eu causei. Terra do Sol era um paraíso, e agora olha só pra isso!",
                            "Poluição por todo lado, água escassa, e nem as frutas conseguem crescer direito."
                        }
                    },
                    new Dialog
                    {
                        characterName = "Floquinho",
                        characterSprite = FLo,
                        lines = new string[]
                        {
                            "[Floquinho abana o rabo e dá um latido animado, tentando animar Antônio.]"
                        }
                    },
                    new Dialog
                    {
                        characterName = "Antônio",
                        characterSprite = Faz2,
                        lines = new string[]
                        {
                            "Hehe, você tem razão, Floquinho. Não adianta ficar lamentando. Precisamos começar a agir!",
                            "E olha só, nosso novo ajudante chegou na hora certa. Vamos trabalhar juntos pra consertar esse estrago."
                        }
                    },
                    new Dialog
                    {
                        characterName = "Floquinho",
                        characterSprite = FLo,
                        lines = new string[]
                        {
                            "[Floquinho corre em círculos e aponta com o focinho para as frutas no campo.]"
                        }
                    },
                    new Dialog
                    {
                        characterName = "Antônio",
                        characterSprite = Faz2,
                        lines = new string[]
                        {
                            "Isso mesmo, Floquinho! Nosso primeiro passo é recolher essas frutas.",
                            "Assim, podemos começar a limpar e recuperar nossa fazenda. Vamos nessa!"
                        }
                    }
                }
            }
        };
    }
}