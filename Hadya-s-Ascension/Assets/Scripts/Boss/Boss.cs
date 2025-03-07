using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Transform Player; // ������ �� ������
    public bool isFlipped = false; // ���� ��� ������������ ��������� ���������

    void Start()
    {
        // ������������� ������� ������ ��� ������, ���� �� �� �������� � ����������
        if (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    public void LookAtPlayer()
    {
        if (Player == null) return; // ���������, ��� ����� ��������

        // �������� ������� ������� �������
        Vector3 scale = transform.localScale;

        // ���� ����� ��������� ����� �� ����� � ���� �� ������
        if (Player.position.x < transform.position.x && isFlipped)
        {
            scale.x = Mathf.Abs(scale.x); // �������� ������ �� �����������
            isFlipped = false;
        }
        // ���� ����� ��������� ������ �� ����� � ���� ������
        else if (Player.position.x > transform.position.x && !isFlipped)
        {
            scale.x = -Mathf.Abs(scale.x); // ���������� ������ � �������� ���������
            isFlipped = true;
        }

        // ��������� ����� �������
        transform.localScale = scale;
    }
}