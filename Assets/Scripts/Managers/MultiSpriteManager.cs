using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSpriteManager
{
    public Sprite[] MiniStatIcons;
    public Sprite[] Days;
    public Sprite[] DaysPannel;
    public Sprite[] StatusBar;

    public void Init()
    {
        MiniStatIcons = Resources.LoadAll<Sprite>("MultiSprite/MiniStatIcons");
        Days = Resources.LoadAll<Sprite>("MultiSprite/Days");
        DaysPannel = Resources.LoadAll<Sprite>("MultiSprite/DaysPannel");
        StatusBar = Resources.LoadAll<Sprite>("MultiSprite/StatusBar");
    }
}
