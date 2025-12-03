using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class WeatherManager : MonoBehaviour
{
    private int m_currentIndex;
    private WeatherType m_currentWeather;

    public static WeatherManager s_instance { get; private set; }
    public static event Action<WeatherType> OnWeatherChanged; //天候の変化イベント.
    public WeatherType CurrentWeather => m_currentWeather;
    public enum WeatherType
    {
        rainy, //雨.
        sunny, //晴れ.
        snow, //雪.
        windy //強風.
    }

    private void Awake()
    {
        if (s_instance != null && s_instance != this)
        {
            Destroy(s_instance);
            return;
        }
        s_instance = this;
        
        m_currentWeather = WeatherType.sunny;
        m_currentIndex = (int)m_currentWeather;
    }

    public void SetWeather(int newWeatherIndex)
    {
        //同じ天気が選択されていれば処理しない.
        if (m_currentIndex == newWeatherIndex) return;

        m_currentIndex = newWeatherIndex;

        // インデックスから列挙値へ変換.
        m_currentWeather = (WeatherType)m_currentIndex;

        //イベント通知(天候の変化を伝える).
        OnWeatherChanged?.Invoke(m_currentWeather);
    }
}
