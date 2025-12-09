using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager s_instance;

    [SerializeField] private Transform m_startPos; //プレイヤーが最初にスポーンする場所.
    private GameObject m_player;
    Transform m_checkPoint;

    private void Awake()
    {
        if (s_instance != null && s_instance != this)
        {
            Destroy(s_instance);
            return;
        }
        s_instance = this;
    }

    private void Start()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
        if (m_player != null)
        {
            Respown();
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
            Debug.Log($"respawn{m_startPos.position},player{m_player.transform.position}");
        }
    }

    //プレイヤーがゴールに到達したときの処理.
    public void ClearStage()
    {
        SceneController.s_instance.LoadSelectStage();
    }
}
