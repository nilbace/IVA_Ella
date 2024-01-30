using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BtnChildUpdown : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float offset = 1.0f;  // �̵��� Offset ��

    public void OnPointerDown(PointerEventData eventData)
    {

        // �ڽ� ������Ʈ���� ��ġ�� Offset��ŭ ������
        foreach (Transform child in transform)
        {
            child.position -= new Vector3(0, offset, 0);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        // �ڽ� ������Ʈ���� ��ġ�� Offset��ŭ �ø���
        foreach (Transform child in transform)
        {
            child.position += new Vector3(0, offset, 0);
        }
    }
}