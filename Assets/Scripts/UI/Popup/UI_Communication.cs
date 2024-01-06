using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_Communication : UI_Popup
{
    public static UI_Communication instance;

    public Sprite[] BubbleIMGs;
    public Sprite[] CharIMGs;
    TMPro.TMP_Text dialogueText;
    public List<Dialogue> dialogues;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    public float TypingDelay;
    private WaitForSeconds typingDelay;
    private int currentDialogueIndex = 0;

    public float TargetX;
    public float MoveTime;
    public float PeriodScale;

    bool isEnd;

    enum Texts { sentenceTMP, Option1TMP, Option2TMP }
    enum Images 
    { LeftIMG, RightIMG,
        ChatBubbleIMG
    }

    enum Buttons
    {
        Option1BTN,
        Option2BTN
    }


    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        Init();
        typingDelay = new WaitForSeconds(TypingDelay);
    }

    public void StartDiagloue(List<Dialogue> DiaList)
    {
        dialogues = DiaList;
        ShowDialogue(dialogues[0]);
    }

    public override void Init()
    {
        base.Init();
        Bind<TMPro.TMP_Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));

        GetImage((int)Images.LeftIMG).gameObject.SetActive(false);
        GetImage((int)Images.RightIMG).gameObject.SetActive(false);
        dialogueText = GetText((int)Texts.sentenceTMP);
    }

    void Update()
    {
        // ȭ���� ��ġ�ϸ� ���� ���� �Ѿ��
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began
            || Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // ���� ��� ���̶�� ��� ��� �� ���� �����ֱ�
                StopCoroutine(typingCoroutine);
                dialogueText.text = dialogues[currentDialogueIndex].sentence;
                isTyping = false;
            }
            else
            {
                // ���� ���� �Ѿ��
                NextDialogue();
            }
        }
    }

    void ShowDialogue(Dialogue dialogue)
    {
        // ��� ����� ���� �ڷ�ƾ ����
        typingCoroutine = StartCoroutine(TypeSentence(dialogue));
    }

    void NextDialogue()
    {
        if(!isEnd)
        {
            // ���� ��� �ε����� �̵�
            currentDialogueIndex++;

            // ��簡 ��� ������ ��� ��ȭ ����
            if (currentDialogueIndex >= dialogues.Count)
            {
                EndDialogue();
                return;
            }

            //��ư�̸� ��ư ���
            if (dialogues[currentDialogueIndex].name == "1")
            {
                ShowOptionBTN();
                return;
            }

            // ���� ��� ���
            ShowDialogue(dialogues[currentDialogueIndex]);
        }
    }

    void EndDialogue()
    {
        Managers.UI_Manager.ClosePopupUI();
    }

    

    IEnumerator TypeSentence(Dialogue dialogue)
    {
        ChooseBubbleIMG(dialogue);
        ShowImage(dialogue);

        // ���� ��� ������ ǥ��
        isTyping = true;

        // ��� �ʱ�ȭ
        dialogueText.text = "";

        int chatIndex = 0; 

        foreach (char letter in dialogue.sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return typingDelay;

            Managers.Sound.Play(GetChatSound(chatIndex));
            chatIndex++;
        }

        // ���� ��� �Ϸ� �� ��� ���� ������Ʈ
        isTyping = false;
    }

    public bool SoundSwitchMode = false;

    Define.Sound GetChatSound(int index)
    {

        if (!SoundSwitchMode) return Define.Sound.Chat1;

        switch (index % 3)
        {
            case 0:
                return Define.Sound.Chat1;
            case 1:
                return Define.Sound.Chat2;
            case 2:
                return Define.Sound.Chat3;
            default:
                return Define.Sound.Bgm;
        }
    }

    void ChooseBubbleIMG(Dialogue dialogue)
    {
        if(dialogue.name == "����")
        {
            if (dialogue.isLeft) GetImage((int)Images.ChatBubbleIMG).sprite = BubbleIMGs[0];
            else GetImage((int)Images.ChatBubbleIMG).sprite = BubbleIMGs[1];
        }
        else if(dialogue.name == "����")
        {
            if (dialogue.isLeft) GetImage((int)Images.ChatBubbleIMG).sprite = BubbleIMGs[2];
            else GetImage((int)Images.ChatBubbleIMG).sprite = BubbleIMGs[3];
        }
        else
        {
            if (dialogue.isLeft) GetImage((int)Images.ChatBubbleIMG).sprite = BubbleIMGs[4];
            else GetImage((int)Images.ChatBubbleIMG).sprite = BubbleIMGs[5];
        }
    }

    void ShowImage(Dialogue dialogue)
    {
        if (dialogue.name == "����")
        {
            TurnOnImage(dialogue.isLeft, CharIMGs[0]);
        }
        else if (dialogue.name == "����")
        {
            TurnOnImage(dialogue.isLeft, CharIMGs[1]);
        }
        else
        {
            TurnOnImage(dialogue.isLeft, CharIMGs[2]);
        }
    }

    void TurnOnImage(bool isLeft, Sprite sprite)
    {
        if (isLeft)
        {
            GetImage((int)Images.LeftIMG).gameObject.SetActive(true);
            GetImage((int)Images.LeftIMG).sprite = sprite;
            GetImage((int)Images.LeftIMG).color = Color.white;
            GetImage((int)Images.RightIMG).color = Color.gray;
        }
        else
        {
            GetImage((int)Images.RightIMG).gameObject.SetActive(true);
            GetImage((int)Images.RightIMG).sprite = sprite;
            GetImage((int)Images.LeftIMG).color = Color.gray;
            GetImage((int)Images.RightIMG).color = Color.white;
        }
    }

    void ShowOptionBTN()
    {
        isEnd = true;
        GetButton((int)Buttons.Option1BTN).transform.DOLocalMoveX(TargetX, MoveTime).SetEase(Ease.OutElastic, 0, PeriodScale);
        GetButton((int)Buttons.Option2BTN).transform.DOLocalMoveX(TargetX, MoveTime).SetEase(Ease.OutElastic, 0, PeriodScale);
        GetText((int)Texts.Option1TMP).text = dialogues[currentDialogueIndex].sentence;
        GetText((int)Texts.Option2TMP).text = dialogues[currentDialogueIndex + 1].sentence;

        if (Managers.Data._myPlayerData.nowGoldAmount >= dialogues[currentDialogueIndex].CostGold)
        {
            Debug.Log(dialogues[currentDialogueIndex].CostGold);
            GetButton((int)Buttons.Option1BTN).interactable = true;
        }
        else
        {
            GetButton((int)Buttons.Option1BTN).interactable = false;
            GetButton((int)Buttons.Option1BTN).GetComponent<Image>().sprite = GetButton((int)Buttons.Option1BTN).spriteState.pressedSprite;
        }

        if (Managers.Data._myPlayerData.nowGoldAmount >= dialogues[currentDialogueIndex + 1].CostGold)
        {
            GetButton((int)Buttons.Option2BTN).interactable = true;
        }
        else
        {
            GetButton((int)Buttons.Option2BTN).interactable = false;
            GetButton((int)Buttons.Option1BTN).GetComponent<Image>().sprite = GetButton((int)Buttons.Option1BTN).spriteState.pressedSprite;
        }

        void ShowOption1()
        {
            if (dialogues[currentDialogueIndex].CostGold > 0)
            {
                Managers.Data._myPlayerData.nowGoldAmount -= dialogues[currentDialogueIndex].CostGold;
                for (int i = 0; i < dialogues[currentDialogueIndex+2].rewardStats.Count; i++)
                {
                    Managers.Data._myPlayerData.StatUpByDialogue(dialogues[currentDialogueIndex + 2].rewardStats[i]);
                }
            }

            Managers.instance.ShowDefualtPopUP(dialogues[currentDialogueIndex + 2].sentence);
        }

        void ShowOption2()
        {
            if (dialogues[currentDialogueIndex + 1].CostGold > 0)
            {
                Managers.Data._myPlayerData.nowGoldAmount -= dialogues[currentDialogueIndex + 1].CostGold;
                for (int i = 0; i < dialogues[currentDialogueIndex + 3].rewardStats.Count; i++)
                {
                    Managers.Data._myPlayerData.StatUpByDialogue(dialogues[currentDialogueIndex + 3].rewardStats[i]);
                }
            }

            Managers.instance.ShowDefualtPopUP(dialogues[currentDialogueIndex + 3].sentence);
        }

        GetButton((int)Buttons.Option1BTN).onClick.AddListener(ShowOption1);
        GetButton((int)Buttons.Option2BTN).onClick.AddListener(ShowOption2);
    }
}