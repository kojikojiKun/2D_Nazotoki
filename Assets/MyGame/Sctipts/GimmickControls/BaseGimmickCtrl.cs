using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct WeatherPriority
{    
   [Header("ギミックが反応する天候(リスト順)")] public WeatherManager.WeatherType w_weatherType;
}

public class BaseGimmickCtrl : MonoBehaviour
{
    WeatherManager m_weatherManager;
    [SerializeField] private List<WeatherPriority> m_weatherPriorities;

    private int m_progress = 0; //ギミックの進行度.
    private bool m_isWithinCamera = false; //カメラの範囲内.
    private bool m_previousCameraState;

    private void Start()
    {
        m_weatherManager = WeatherManager.s_instance;
    }

    private void Update()
    {
        CheckCameraVisibility();
    }

    //カメラに移っているかを判定.
    private void CheckCameraVisibility()
    {
        bool nowVisible = IsOnScreen();

        if (nowVisible == true && m_previousCameraState == false)
        {
            OnEnterCamera();
        }
        else if (nowVisible == false && m_previousCameraState == true)
        {
            OnExitCamera();
        }

        m_previousCameraState = nowVisible;
    }

    private bool IsOnScreen()
    {
        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);
        return screenPos.z > 0 &&
            screenPos.x > 0 && screenPos.x < 1 &&
            screenPos.y > 0 && screenPos.y < 1;
    }

    private void OnEnterCamera()
    {
        m_isWithinCamera = true;
        TryProgress(m_weatherManager.CurrentWeather);
    }

    private void OnExitCamera()
    {
        m_isWithinCamera = false;
    }

    //イベント受け取り.
    public void OnWeatherChanged(WeatherManager.WeatherType newWeather)
    {
        if (m_isWithinCamera == false) return;
        TryProgress(newWeather);
    }

    //進行度チェック.
    private void TryProgress(WeatherManager.WeatherType weatherType)
    {
        //nullチェック.
        if (m_weatherPriorities == null || m_weatherPriorities.Count == 0)
        {
            Debug.LogError("must set weatherPriorities");
            return;
        }

        if (m_progress >= m_weatherPriorities.Count) return;

        var expectedWeather = m_weatherPriorities[m_progress].w_weatherType;

        if (weatherType == expectedWeather)
        {
            m_progress++;
        }
        else
        {
            m_progress = 0;
        }
    }

    //進行度が最大まで上昇するとtrueを返す.
    public bool IsGimmickActive()
    {
        return m_progress >= m_weatherPriorities.Count;
    }
}
