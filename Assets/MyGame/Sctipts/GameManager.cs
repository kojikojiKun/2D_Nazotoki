using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager s_instance;
    public const string HAS_LANCHED = "HasLanched";

    [System.Serializable]
    private struct SceneBGM
    {
        public string sceneName;
        public AudioClip bgm;
    }

    private AudioSource m_audioSource;
    private string m_currentSceneName;
    [SerializeField] SceneBGM[] m_stageBGMs;

    private void Awake()
    {
        //インスタンス化.
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        s_instance = this;
        DontDestroyOnLoad(s_instance);

        m_audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        m_currentSceneName = scene.name;
        PlaySceneBGM(m_currentSceneName);
    }

    //シーンに対応したBGMを再生.
    void PlaySceneBGM(string sceneName)
    {
        foreach (var data in m_stageBGMs)
        {
            if (data.sceneName == sceneName)
            {
                m_audioSource.clip = data.bgm;
                m_audioSource.Play();
                return;
            }
        }

        //見つからなければ停止.
        m_audioSource.Stop();
    }
}
