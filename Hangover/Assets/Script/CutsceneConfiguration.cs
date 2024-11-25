using UnityEngine;

public class CutsceneConfiguration : MonoBehaviour
{
    public Sprite Faz1, Faz2, Faz3;
    public Sprite FLo;

    public Sprite
        Cutscene1Background, Cutscene2Background, Cutscene3Background; // Adiciona referência ao fundo da Cutscene1

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
            },
            new Cutscene
            {
                id = "Cutscene2",
                background = Cutscene2Background,
                dialogs = new Dialog[]
                {
                    new Dialog
                    {
                        characterName = "Antônio",
                        characterSprite = Faz2,
                        lines = new string[]
                        {
                            "Ufaaa... Estamos indo bem, olha só essa área aqui!",
                            "As frutas estão crescendo de novo, e o solo tá começando a respirar. É um bom começo."
                        }
                    },
                    new Dialog
                    {
                        characterName = "Floquinho",
                        characterSprite = FLo,
                        lines = new string[]
                        {
                            "[Floquinho se deita perto de uma árvore saudável, dando um latido contente.]"
                        }
                    },
                    new Dialog
                    {
                        characterName = "Antônio",
                        characterSprite = Faz2,
                        lines = new string[]
                        {
                            "Hehe! Até o Floquinho tá aliviado, mas não podemos relaxar agora.",
                            "Ainda tem muita terra pra limpar, e as coisas vão ficando mais complicadas daqui pra frente."
                        }
                    },
                    new Dialog
                    {
                        characterName = "Floquinho",
                        characterSprite = FLo,
                        lines = new string[]
                        {
                            "[Floquinho se levanta de repente, farejando algo e correndo até um monte de lixo tóxico.]"
                        }
                    },
                    new Dialog
                    {
                        characterName = "Antônio",
                        characterSprite = Faz3,
                        lines = new string[]
                        {
                            "Tá vendo isso? Esses resíduos da empresa não vão sumir sozinhos.",
                            "Precisamos continuar juntando frutas e mostrando pra essa terra que ainda há esperança!"
                        }
                    },
                    new Dialog
                    {
                        characterName = "Floquinho",
                        characterSprite = FLo,
                        lines = new string[]
                        {
                            "[Floquinho olha para Antônio, balança o rabo, e volta para perto dele, trazendo um pequeno galho saudável entre os dentes.]"
                        }
                    },
                    new Dialog
                    {
                        characterName = "Antônio",
                        characterSprite = Faz2,
                        lines = new string[]
                        {
                            "Hehe, Floquinho tá dizendo que a gente tem força pra superar qualquer coisa.",
                            "E eu acredito nele. Vamos em frente, e não se esqueça: cada passo conta!"
                        }
                    }
                }
            },
            new Cutscene
            {
                id = "Cutscene3",
                background = Cutscene3Background,
                dialogs = new Dialog[]
                {
                    new Dialog
                    {
                        characterName = "Antônio",
                        characterSprite = Faz2,
                        lines = new string[]
                        {
                            "[Antônio e Floquinho estão no centro da fazenda completamente restaurada. O campo está cheio de árvores frutíferas, o céu limpo, e um riacho cristalino corre ao fundo.]",
                            "Suspiro emocionado... Nós conseguimos! Olha só pra isso... ",
                            "A Terra do Sol tá viva de novo! As frutas tão radiantes, a terra tá saudável, e até o ar parece mais leve"
                        }
                    },
                    new Dialog
                    {
                        characterName = "Floquinho",
                        characterSprite = FLo,
                        lines = new string[]
                        {
                            "[Floquinho corre feliz pelo campo, latindo animadamente e pegando uma maçã que caiu do pé.]"
                        }
                    },
                    new Dialog
                    {
                        characterName = "Antônio",
                        characterSprite = Faz2,
                        lines = new string[]
                        {
                            "Haha! Parece que até o Floquinho tá comemorando. E com razão!",
                            " Você foi incrível. Se não fosse pela sua ajuda, essa fazenda nunca teria se recuperado.",
                            "Agora eu sei... Essa terra não é só minha. Ela pertence a todos que cuidam dela e a respeitam.",
                            "Nunca mais vou deixar algo assim acontecer. Essa fazenda é nosso legado, meu e seu, Obrigado, de coração"
                        }
                    },
                    new Dialog
                    {
                        characterName = "Floquinho",
                        characterSprite = FLo,
                        lines = new string[]
                        {
                            "[Floquinho dá um latido forte, como se concordasse, e Antônio ri enquanto o abraça.]"
                        }
                    },
                    new Dialog
                    {
                        characterName = "Antônio",
                        characterSprite = Faz2,
                        lines = new string[]
                        {
                            "Mas chega de conversa! Que tal um suco de frutas fresquinhas pra comemorar? Hehe! Você merece."
                        }
                    }
                }
            }
        };
    }
}