using UnityEngine;

public class Enemy_2Ctrl : MonoBehaviour
{
    private Animator m_animator;
    private PlayerController m_playerController;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (GameManager.s_instance.PlayerController != null)
        {
            m_playerController = GameManager.s_instance.PlayerController;
        }
    }

    private void Update()
    {
        if (m_playerController == null)
        {
            m_playerController = GameManager.s_instance.PlayerController;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AttackAnim();
        }
    }

    void AttackAnim()
    {
        m_animator.SetTrigger("attack");
    }

    public void AttackPlayer()
    {
        m_playerController.KnockBackPlayer();
    }
}
