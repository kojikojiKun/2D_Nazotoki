using System.Collections;
using UnityEngine;

public class Enemy_2Ctrl : MonoBehaviour
{
    [SerializeField] ParticleSystem m_particle;
    [SerializeField] Transform m_moveParticlePos;
    private Animator m_animator;
    private PlayerController m_playerController;
    private bool m_isDead = false;

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
        if (m_isDead == false)
            m_animator.SetTrigger("attack");
    }

    public void Die()
    {
        StartCoroutine(OnDie());
    }

    private IEnumerator OnDie()
    {
        m_isDead = true;
        m_animator.SetTrigger("die");
        m_particle.gameObject.transform.position = m_moveParticlePos.position;

        yield return new WaitForSeconds(1f);

        this.gameObject.SetActive(false);
    }

    public void AttackPlayer()
    {
        m_playerController.KnockBackPlayer();
    }
}
