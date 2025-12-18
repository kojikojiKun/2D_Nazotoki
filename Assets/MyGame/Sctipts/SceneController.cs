using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController s_instance;

    private void Awake()
    {
        //インスタンス化.
        if (s_instance != null && s_instance != this)
        {
            Destroy(s_instance);
            return;
        }
        s_instance = this;
        DontDestroyOnLoad(s_instance);
    }

    public void StartGame()
    {
        //チュートリアル未プレイならチュートリアル開始.
        if (PlayerPrefs.HasKey(GameManager.HAS_LANCHED))
        {
            //チュートリアルプレイ済みならステージ選択シーンに遷移.
            LoadSelectStage();
        }
        else
        {
            SceneManager.LoadScene("Tutolial");
        }
    }

    //ステージ選択画面.
    public void LoadSelectStage()
    {     
        SceneManager.LoadScene("SelectStage");
    }

    //タイトル画面.
    public void LoadTitle()
    {
        SceneManager.LoadScene("Title");
    }

    //選択されたステージ.
    public void LoadStage(string stage)
    {
        SceneManager.LoadScene(stage);
    }
}
