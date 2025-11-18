using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WeatherEventListner : MonoBehaviour
{
    private WeatherManager.WeatherType m_weatherType;
    [SerializeField] List<BaseGimmickCtrl> m_gimmicks;

    //“VŒó‚Ì•Ï‰»ƒCƒxƒ“ƒg‚ðw“Ç.
    private void OnEnable() => WeatherManager.OnWeatherChanged += HandleWeatherChanged;

    //–³Œø‰»‚µ‚½‚Æ‚«w“Ç‰ðœ.
    private void OnDisable() => WeatherManager.OnWeatherChanged -= HandleWeatherChanged;

    //•Ï‰»‚µ‚½“VŒó‚ðŽó‚¯Žæ‚é.
    private void HandleWeatherChanged(WeatherManager.WeatherType newWeather)
    {
        if (m_gimmicks == null || m_gimmicks.Count == 0)
        {
            Debug.LogError("gimmicks is null");
            return;
        }

        //BaseGimmickCtrl‚É“VŒó‚ð“n‚·.
        foreach (var gimmick in m_gimmicks)
        {
            gimmick.OnWeatherChanged(newWeather);
        }
    }
}
