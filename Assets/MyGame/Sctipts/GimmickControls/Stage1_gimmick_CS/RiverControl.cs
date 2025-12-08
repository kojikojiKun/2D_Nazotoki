using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RiverControl : MonoBehaviour
{
    [SerializeField] GameObject m_disableObject; //プレイヤーを通れなくしているオブジェクト.
    [SerializeField] GameObject m_riverWater;
    [SerializeField] private float m_travelTime; //移動にかかる時間.
    [SerializeField] private float m_delayAppearTime; //出現までの時間.
    [SerializeField] private float m_delayDisappearTime; //見えなくなるまでの時間.

    WeatherManager m_weatherManager;
    BaseGimmickCtrl m_gimmickCtrl;
    private float m_goalWarerHeight = -34f; //目標のy座標.
    private Vector3 m_initWaterPos; //初期座標.
    private Vector3 m_goalWaterPos; //目標座標.
    public float DelayAppearTime => m_delayAppearTime;

    private bool m_isSolved = false; //ギミック解除フラグ.
    private bool m_isAppeard = false; //水出現フラグ.
    private bool m_isDisabled = false; //オブジェクト無効フラグ.

    private void Start()
    {
        m_weatherManager = WeatherManager.s_instance;
        m_gimmickCtrl = GetComponentInParent<BaseGimmickCtrl>();

        m_initWaterPos = m_riverWater.transform.position;
        m_goalWaterPos = new Vector3(m_riverWater.transform.position.x,
                m_goalWarerHeight,
                m_riverWater.transform.position.z);
    }

    private void Update()
    {
        //ギミックが解除されたとき.
        if (m_gimmickCtrl.IsGimmickActive() == true && m_isSolved == false)
        {
            AppearRiver();
            m_isSolved = true;
        }
        Debug.Log($"currentWeather={m_weatherManager.CurrentWeather},{m_isAppeard}");
        //晴れを選択すると川の水が消える.
        if (m_isAppeard == true && m_weatherManager.CurrentWeather == WeatherManager.WeatherType.sunny)
        {
            DisappearRiver();
        }

        //雨を選択すると川の水が現れる.
        if (m_isAppeard == false && m_weatherManager.CurrentWeather == WeatherManager.WeatherType.rainy)
        {
            AppearRiver();
        }
    }

    //川に水を出現させる.
    void AppearRiver()
    {
        if (m_isDisabled == true)
        {
            m_disableObject.SetActive(true);
        }

        //delayTime待ってからgoalWaterPosへtravelTimeで移動する.
        m_riverWater.transform.DOMove(m_goalWaterPos, m_travelTime).SetDelay(m_delayAppearTime)
                .OnComplete(() =>
                {
                    DOTween.Kill(m_riverWater.transform);
                    m_isAppeard = true;
                });
    }

    //水を消滅させる.
    void DisappearRiver()
    {
        //障害物を消す.
        DisableObstacleObject();

        m_riverWater.transform.DOMove(m_initWaterPos, m_travelTime).SetDelay(m_delayAppearTime)
                .OnComplete(() =>
                {
                    DOTween.Kill(m_riverWater.transform);
                    m_isAppeard = false;
                });
    }

    //障害物を無効にする.
    void DisableObstacleObject()
    {
        m_disableObject.SetActive(false);
        m_isDisabled = true;
    }
}
