using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColliderDetector : MonoBehaviour
{
    [SerializeField] PlayerController m_player;
    [SerializeField] PlayerAnimation m_playerAnim;

    [Header("壁との接触判定")]
    [SerializeField] LayerMask m_wallMask; //壁のレイヤー.
    [SerializeField, Range(0.01f, 0.2f)] private float m_skinWidth; //壁との余白.
    [SerializeField, Range(0.1f, 1f)] private float m_upRayLength; //上に向かって飛ばすrayの長さ.
    [SerializeField] CapsuleCollider2D m_playerCol; //プレイヤーのコライダー.
    [SerializeField] Rigidbody2D m_playerRb2D; //プレイヤーのrigidBody

    [Header("プレイヤーの接地判定")]
    [SerializeField] Vector2 m_boxRaySize; //boxRaycast2Dの大きさ.
    [SerializeField] float m_boxRayLength; //boxRaycastを飛ばす距離.
    [SerializeField] Vector3 m_offset;
    [SerializeField] LayerMask m_groundMask; //地面のレイヤー.

    [Header("地面の角度を判定")]
    [SerializeField] float m_maxSlopeAngle; //プレイヤーが滑り落ちる地面の角度.
    [SerializeField] Vector2 m_groundCheckSize;
    [SerializeField] float m_groundCheckDist;

    Vector2 m_move;
    private bool m_isGrounded; //接地フラグ.

    private void Update()
    {
        CeilingDetect();
        WallDetect();
        GroundDetect();
    }

    private void FixedUpdate()
    {
        m_move = m_player.InputMove.normalized * m_player.MoveSpeed * Time.fixedDeltaTime; //移動入力を取得.
    }

    //プレイヤーの接地判定.
    void GroundDetect()
    {
        Vector2 rayOrigin = transform.position + m_offset;

        RaycastHit2D hit = Physics2D.BoxCast(
            rayOrigin, //開始位置.
            m_boxRaySize, //大きさ.
            0f, //角度.
            Vector2.down, //方向.
            m_boxRayLength, //距離.
            m_groundMask
            );

        if (hit.collider != null)
        {
            //法線を取得.
            Vector2 normal = hit.normal;

            //地面の角度を計算.
            float slopeAngle = Vector2.Angle(normal, Vector2.up);

            if (slopeAngle > m_maxSlopeAngle)
            {
                //急斜面なら地面扱いしない.
                m_isGrounded = false;

                Vector2 slide = new Vector2(normal.y, -normal.x);
                if (slide.y > 0f)
                {
                    slide = -slide;
                }
                m_player.SlideDown(slide);
            }
            else
            {
                m_isGrounded = true;
            }
        }
        else
        {
            m_isGrounded = false;
        }

        //プレイヤーとプレイヤーのアニメーション管理スクリプトに接地判定を渡す.
        m_player.IsGroundCheck(m_isGrounded);
        m_playerAnim.IsGroundCheck(m_isGrounded);
    }

    //次のフレームの足元が歩ける場所かどうかをチェック.
    public bool CanStepSlope(Vector2 nextPos)
    {
        //足元にboxCastを飛ばす.
        RaycastHit2D hit = Physics2D.BoxCast(
            nextPos, //開始位置.
            m_groundCheckSize, //大きさ.
            0f, //角度.
            Vector2.down, //方向.
            m_groundCheckDist, //距離.
            m_groundMask
        );

        if (hit.collider == null)
        {
            return true;
        }

        Vector2 normal = hit.normal;

        //地面との角度を計算.
        float slopeAngle = Vector2.Angle(normal, Vector2.up);

        //下りは歩行可能.
        if (normal.y < 0f)
        {
            return true;
        }

        //上りの角度をが急なら歩行不可能.
        if (slopeAngle > m_maxSlopeAngle)
        {
            return false;
        }

        //角度が許容範囲内ならtrue
        return true;
    }

    private void OnDrawGizmos()
    {
        // ================================
        // ① GroundDetect の BoxCast 描画
        // ================================
        Gizmos.color = Color.red;

        Vector2 groundOrigin = (Vector2)transform.position + m_offset;

        // 中心 = origin + 下方向の半分
        Vector2 groundCenter = groundOrigin + Vector2.down * (m_boxRayLength * 0.5f);

        // サイズ = 元のサイズ + 下方向の長さ
        Vector3 groundSize = new Vector3(
            m_boxRaySize.x,
            m_boxRaySize.y + m_boxRayLength,
            1f
        );

        Gizmos.DrawWireCube(groundCenter, groundSize);


        // ================================
        // ② Slope チェックの BoxCast 描画
        // ================================
        Gizmos.color = Color.yellow;

        Vector2 slopeOrigin = (Vector2)transform.position;

        // 中心 = origin + 下方向の半分
        Vector2 slopeCenter = slopeOrigin + Vector2.down * (m_groundCheckDist * 0.5f);

        // サイズ = 元サイズ + 下方向距離
        Vector3 slopeSize = new Vector3(
            m_groundCheckSize.x,
            m_groundCheckSize.y + m_groundCheckDist,
            1f
        );

        Gizmos.DrawWireCube(slopeCenter, slopeSize);


        // ================================
        // ③ 参考：Ray の可視化
        // ================================
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(groundOrigin, groundOrigin + Vector2.down * m_boxRayLength);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(slopeOrigin, slopeOrigin + Vector2.down * m_groundCheckDist);
    }


    //頭上のオブジェクトとの接触を判定.
    void CeilingDetect()
    {
        Vector2 rayOrigin = transform.position;

        bool hitCeilig = false;

        //上に向けてrayを出す.
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, m_upRayLength, m_wallMask);

        //シーンビューで可視化.
        Debug.DrawRay(rayOrigin, Vector2.up * m_upRayLength, Color.red);

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
}
