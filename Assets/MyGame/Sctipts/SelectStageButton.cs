using UnityEngine;
using UnityEngine.UI;

public class SelectStageButton : MonoBehaviour
{
    [SerializeField] private string m_sceneName;
    [SerializeField] private bool m_isLoadTitle = false;
    Button m_button;

    private void Start()
    {
        m_button = GetComponent<Button>();

        //ボタンに設定した名前のシーンに遷移.
        m_button.onClick.AddListener(() =>
        {
            if (m_isLoadTitle == false)
            {
                if (m_sceneName != null)
                {
                    SceneController.s_instance.LoadStage(m_sceneName);
                }
                else
                {
                    Debug.Log("scene name is null !!");
                    SceneController.s_instance.LoadTitle();
                }
            }
            else
            {
                SceneController.s_instance.LoadTitle();
            }
        });
    }
}
