using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController s_instance;
    private void Awake()
    {
        if (s_instance != null && s_instance != this)
        {
            Destroy(s_instance);
            return;
        }
        s_instance = this;
        DontDestroyOnLoad(s_instance);
    }

    public void LoadSelectStage()
    {
        SceneManager.LoadScene("SelectStage");
    }
}
