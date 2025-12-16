using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class StageManager : MonoBehaviour
{
    public static StageManager s_instance;
    [SerializeField] GameObject m_clearWindow;
 
    [SerializeField] private Transform m_startPos; //プレイヤーが最初にスポーンする場所.
    private GameObject m_player;
    Transform m_checkPoint;
    PlayerInput m_input;

    private void Awake()
    {
        //インスタンス化.
        if (s_instance != null && s_instance != this)
        {
            Destroy(s_instance);
            return;
        }
        s_instance = this;
    }

    private void Start()
    {
        //プレイヤーをスタート地点まで移動.
        m_player = GameObject.FindGameObjectWithTag("Player");
        if (m_player != null)
        {
            Respown();
            m_input = m_player.GetComponent<PlayerInput>();
        }
    }

    //プレイヤーが最後に通過したチェックポイントの座標を受け取る.
    public void ReceiveCheckPoint(Transform lastCheckPoint)
    {
        m_checkPoint = lastCheckPoint;
    }

    //プレイヤーをチェックポイントまで移動させる.
    public void Respown()
    {
        
        if (m_checkPoint != null)
        {
            m_player.transform.position = m_checkPoint.position;
        }
        else
        {
            m_player.transform.position = m_startPos.position;
        }
    }

    //プレイヤーがゴールに到達したときの処理.
    public void ClearStage()
    {
        //キャラクターの操作をを無効化.
        m_input.enabled = false;

        //delayTime経過でクリアウィンドウ表示.
        float delayTime = 1.5f;
        this.transform.DOLocalMove(new Vector3(1f, 0, 0), delayTime).OnComplete(() =>
        {
            //クリアウィンドウ表示.
            m_clearWindow.SetActive(true);
        }
            );
    }
}
