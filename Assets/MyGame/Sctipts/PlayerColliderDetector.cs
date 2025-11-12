using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColliderDetector : MonoBehaviour
{
    private enum ColliderParts
    {
        foot,
        character
    }

    [SerializeField] ColliderParts m_colldierPart;

    [Header("footColliderのパラメータ")]
    [SerializeField] private int m_groundLayerNum; //地面のレイヤー.

    [Header("charactorColliderのパラメータ")]
    [SerializeField] PlayerController m_player;
    [SerializeField] LayerMask m_wallMask; //壁のレイヤー.
    [SerializeField] private float m_skinWidth; //壁との余白.
    [SerializeField] CapsuleCollider2D m_playerCol; //プレイヤーのコライダー.
    [SerializeField] Rigidbody2D m_playerRb2D; //プレイヤーのrigidBody

    Vector2 m_input;
    private bool m_isGrounded; //接地フラグ.
    private bool m_isMoveVertically; //プレイヤーの上下の移動検知フラグ.

    private void Update()
    {


        //コライダーの部位がキャラクターのとき.
        if (m_colldierPart == ColliderParts.character)
        {
            m_input = m_player.InputMove.normalized; //移動入力を取得.
            m_isMoveVertically = Mathf.Abs(m_playerRb2D.velocity.y) > 0.01f; //上下の移動を検知.
            Move();
        }
    }

    //プレイヤーの壁すり抜け防止.
    void Move()
    {
        if (m_input.sqrMagnitude <= 0f) return;

        Vector2 direction = m_input;
        float moveDist = m_input.magnitude;
        Vector2 rayOrigin = transform.position; //rayの発射箇所.
        float rayLength = moveDist + m_skinWidth; //rayの長さ.

        Vector2 offset = Vector2.zero;
        if (Mathf.Abs(direction.x) > 0.01f)
        {
            offset = new Vector2(0, m_playerCol.size.y * 0.4f);
        }

        bool hitWall = false;
        float allowDist = moveDist;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == m_groundLayerNum)
        {
            m_isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == m_groundLayerNum)
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
