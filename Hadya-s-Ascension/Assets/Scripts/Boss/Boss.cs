using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Transform Player; // Ссылка на игрока
    public bool isFlipped = false; // Флаг для отслеживания состояния отражения

    void Start()
    {
        // Автоматически находим игрока при старте, если он не назначен в инспекторе
        if (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    public void LookAtPlayer()
    {
        if (Player == null) return; // Проверяем, что игрок назначен

        // Получаем текущий масштаб объекта
        Vector3 scale = transform.localScale;

        // Если игрок находится слева от босса и босс не отражён
        if (Player.position.x < transform.position.x && isFlipped)
        {
            scale.x = Mathf.Abs(scale.x); // Отражаем спрайт по горизонтали
            isFlipped = false;
        }
        // Если игрок находится справа от босса и босс отражён
        else if (Player.position.x > transform.position.x && !isFlipped)
        {
            scale.x = -Mathf.Abs(scale.x); // Возвращаем спрайт в исходное состояние
            isFlipped = true;
        }

        // Применяем новый масштаб
        transform.localScale = scale;
    }
}