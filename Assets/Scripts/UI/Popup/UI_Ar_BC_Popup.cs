using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_Ar_BC_Popup : UI_Popup
{
    public static object tasktype;
    public static bool isCold;
    public static bool isRunAway;
    OneDayScheduleData oneDayScheduleData;
    public Animator ScreenAnimator;
    public Animator RubiaAnimator;
    public TMPro.TMP_Text Infotext;
    public Button BTN_Close;
    void Start()
    {
        Init();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            if (isCold || isRunAway)
            {
                ScreenAnimator.SetTrigger(isCold ? "Cold" : "RunAway");
            }
            else
            {
                ScreenAnimator.SetTrigger(oneDayScheduleData.PathName);
            }
        }
    }

    public override void Init()
    {
        base.Init();

        BTN_Close.onClick.AddListener(CloseBTN);
        oneDayScheduleData = Managers.Data.GetOneDayDataByObject(tasktype);
        ScreenAnimator.speed = RubiaAnimator.speed = ScreenAniSpeed;
        if(isCold || isRunAway)
        {
            ScreenAnimator.SetTrigger(isCold ? "Cold" : "RunAway");
            Infotext.text = isCold ?
                "����ģ ����� ����� ������ �ʷ��մϴ�. �������� ��ȣ�� �޵��� �սô�." :
                "������ � �巡���� ��� �޷��� ������ ���ƿñ��?";
        }
        else
        {
            ScreenAnimator.SetTrigger(oneDayScheduleData.PathName);
            Infotext.text = oneDayScheduleData.ArchiveInfoText;
        }
     
        if(tasktype is BroadCastType)
        {
            RubiaAnimator.SetTrigger(oneDayScheduleData.RubiaAni);
        }
        else
        {
            RubiaAnimator.gameObject.SetActive(false);
        }
        
    }

    private void OnDisable()
    {
        isCold = false;
        isRunAway = false;
    }
}
