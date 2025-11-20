using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("プレイヤーの初期位置"), SerializeField] Transform m_startPos;
    [Header("プレイヤーのステータス")]
    [SerializeField] private float m_defSpeed; //通常の移動スピード.
    [SerializeField] private float m_jumpForce; //ジャンプ力.
    [SerializeField] PlayerColliderDetector m_colDetect; //接地判定,壁との接触判定,天井との接触判定.
    [SerializeField] CapsuleCollider2D m_characterCollider; //プレイヤーの当たり判定.

    private PlayerInput m_playerInput;
    private Vector2 m_defCharColOffset; //プレイヤーコライダーのoffsetの初期値.
    private Vector2 m_defCharColSize; //プレイヤーコライダーのサイズの初期値.
    private Rigidbody2D m_rb2D;
    private Vector2 m_inputMove; //入力値.

    private float m_moveSpeed; //反映する移動スピード.
    private bool m_pressJump; //ジャンプボタン入力フラグ.
    private bool m_pressCrouch; //しゃがみボタン検知フラグ.
    private bool m_isCrouch; //しゃがみフラグ.
    private bool m_canStanding; //しゃがみ解除可能フラグ.
    private bool m_isGrounded; //接地フラグ.

    private bool m_hitWall; //壁との接触検知フラグ.
    private float m_allowedDist; //壁までの距離制限.

    public Vector2 InputMove => m_inputMove;
    public float MoveSpeed => m_moveSpeed;
    public bool PressJump => m_pressJump;
    public bool PressCrouch => m_pressCrouch;
    public bool IsCrouch => m_isCrouch;

    // Start is called before the first frame update
    void Start()
    {
        //必要な要素を参照.
        m_playerInput = GetComponent<PlayerInput>();
        m_rb2D = GetComponent<Rigidbody2D>();
        m_defCharColOffset = new Vector2(m_characterCollider.offset.x, m_characterCollider.offset.y);
        m_defCharColSize = new Vector2(m_characterCollider.size.x, m_characterCollider.size.y);

        //移動速度の初期値を保存.
        m_moveSpeed = m_defSpeed;

        //初期位置に移動させる.
        if (m_startPos != null)
            transform.position = m_startPos.position;
    }

    // Update is called once per frame
    void Update()
    {
        MovingPlayer();
    }

    //移動入力受け取り.
    public void OnMove(InputAction.CallbackContext context)
    {
        m_inputMove = context.ReadValue<Vector2>();
    }

    //壁との接触情報を受け取る.
    public void ComputeWallCheck(bool hitWall,float allowedDist)
    {
        m_hitWall = hitWall;
        m_allowedDist = allowedDist;
    }

    //プレイヤーを移動させる.
    void MovingPlayer()
    {
        if (m_hitWall == false || m_allowedDist > 0)
        {
           /* Vector2 direcition = Vector2.zero;
            if (m_inputMove.x > 0.3f)
            {
                //移動方向を画面右側にする.
                direcition = Vector2.right;
            }
            else if (m_inputMove.x < -0.3)
            {
                //移動方向を画面左側にする.
                direcition = Vector2.left;
            }
            */

            //入力値から速度ベクトルを作る.
            Vector2 velocity = m_inputMove * m_moveSpeed;

            //次のフレームの地面が歩けるかどうかを求める.
            Vector2 nextPos = (Vector2)transform.position + velocity * Time.deltaTime;
            if (m_colDetect.CanStepSlope(nextPos) == false)
            {
                //地面の斜面が急なら移動を止める.
                velocity.x = 0f;
            }

            //実際の移動距離.
            float moveDist = velocity.magnitude * Time.deltaTime;

            //壁までの距離制限を反映.
            float allowedMove = Mathf.Min(moveDist, m_allowedDist);

            Vector2 direction = velocity.normalized;

            //実際に移動させる.
            transform.position += (Vector3)(direction * allowedMove);
        }
    }

    //急斜面の上ならプレイヤーを滑らせる.
    public void SlideDown(Vector2 slide)
    {
        float slidePower = 20f;

        // 空中に飛び上がらないように、Y成分だけ落とす手もある
        slide = slide.normalized;

        if (slide.y > 0f)
        {
            slide.y = 0f;
        }

        // 加速しすぎ防止
        if (m_rb2D.velocity.magnitude < 6f)
        {
            m_rb2D.AddForce(slide * slidePower, ForceMode2D.Force);
        }
    }

    public void IsGroundCheck(bool isGrounded)
    {
        m_isGrounded = isGrounded;
    }

    //ジャンプ入力受け取り.
    public void OnJump(InputAction.CallbackContext context)
    {
        //ボタンが押されたとき.
        if (context.phase == InputActionPhase.Started && m_isGrounded==true && m_canStanding==true)
        {
            JumpingPlayer();

            //ボタンが押されたことを検知.
            m_pressJump = true;
        }
        //ボタンが離されたとき.
        else if (context.phase == InputActionPhase.Canceled)
        {
            m_pressJump = false;
        }
    }

    //プレイヤーをジャンプさせる.
    void JumpingPlayer()
    {
        //上方向にm_jumpForceの力を加える.
        m_rb2D.AddForce(Vector2.up * m_jumpForce, ForceMode2D.Impulse);

        //しゃがみ状態解除.
        m_isCrouch = false;

        //しゃがみ解除で移動速度を元に戻す.
        m_moveSpeed = m_defSpeed;

        //プレイヤーコライダーのパラメータを初期値に戻す.
        m_characterCollider.offset = new Vector2(m_defCharColOffset.x, m_defCharColOffset.y);
        m_characterCollider.size = new Vector2(m_defCharColSize.x, m_defCharColSize.y);
    }

    //上方向のオブジェクトと接触しているかどうかを受け取る.
    public void CeilingCheck(bool hitCeiling)
    {
        //オブジェクトと接触していればしゃがみ解除できないようにする.
        m_canStanding = !hitCeiling;
    }

    //しゃがみ入力受け取り.
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && m_isCrouch == false)
        {
            //ボタン入力を検知.
            m_pressCrouch = true;

            //しゃがみ状態に移行.
            m_isCrouch = true;

            //しゃがみ中は移動速度低下.
            m_moveSpeed = m_defSpeed * 0.5f;

            //キャラクターの姿勢にコライダーを合わせる
            m_characterCollider.offset = new Vector2(0f, -0.22f);
            m_characterCollider.size = new Vector2(0.5f, 0.51f);
        }
        else if (context.phase == InputActionPhase.Started && m_isCrouch == true && m_canStanding==true)
        {
            //ボタン入力を検知.
            m_pressCrouch = true;

            //しゃがみ状態解除.
            m_isCrouch = false;

            //しゃがみ解除で移動速度を元に戻す.
            m_moveSpeed = m_defSpeed;

            //プレイヤーコライダーのパラメータを初期値に戻す.
            m_characterCollider.offset = new Vector2(m_defCharColOffset.x, m_defCharColOffset.y);
            m_characterCollider.size = new Vector2(m_defCharColSize.x, m_defCharColSize.y);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            m_pressCrouch = false;
        }
    }
}
