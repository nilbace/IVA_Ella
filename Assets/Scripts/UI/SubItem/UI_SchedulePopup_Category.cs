using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SchedulePopup_Category : MonoSingleton<UI_SchedulePopup_Category>
{
    public Button targetButton;  // üũ�� ��� Button

    private bool isButtonDown = false;  // ��ư�� ���ȴ��� ����

    private void Update()
    {
        // ���콺 ���� ��ư�� ������ ��
        if (Input.GetMouseButtonDown(0))
        {
            // targetButton�� null�� �ƴϰ� ���콺 Ŭ���� ��ġ�� targetButton�� RectTransform �ȿ� �ִ��� Ȯ��
            if (targetButton != null && RectTransformUtility.RectangleContainsScreenPoint(targetButton.GetComponent<RectTransform>(), Input.mousePosition))
            {
                isButtonDown = true;
                Debug.Log("��ư ����");
            }
        }

        // ���콺 ���� ��ư�� ���� ��
        if (Input.GetMouseButtonUp(0))
        {
            // targetButton�� null�� �ƴϰ� ���콺 Ŭ���� ��ġ�� targetButton�� RectTransform �ȿ� �ִ��� Ȯ���ϰ� ��ư�� ���� �������� Ȯ��
            if (targetButton != null && RectTransformUtility.RectangleContainsScreenPoint(targetButton.GetComponent<RectTransform>(), Input.mousePosition) && isButtonDown)
            {
                isButtonDown = false;
                Debug.Log("��ư ��");
            }
        }
    }

    private void Start()
    {
        targetButton.onClick.AddListener(() => Debug.Log("��ư ����"));
    }
}
