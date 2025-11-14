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
    [SerializeField, Range(0.01f, 0.2f)] private float m_skinWidth; //壁との余白.
    [SerializeField,Range(0.1f,1f)] private float m_upRayLength; //上に向かって飛ばすrayの長さ.
    [SerializeField] CapsuleCollider2D m_playerCol; //プレイヤーのコライダー.
    [SerializeField] Rigidbody2D m_playerRb2D; //プレイヤーのrigidBody

    Vector2 m_move;
    private bool m_isGrounded; //接地フラグ.

    private void Update()
    {
        //コライダーの部位がキャラクターのとき.
        if (m_colldierPart == ColliderParts.character)
        {
            CeilingDetect();
            WallDetect();
        }
    }

    private void FixedUpdate()
    {//コライダーの部位がキャラクターのとき.
        if (m_colldierPart == ColliderParts.character)
        {
            m_move = m_player.InputMove.normalized * m_player.MoveSpeed * Time.fixedDeltaTime; //移動入力を取得.
        }
    }

    //頭上のオブジェクトとの接触を判定.
    void CeilingDetect()
    {
        Vector2 rayOrigin = transform.position;

        bool hitCeilig = false;

        //上に向けてrayを出す.
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, m_upRayLength, m_wallMask);

        //シーンビューで可視化.
        Debug.DrawRay(rayOrigin, Vector2.up*m_upRayLength, Color.red);

        if (hit.collider != null)
        {
            hitCeilig = true;
        }

        //プレイヤーに頭上のオブジェクトとの接触情報を渡す.
        m_player.CeilingCheck(hitCeilig);
    }

    //プレイヤーの壁すり抜け防止.
    void WallDetect()
    {
        //if (m_move.sqrMagnitude <= 0f) return;

        Vector2 direction = m_move.normalized;
        float moveDist = m_move.magnitude;
        Vector2 rayOrigin = transform.position; //rayの発射箇所.
        float rayLength = moveDist + m_skinWidth; //rayの長さ.

        //カプセルコライダーの高さ、幅を元にrayのオフセットを決める.
        Vector2 offset = Vector2.zero;

        //横方向に動いているとき上下にrayを出す.
        if (Mathf.Abs(direction.x) > 0.01f)
        {
            offset = new Vector2(0, m_playerCol.size.y * 1f);
        }

        //縦方向に動いているとき左右にrayを出す.
        if (Mathf.Abs(m_playerRb2D.velocity.y) > 0.01f)
        {
            offset = new Vector2(m_playerCol.size.x * 0.4f, 0);
        }

        bool hitWall = false;
        float allowedDist = moveDist;

        // Ray を2本出す
        RaycastHit2D hit1 = Physics2D.Raycast(rayOrigin + offset, direction, rayLength, m_wallMask);
        RaycastHit2D hit2 = Physics2D.Raycast(rayOrigin - offset, direction, rayLength, m_wallMask);

        // Sceneビューで可視化
        Debug.DrawRay(rayOrigin + offset, direction * 5, Color.red);
        Debug.DrawRay(rayOrigin - offset, direction * 5, Color.red);

        if (hit1.collider != null)
        {
            allowedDist = Mathf.Min(allowedDist, hit1.distance - m_skinWidth);
            hitWall = true;
        }
        if (hit2.collider != null)
        {
            allowedDist = Mathf.Min(allowedDist, hit2.distance - m_skinWidth);
            hitWall = true;
        }

        //プレイヤーに壁との接触情報を渡す.
        m_player.ComputeWallCheck(hitWall, allowedDist);
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
