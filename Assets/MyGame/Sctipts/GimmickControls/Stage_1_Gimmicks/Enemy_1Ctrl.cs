using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1Ctrl : MonoBehaviour
{
    Animator m_animator;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Attack();
        }
    }

    void Attack()
    {
        Debug.Log("attack");
        m_animator.SetTrigger("withinRange");
    }
}
