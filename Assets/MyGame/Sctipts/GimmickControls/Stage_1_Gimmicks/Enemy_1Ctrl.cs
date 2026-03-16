using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy_1Ctrl : MonoBehaviour
{    
    [SerializeField] PlayerController m_playerController;
    [SerializeField] GameObject m_tentacle;
    [SerializeField] CameraController m_camera;
    [SerializeField] Transform m_FixedCameraPos;
    Animator m_animator;
    PlayerInput m_playerInput;
    Rigidbody2D m_playerRb;
    private bool m_isCatch=false;
    public bool CanMove = true;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_playerInput = m_playerController.GetComponent<PlayerInput>();
        m_playerRb = m_playerController.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!CanMove)
            return;

        if(m_isCatch)
        {
            m_playerController.gameObject.transform.position = m_tentacle.transform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (m_isCatch)
            {
                Attack();
            }
        }
    }

    //プレイヤーを攻撃.
    void Attack()
    {
        m_animator.SetTrigger("attack");
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
        m_playerRb.useFullKinematicContacts=false;

        //カメラの追跡再開.
        m_camera.StartChase();

        GameManager.s_instance.Respawn();
    }
}
