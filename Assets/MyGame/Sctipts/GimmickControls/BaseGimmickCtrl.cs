using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class WeatherPriority
{    
   [Header("ギミックが反応する天候(orderを小さい順に)")] public WeatherManager.WeatherType w_weatherType;
   [Header("反応する順序")] public int w_order;
}

public class BaseGimmickCtrl : MonoBehaviour
{
    [SerializeField] WeatherManager m_weatherManager;
    [SerializeField] private List<WeatherPriority> m_weatherPrioritiesList;
    private int m_progress = 0; //ギミックの進行度.
    private bool m_withinCamera = false; //カメラの範囲内.
    private bool m_isCheckedWeather = false;

    private void Update()
    {
        CheckCameraVisibility();

        if (m_withinCamera == true)
        {
            InitialWeatherCheck();
        }
    }

    //カメラに移っているとき.
    private void CheckCameraVisibility()
    {
        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);

        //オブジェクトがカメラの描画範囲内かをチェック.
        m_withinCamera = screenPos.z > 0 &&
            screenPos.x > 0 && screenPos.x < 1 &&
            screenPos.y > 0 && screenPos.y < 1;
    }

    //カメラの範囲内に入ったとき、現在の天気を取得.
    private void InitialWeatherCheck()
    {
        //チェック済みなら実行しない.
        if (m_isCheckedWeather == false)
        {
            //現在の選択されている天気を取得.
            var currentWeather = m_weatherManager.CurrentWeather;

            //天候が正しければ進行度上昇.
            if (m_progress < m_weatherPrioritiesList.Count)
            {
                if (m_weatherPrioritiesList[m_progress].w_weatherType == currentWeather)
                {
                    m_progress++;
                    m_isCheckedWeather = true;
                }
            }
        }
    }

    //天気がギミックの解除条件といっちしているかをチェック.
    private void WeatherCheck(WeatherManager.WeatherType weatherType)
    {
        //進行度が範囲内なら、次に選択すべき天候を取得.
        if (m_progress < m_weatherPrioritiesList.Count)
        {
            var nextWeather = m_weatherPrioritiesList[m_progress].w_weatherType;

            if (weatherType == nextWeather)
            {
                //選択した天候が正解なら進行度上昇.
                m_progress++;
                Debug.Log("select succes!!");
            }
            //天候を間違えると進行度リセット.
            else
            {
                m_progress = 0;
                Debug.Log("select failed...");
            }
        }
    }

    //変化した天候を受け取る.
    public void OnWeatherChanged(WeatherManager.WeatherType weatherType)
    {
        //nullチェック.
        if (m_weatherPrioritiesList == null || m_weatherPrioritiesList.Count == 0)
        {
            Debug.LogError("must set weatherPriorities");
            return;
        }

        //カメラの範囲外のとき.
        if (m_withinCamera == false)
        {
            return;
        }

        WeatherCheck(weatherType);
    }

    //進行度が最大まで上昇するとtrueを返す.
    public bool IsGimmickActive()
    {
        return m_progress >= m_weatherPrioritiesList.Count;
    }
}
