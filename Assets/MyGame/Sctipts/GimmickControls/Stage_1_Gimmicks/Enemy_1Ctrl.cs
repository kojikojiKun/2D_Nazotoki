using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy_1Ctrl : MonoBehaviour
{

    [SerializeField] GameObject m_tentacle;
    [SerializeField] CameraController m_camera;
    [SerializeField] Transform m_FixedCameraPos;
    PlayerController m_playerController;
    Animator m_animator;
    PlayerInput m_playerInput;
    Rigidbody2D m_playerRb;
    private bool m_isCatch = false;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (GameManager.s_instance.PlayerController != null)
        {
            m_playerController = GameManager.s_instance.PlayerController;
            m_playerInput = m_playerController.GetComponent<PlayerInput>();
            m_playerRb = m_playerController.GetComponent<Rigidbody2D>();
        }
    }

    private void Update()
    {
        if (m_playerController == null)
        {
            m_playerController = GameManager.s_instance.PlayerController;
            m_playerInput = m_playerController.GetComponent<PlayerInput>();
            m_playerRb = m_playerController.GetComponent<Rigidbody2D>();
        }

        if (m_isCatch)
        {
            m_playerController.gameObject.transform.position = m_tentacle.transform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AttackAnim();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_animator.ResetTrigger("attack");
        }
    }

    //プレイヤーを攻撃.
    void AttackAnim()
    {
        m_animator.SetTrigger("attack");
    }

    public void FreezeAnim()
    {
        m_animator.SetBool("isFreeze", true);
        m_animator.SetTrigger("freeze");
    }

    public void DeFrostAnim()
    {
        m_animator.SetBool("isFreeze", false);
    }

    //プレイヤーを触手のアニメーションに応じて移動させる.
    public void RemovePlayer()
    {
        m_isCatch = true;

        //カメラの位置を固定.
        m_camera.FixedPosition(m_FixedCameraPos);

        //入力、物理演算無効化.
        m_playerInput.enabled = false;
        m_playerRb.useFullKinematicContacts = true;
    }

    //プレイヤーの移動が完了したらリスポーンさせる.
    public void FinishRemovePlayer()
    {
        m_isCatch = false;
        m_playerRb.useFullKinematicContacts = false;

        //カメラの追跡再開.
        m_camera.StartChase();

        GameManager.s_instance.Respawn();
    }
}
