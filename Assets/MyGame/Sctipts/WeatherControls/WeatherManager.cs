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

    [System.Serializable]
    //天気に対応したパーティクルとSE.
    private struct WeatherPtl_SE
    {
        public WeatherType type;
        public ParticleSystem ptl;
        public AudioClip audioClip;
        public Material skybox;

        [Header("Fogの設定")]
        public bool enableFog;
        public Color fogColor;
        public float fogDensity;
    }

    private AudioSource m_audioSource;
    private Coroutine m_seFadeCoroutine;
    [SerializeField] private float m_seFadeDuration; //SEのフェードインにかかる時間.
    [SerializeField] private WeatherPtl_SE[] m_ptl_SE; 
    [SerializeField] GameObject m_particleC; //パーティクルのコンテナ.

    [SerializeField] PlayerController m_player;

    private void Awake()
    {
        //インスタンス化.
        if (s_instance != null && s_instance != this)
        {
            Destroy(s_instance);
            return;
        }
        s_instance = this;

        m_audioSource = GetComponent<AudioSource>();
        
        //初期の天気を設定.
        m_currentWeather = WeatherType.sunny;
        m_currentIndex = (int)m_currentWeather;

        PlayWeatherParticle(m_currentWeather);
    }

    private void Update()
    {
        //パーティクルをプレイヤーに追従させる.
        m_particleC.transform.position = m_player.transform.position;
    }

    public void SetWeather(int newWeatherIndex)
    {
        //同じ天気が選択されていれば処理しない.
        if (m_currentIndex == newWeatherIndex) return;

        m_currentIndex = newWeatherIndex;

        // インデックスから列挙値へ変換.
        m_currentWeather = (WeatherType)m_currentIndex;
        PlayWeatherParticle(m_currentWeather);

        //イベント通知(天候の変化を伝える).
        OnWeatherChanged?.Invoke(m_currentWeather);
    }

    //天候エフェクトを再生.
    private void PlayWeatherParticle(WeatherType weatherType)
    {
       foreach(var data in m_ptl_SE)
        {
            //選択された天気に対応したパーティクルを再生.
            if(data.type == weatherType)
            {
                data.ptl.Play();

                //SEを変える.
                StartSeFadeIn(data.audioClip);

                //skyboxを変える.
                ChangeSkybox(data.skybox);

                //fogの設定を変える.
                ChangeFog(data);
            }
            else
            {
                //ほかの天気は止める.
                if (data.ptl.isPlaying == true)
                {
                    data.ptl.Stop();
                }
            }
        }
    }

    //skyboxの色を変更する.
    private void ChangeSkybox(Material skybox)
    {
        if (skybox == null) return;

        RenderSettings.skybox = skybox;

        // Lighting 更新（重要）
        DynamicGI.UpdateEnvironment();
    }

    //fogの設定を変更する.
    private void ChangeFog(WeatherPtl_SE data)
    {
        RenderSettings.fog = data.enableFog;

        if (data.enableFog == false) return;

        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = data.fogColor;
        RenderSettings.fogDensity = data.fogDensity;
    }


    void StartSeFadeIn(AudioClip se)
    {
        if (m_seFadeCoroutine != null)
        {
            StopCoroutine(m_seFadeCoroutine);
        }

        m_seFadeCoroutine = StartCoroutine(FadeOutInSE(se));
    }

    private IEnumerator FadeOutInSE(AudioClip nextSe)
    {
        // フェードアウト
        float t = 0f;
        float startVolume = m_audioSource.volume;

        while (t < m_seFadeDuration)
        {
            t += Time.deltaTime;
            m_audioSource.volume =
                Mathf.Lerp(startVolume, 0f, t / m_seFadeDuration);
            yield return null;
        }

        m_audioSource.volume = 0f;
        m_audioSource.Stop();

        // clip切り替え
        m_audioSource.clip = nextSe;
        m_audioSource.Play();

        // フェードイン
        t = 0f;
        while (t < m_seFadeDuration)
        {
            t += Time.deltaTime;
            m_audioSource.volume =
                Mathf.Lerp(0f, 1f, t / m_seFadeDuration);
            yield return null;
        }

        m_audioSource.volume = 1f;
    }
}
