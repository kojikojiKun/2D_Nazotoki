using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    Animator m_animator;
    PlayerController m_playerController;

    private Vector2 m_inputMove; //入力値.
    private float m_moveSpeedX; //x軸方向の移動速度.
    private bool m_isJumping;
    private bool m_isGrounded;
    private bool m_isGroundedPrev;

    // Start is called before the first frame update
    void Start()
    {
        m_animator=GetComponent<Animator>();
        m_playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        RunAnim();
        CrouchAnim();
        JumpAnim();
        LandAnim();
    }

    //走るアニメーション再生.
    void RunAnim()
    {
        m_inputMove = m_playerController.InputMove; //入力値を取得.

        if (m_inputMove.x > 0.5f)
        {
            //キャラクターに右を向かせる.
            transform.localRotation = Quaternion.Euler(0, 90, 0);
        }
        else if (m_inputMove.x < -0.5f)
        {
            //キャラクターに左を向かせる.
            transform.localRotation = Quaternion.Euler(0, -90, 0);
        }

        //左右どちらかの移動入力がされているとき.
        if (m_inputMove.x > 0.5f || m_inputMove.x < -0.5f)
        {
            //アニメーターにフラグを渡す.
            m_animator.SetBool("moving", true);
        }
        //移動入力がされていないとき.
        else if (m_inputMove.x < 0.5f || m_inputMove.x > -0.5f)
        {
            //アニメーターにフラグを渡す.
            m_animator.SetBool("moving", false);
        }
    }

    //しゃがみアニメーションを再生.
    void CrouchAnim()
    {
        m_animator.SetBool("isCrouch", m_playerController.IsCrouch);
    }

    //ジャンプアニメーション再生.
    void JumpAnim()
    {
        if (m_playerController.PressJump == false)
        {
            m_isJumping = false;
        }

        if (m_playerController.PressJump == true && m_isJumping == false)
        {
            m_animator.SetTrigger("jump");
            m_isJumping = true;
        }
    }

    //接触判定を受け取る.
    public void IsGroundCheck(bool isGrounded)
    {
        m_isGrounded = isGrounded;
    }

    //着地アニメーションを再生.
    void LandAnim()
    {
        m_animator.SetBool("isGrounded", m_isGrounded);

        if (m_isGroundedPrev == false)
        {
            if (m_isGrounded == true)
            {
                m_animator.SetTrigger("land");
            }
        }
        m_isGroundedPrev = m_isGrounded;
    }

    //ステージクリア時のアニメーションを再生.
    public void ClearAnim()
    {
        m_animator.SetTrigger("clearStage");
    }
}
