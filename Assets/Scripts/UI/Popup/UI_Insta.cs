using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Insta : UI_Popup
{
    enum Buttons
    {
        CloseBTN
    }


    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.CloseBTN).onClick.AddListener(CloseBTN);
    }
}
