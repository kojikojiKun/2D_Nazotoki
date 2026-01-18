using UnityEngine;
using UnityEngine.UI;

public class SelectStageButton : MonoBehaviour
{
    [SerializeField] private string m_loadSceneName;
    [SerializeField] private bool m_isLoadTitle = false;

    private Button m_button;

    private void Start()
    {
        if (!TryGetComponent(out m_button))
        {
            Debug.LogError("Button component not found.");
            return;
        }

        m_button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (SceneController.s_instance == null)
        {
            Debug.LogError("SceneController is null.");
            return;
        }

        if (m_isLoadTitle)
        {
            SceneController.s_instance.LoadTitle();
            return;
        }

        if (!string.IsNullOrEmpty(m_loadSceneName))
        {
            SceneController.s_instance.LoadStage(m_loadSceneName);
        }
        else
        {
            Debug.LogWarning("scene name is empty. Load Title.");
            SceneController.s_instance.LoadTitle();
        }
    }
}
