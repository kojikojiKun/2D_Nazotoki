using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColliderDetector : MonoBehaviour
{
    [SerializeField] PlayerController m_player;
    [SerializeField] PlayerAnimation m_playerAnim;

    [Header("壁との接触判定")]
    [SerializeField] LayerMask m_wallMask; //壁のレイヤー.
    [SerializeField, Range(0.01f, 1f)] private float m_skinWidth; //壁との余白.
    [SerializeField, Range(0.1f, 1f)] private float m_upRayLength; //上に向かって飛ばすrayの長さ.
    [SerializeField] CapsuleCollider2D m_playerCol; //プレイヤーのコライダー.
    [SerializeField] Rigidbody2D m_playerRb2D; //プレイヤーのrigidBody

    [Header("プレイヤーの接地判定")]
    [SerializeField] Vector2 m_boxRaySize; //boxRaycast2Dの大きさ.
    [SerializeField] float m_boxRayLength; //boxRaycastを飛ばす距離.
    [SerializeField] Vector3 m_offset;
    [SerializeField] LayerMask m_groundMask; //地面のレイヤー.
    [SerializeField] private float m_maxProceedAngle; //プレイヤーが歩ける地面の角度.

    Vector2 m_move;
    private bool m_inSlope;
    private bool m_isGrounded; //接地フラグ.

    private void Start()
    {
        //Time.timeScale = 0.5f;
    }

    private void FixedUpdate()
    {
        m_move = m_player.InputMove.normalized * m_player.MoveSpeed * Time.fixedDeltaTime; //移動入力を取得.
        CeilingDetect();
        WallDetect();
        GroundDetect();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("checkPoint"))
        {
            //チェックポイントの座標を渡す.
            StageManager.s_instance.ReceiveCheckPoint(collision.transform);
        }

        if (collision.CompareTag("respawnPlayer"))
        {
            //プレイヤーをリスポーンさせる.
            StageManager.s_instance.Respown();
        }

        if (collision.CompareTag("goal"))
        {
            //クリアアニメーション再生.
            m_playerAnim.ClearAnim();

            //ステージクリア時の処理を実行する.
            StageManager.s_instance.ClearStage();
        }
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
            Vector2 normal = hit.normal;

            //法線を上向きに補正.
            if (normal.y < 0)
            {
                normal = -normal;
            }
            //角度計算+角度を常に0～90度に正規化.
            float angle = Vector2.Angle(normal, Vector2.up);
            if (angle > 90f)
            {
                angle = 180f - angle;
            }

            //角度が小さすぎる場合は平面と同じ扱い.
            if (angle < 5f)
            {
                m_inSlope = false;
                m_player.ReceiveSlopeDirection(Vector2.zero, 0, m_inSlope);
            }

            //急斜面→滑り落ちる.
            if (angle >= m_maxProceedAngle)
            {
                m_inSlope = true;

                //地面の角度を計算.
                Vector2 slopeDir = new Vector2(normal.y, -normal.x);

                //ベクトルが上方向に向いていたら下方向に補正する.
                if (Vector2.Dot(slopeDir, Vector2.down) < 0f)
                {
                    slopeDir = -slopeDir; //下方向に反転.
                }

                slopeDir.Normalize();

                //プレイヤーに地面の角度を渡す.
                m_player.ReceiveSlopeDirection(slopeDir, angle, m_inSlope);
            }
            else
            {
                m_inSlope = false;
                m_player.ReceiveSlopeDirection(Vector2.zero, angle, m_inSlope);
            }
            m_isGrounded = true;
        }
        else
        {
            m_inSlope = false;
            m_isGrounded = false;

            //接地していなければslopeDirリセット.
            m_player.ReceiveSlopeDirection(Vector2.zero, 0, m_inSlope);

        }

        //プレイヤーとプレイヤーのアニメーション管理スクリプトに接地判定を渡す.
        m_player.IsGroundCheck(m_isGrounded);
        m_playerAnim.IsGroundCheck(m_isGrounded);
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

    //rayと斜面の角度を計算.
    bool IsWall(RaycastHit2D hit)
    {
        if (hit.collider == null) return false;

        // 法線と上方向の角度を計算
        float angle = Vector2.Angle(hit.normal, Vector2.up);

        // 角度が大きい => 壁
        return angle > m_maxProceedAngle;
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

        offset = new Vector2(0, m_playerCol.size.y * 1f);

        bool hitWall = false;
        float allowedDist = moveDist;

        // Ray を2本出す
        RaycastHit2D hit1 = Physics2D.Raycast(rayOrigin + offset, direction, rayLength, m_wallMask);
        RaycastHit2D hit2 = Physics2D.Raycast(rayOrigin - offset, direction, rayLength, m_wallMask);

        // Sceneビューで可視化
        Debug.DrawRay(rayOrigin + offset, direction * 5, Color.red);
        Debug.DrawRay(rayOrigin - offset, direction * 5, Color.red);

        if (hit1.collider != null && IsWall(hit1))
        {
            hitWall = true;

            if (hitWall == true)
            {
                //移動可能距離を計算.
                allowedDist = Mathf.Min(allowedDist, hit1.distance - m_skinWidth);
            }
        }
        if (hit2.collider != null && IsWall(hit2))
        {
            hitWall = true;

            if (hitWall == true)
                //移動可能距離を計算.
                allowedDist = Mathf.Min(allowedDist, hit2.distance - m_skinWidth);
        }

        //プレイヤーに壁との接触情報を渡す.
        m_player.ComputeWallCheck(hitWall, allowedDist);
    }
}
