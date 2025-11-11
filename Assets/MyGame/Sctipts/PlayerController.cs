using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("プレイヤーのステータス")]
    [SerializeField] private float m_defSpeed; //通常の移動スピード.
    [SerializeField] private float m_jumpForce; //ジャンプ力.
    [SerializeField] PlayerColliderDetector m_footCollider; //接地判定.
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

    public Vector2 InputMove => m_inputMove;
    public bool PressJump => m_pressJump;
    public bool PressCrouch => m_pressCrouch;
    public bool IsCrouch => m_isCrouch;

    // Start is called before the first frame update
    void Start()
    {
        m_playerInput = GetComponent<PlayerInput>();
        m_rb2D = GetComponent<Rigidbody2D>();
        m_moveSpeed = m_defSpeed;
        m_defCharColOffset = new Vector2(m_characterCollider.offset.x, m_characterCollider.offset.y);
        m_defCharColSize = new Vector2(m_characterCollider.size.x, m_characterCollider.size.y);
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

    //プレイヤーを移動させる.
    void MovingPlayer()
    {
        if (m_inputMove.x > 0.3f)
        {
            //画面右側に移動.
            transform.position += new Vector3(m_moveSpeed, 0, 0) * Time.deltaTime;
        }
        else if (m_inputMove.x < -0.3)
        {
            //画面左側に移動.
            transform.position += new Vector3(-m_moveSpeed, 0, 0) * Time.deltaTime;
        }
    }

    //ジャンプ入力受け取り.
    public void OnJump(InputAction.CallbackContext context)
    {
        //ボタンが押されたとき.
        if (context.phase == InputActionPhase.Started && m_footCollider.IsGrounded() == true)
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
        else if (context.phase == InputActionPhase.Started && m_isCrouch == true)
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
