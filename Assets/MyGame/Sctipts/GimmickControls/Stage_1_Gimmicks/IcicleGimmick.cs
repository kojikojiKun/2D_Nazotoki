using UnityEngine;
using System.Collections;

public class IcicleGimmick : MonoBehaviour
{
    [SerializeField] Icicle m_icicle;
    [SerializeField] private float m_playParticleInterval;
    [SerializeField] private float m_generateInterval;
    private WeatherManager.WeatherType? m_runningWeather = null;
    BaseGimmickCtrl m_gimmickCtrl;
    Coroutine m_coroutine;

    private void Awake()
    {
        m_gimmickCtrl = GetComponent<BaseGimmickCtrl>();
    }

    private void Start()
    {
        m_icicle.gameObject.SetActive(false);
    }

    private void Update()
    {
        var weather = m_gimmickCtrl.CurrentWeather;
        if (m_runningWeather == weather)
            return;

        StopCurrent();

        if (m_gimmickCtrl.IsGimmickActive() == false)
        {
            m_coroutine = StartCoroutine(WatarDrop());
        }
        else
        {
            if (m_gimmickCtrl.CurrentWeather == WeatherManager.WeatherType.snow)
            {
                m_coroutine = StartCoroutine(ActiveIcicle());
            }
            else
            {
                m_coroutine = StartCoroutine(WatarDrop());
            }
        }

        m_runningWeather = weather;
    }

    private void StopCurrent()
    {
        if (m_coroutine != null)
        {
            StopCoroutine(m_coroutine);
            m_coroutine = null;
        }

        m_runningWeather = null;
    }

    private IEnumerator WatarDrop()
    {
        while (true)
        {
            ParticlePlayer.s_instance.PlayParticle(ParticleType.WatarDrop);
            yield return new WaitForSeconds(m_playParticleInterval);
        }
    }

    private IEnumerator ActiveIcicle()
    {
        while (true)
        {
            m_icicle.gameObject.SetActive(true);
            m_icicle.Initialize();
            m_icicle.StartEnlarge();
            yield return new WaitForSeconds(m_generateInterval);
        }
    }
}
