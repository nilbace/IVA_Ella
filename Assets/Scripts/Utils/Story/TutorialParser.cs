using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class TutorialParser : MonoSingleton<TutorialParser>
{
    public TextAsset TutorialAsset;

    public List<Dialogue> Dialogues = new List<Dialogue>();

   

    private void Start()
    {
        string[] lines = TutorialAsset.text.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            Dialogues.Add(DebugSetence(lines[i]));
        }

        UI_Tutorial.instance.StartDiagloue(Dialogues);
    }


    Dialogue DebugSetence(string dialogue)
    {
        Dialogue temp = new Dialogue();
        string[] lines = dialogue.Split('\t');

        //�̸�
        temp.name = lines[0];

        //���� ������
        if (lines[1] == "����")
            temp.isLeft = true;
        else
            temp.isLeft = false;

        //��� ���� ��ġ
        if (lines[2] == "") temp.tutorialFocus = TutorialFocusPoint.MaxCount;
        else Enum.TryParse(lines[2], out temp.tutorialFocus);

        if (lines[3] == "TRUE") temp.IsInteractable = true;

        //��ȭ �ϳ�
        temp.sentence = ReplaceString(lines[4]);
        
        

        return temp;
    }

    public string ReplaceString(string inputString)
    {
        string replacedString = inputString.Replace("��", "\n");
        return replacedString;
    }

}
