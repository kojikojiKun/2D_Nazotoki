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
    [SerializeField] private List<WeatherPriority> m_weatherPrioritiesList;
    private int m_progress = 0; //ギミックの進行度.

    //変化した天候を受け取る.
    public void OnWeatherChanged(WeatherManager.WeatherType weatherType)
    {
        if (m_weatherPrioritiesList == null || m_weatherPrioritiesList.Count == 0)
        {
            Debug.LogError("must set weatherPriorities");
            return;
        }

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

    //進行度が最大まで上昇するとtrueを返す.
    public bool IsGimmickActive()
    {
        return m_progress >= m_weatherPrioritiesList.Count;
    }
}
