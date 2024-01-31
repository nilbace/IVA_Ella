using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BtnChildUpdown : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    bool Interactable = true;
    public float offset = 1.0f;  // �̵��� Offset ��

    private void Awake()
    {
        Interactable = true;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (Interactable)
        {

            // �ڽ� ������Ʈ���� ��ġ�� Offset��ŭ ������
            foreach (Transform child in transform)
            {
                child.position -= new Vector3(0, offset, 0);
            }
        }
    }

    public void SetUninteractable()
    {
        Interactable = false;
        foreach (Transform child in transform)
        {
            child.position -= new Vector3(0, offset, 0);
        }
    }

    public void NotEnoughMoney()
    {
        Interactable = false;
        foreach (Transform child in transform)
        {
            TMPro.TMP_Text textComponent = child.GetComponent<TMPro.TMP_Text>();
            if (textComponent != null)
            {
                Color textColor = textComponent.color;
                textColor.a = 0.5f;
                textComponent.color = textColor;
            }
        }
    }



public void OnPointerUp(PointerEventData eventData)
    {

        if(Interactable)
        {
            // �ڽ� ������Ʈ���� ��ġ�� Offset��ŭ �ø���
            foreach (Transform child in transform)
            {
                child.position += new Vector3(0, offset, 0);
            }
        }
    }
}