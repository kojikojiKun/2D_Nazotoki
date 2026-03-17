using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController s_instance;
    private AudioSource m_audioSource;

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

    public void LoadTitle()
    {
        StartCoroutine(LoadSceneWithSE("Title"));
    }

    public void LoadSelectStage()
    {
        StartCoroutine(LoadSceneWithSE("SelectStage"));
    }

    public void LoadTutolial()
    {
        StartCoroutine(LoadSceneWithSE("Tutolial"));
    }

    public void LoadStage(string stage)
    {
        StartCoroutine(LoadSceneWithSE(stage));
    }

    private IEnumerator LoadSceneWithSE(string sceneName)
    {
        m_audioSource.Play();
        yield return new WaitForSeconds(0.2f); //SEを再生するため待機.
        SceneManager.LoadScene(sceneName);
    }

}
