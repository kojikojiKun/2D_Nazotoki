using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Stage_1Manager : MonoBehaviour
{
    [SerializeField] BaseGimmickCtrl m_gimmickCtrl;
    [SerializeField] GameObject m_disableObject; //プレイヤーを通れなくしているオブジェクト.
    [SerializeField] GameObject m_riverWater;
    [SerializeField] private float m_travelTime; //移動にかかる時間.
    [SerializeField] private float m_delayAppearTime; //出現までの時間.
    [SerializeField] private float m_delayDisappearTime; //見えなくなるまでの時間.

    private float m_goalWarerHeight = -34f; //目標のy座標.
    private Vector3 m_initWaterPos; //初期座標.
    private Vector3 m_goalWaterPos; //目標座標.
    public float DelayAppearTime => m_delayAppearTime;

    private bool m_isMoved = false;

    private void Start()
    {
        m_initWaterPos = m_riverWater.transform.position;
        m_goalWaterPos = new Vector3(m_riverWater.transform.position.x,
                m_goalWarerHeight,
                m_riverWater.transform.position.z);
    }

    private void Update()
    {
        if (m_gimmickCtrl.IsGimmickActive() == true && m_isMoved==false)
        {
            AppearRiver();
        }
    }

    //障害物を無効にする.
    void DisableObstacleObject()
    {
        
    }

    //川に水を出現させる.
    void AppearRiver()
    {
        //delayTime待ってからgoalWaterPosへtravelTimeで移動する.
        m_riverWater.transform.DOMove(m_goalWaterPos, m_travelTime).SetDelay(m_delayAppearTime);
        m_isMoved = true;
    }

    //水を消滅させる.
    void DisappearRiver()
    {
        m_riverWater.transform.DOMove(m_initWaterPos, m_travelTime).SetDelay(m_delayAppearTime);
    }
}
