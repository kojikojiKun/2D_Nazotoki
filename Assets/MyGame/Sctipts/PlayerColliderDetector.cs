using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColliderDetector : MonoBehaviour
{ 
    [SerializeField] private int groundLayerNum; //地面のレイヤー.
    private bool m_isGrounded; //接地フラグ.

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == groundLayerNum)
        {
            m_isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == groundLayerNum)
        {
            m_isGrounded = false;
        }
    }

    //地面との接触判定を返す
    public bool IsGrounded()
    {
        return m_isGrounded;
    }
}
