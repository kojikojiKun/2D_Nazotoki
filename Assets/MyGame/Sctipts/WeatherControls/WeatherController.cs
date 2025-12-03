using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WeatherController : MonoBehaviour
{
    WeatherManager m_weatherManager;
    [Header("UIホイール")]
    [SerializeField] Canvas m_canvas;
    [SerializeField] GameObject m_weatherMenu;
    [SerializeField] RectTransform m_wheelCenter; //入力値との角度を計算するときの中心.
    [Tooltip("weathermenuの背景を設定"),SerializeField] Image[] m_image;
    [SerializeField] GameObject[] m_weatherWheel;
    
    private PlayerInput m_playerInput;

    private Vector2 m_inputMove; //入力値.
    private Vector3 m_defWheelScale; //UIホイールの初期のscale.
    private Vector3 m_selectedWheelScale; //選択されているUIホイールのscale.

    private int m_slotCnt = 4; //ホイールのアイテム数.
    private int m_currentSlot = -1; //現在選択中のスロットindex.
    private bool m_openWeatherMenu; //UIホイール表示フラグ.
   // private bool m_wasRightStickMoving; //gamepadの右スティック入力フラグ.

    private void Start()
    {
        m_weatherManager = WeatherManager.s_instance;
        m_playerInput = GetComponent<PlayerInput>();
        m_defWheelScale = m_weatherWheel[0].transform.localScale;
        m_selectedWheelScale = m_defWheelScale * 1.2f;

        m_openWeatherMenu = false;
    }

    //天候を変えるメニューの表示切替ボタンの入力受け取り.
    public void OnOpenWeatherMenu(InputAction.CallbackContext context)
    {
        //ボタンが押されたとき.
        if (context.phase == InputActionPhase.Started)
        {
            //表示済みのとき.
            if (m_openWeatherMenu == true)
            {
                m_weatherMenu.SetActive(false); //天候メニューを非表示にする.
                m_openWeatherMenu = false;
            }
            //表示されていないとき.
            else
            {
                m_weatherMenu.SetActive(true); //天候メニューを表示する.
                m_openWeatherMenu = true;
            }
        }
    }

    //UIホイールのアイテムを選択するときの入力受け取り.
    public void OnSelectWeather(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        Vector2 mousePos = Mouse.current.position.ReadValue();

        SelectWeather(input);
    }

    //UIホイールのアイテムを選択する.
    void SelectWeather(Vector2 input)
    {
        //天候メニューが開かれているとき.
        if (m_openWeatherMenu == true)
        {
            Vector2 dir;
            if (IsUsingMouse())
            {
                //マウスの場合：中心との差を取る
                RectTransformUtility.ScreenPointToLocalPointInRectangle
                    (m_wheelCenter,
                    input,
                    m_canvas.worldCamera,
                    out Vector2 localPoint
                    );

                dir = localPoint;
            }
            else
            {
                //スティックの場合：そのまま方向ベクトル
                dir = input;
            }

            if (dir.magnitude < 0.5f)
            {
                return;
            }

            //入力値とホイールの中心との角度を計算.
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (angle < 0)
            {
                angle += 360f; //0～360に正規化.
            }
            angle = (angle + 45f) % 360f;

            //角度からホイールのアイテムを選択.
            int index = Mathf.FloorToInt(angle / (360f / m_slotCnt));
            if (index != m_currentSlot)
            {
                m_currentSlot = index;
            }

            //選択されているアイテムの色を変更,scaleを大きくする.
            for (int i = 0; i < m_image.Length; i++)
            {
                if (i == index)
                {
                    m_image[i].color = new Color(0.4f, 0.7f, 1f);
                    m_weatherWheel[i].transform.localScale = m_selectedWheelScale;
                }
                else
                {
                    m_image[i].color = Color.gray;
                    m_weatherWheel[i].transform.localScale = m_defWheelScale;
                }
            }
        }
    }

    //天候を決定する入力受け取り.
    public void OnDecideWeather(InputAction.CallbackContext context)
    {
        //天候メニューが開かれているとき.
        if (m_openWeatherMenu == true)
        {
            //キーボード、マウス入力のとき.
            if (IsUsingMouse() == true)
            {
                if (context.phase == InputActionPhase.Started)
                {
                    DecideWeather();
                }
            }
            //キーボード、マウス入力でない(Gamepad入力)のとき.
            else
            {
                if (context.performed)
                {
                   // m_wasRightStickMoving = true;
                }

                //右スティックを離した瞬間.
                if (context.canceled)
                {
                   // m_wasRightStickMoving = false;
                    DecideWeather();
                }
            }
        }
    }

    //変更する天候を決定する.
    void DecideWeather()
    {
        m_weatherMenu.SetActive(false); //天候メニューを閉じる.
        m_openWeatherMenu = false;

        //選択中のスロットのindexを渡す.
        m_weatherManager.SetWeather(m_currentSlot);
    }


    //入力デバイスがマウス,キーボードならtrueを返す.
    private bool IsUsingMouse()
    {
        return m_playerInput != null &&
               m_playerInput.currentControlScheme != null &&
               m_playerInput.currentControlScheme.Contains("Keyboard&Mouse");
    }
}
