using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

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
        FirstReference(m_currentSceneName);
        PlaySceneBGM(m_currentSceneName);
    }

    PlayerController m_playerCntl;
    Transform m_respownPos;
    PlayerInput m_playerInput;
    GameObject m_clearWindow;
    const string m_titleSceneName = "Title";
    const string m_selectSceneName = "SelectStage";

    //ゲームプレイに必要な要素を参照.
    void FirstReference(string sceneName)
    {
        //タイトルとステージ選択のシーンならreturn.
        if (sceneName == m_titleSceneName || sceneName == m_selectSceneName)
            return;

        //プレイヤーGameObjectを参照.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            m_playerCntl = player.GetComponent<PlayerController>();

            if (m_playerCntl != null)
                m_playerInput = player.GetComponent<PlayerInput>();
        }

        
        //プレイヤーの初期位置を設定.
        m_respownPos = GameObject.FindGameObjectWithTag("firstPos").transform;
        if(m_respownPos!=null)
            Respawn();
        
        //クリアwindowを参照.
        m_clearWindow = GameObject.FindGameObjectWithTag("clearWindow");
        if(m_clearWindow!=null)
            m_clearWindow.SetActive(false);
    }

    //チェックポイントの座標を受け取る.
    public void ReceiveCheckPoint(Transform checkPoint)
    {
        m_respownPos.position = checkPoint.position;
    }

    //プレイヤーをリスポーンさせる.
    public void Respawn()
    {
        m_playerCntl.transform.position = m_respownPos.transform.position;
    }

    //ステージクリア時
    public IEnumerator ClearStage()
    {
        //プレイヤーの操作無効化.
        m_playerInput.enabled = false;

        float delayTime = 1.5f;
        yield return new WaitForSeconds(delayTime);

        m_clearWindow.SetActive(true);
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
