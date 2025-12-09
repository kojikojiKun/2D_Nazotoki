using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    [SerializeField] GameObject m_moveExplainTxt;
    [SerializeField] GameObject m_jumpExplainTxt;
    [SerializeField] GameObject m_crouchExplainTxt;
    [SerializeField] GameObject m_weatherExplainTxt;

    private readonly float m_txtActiveTime = 3f;

    private bool m_isDisplayMoveTxt;
    private bool m_isDisplayJumpTxt;
    private bool m_isDisplayCrouchTxt;
    private bool m_isDisplayWeatherTxt;
    private Coroutine m_activeCoroutine;

    void Start()
    {
        ShowTextOnce("moveText");
    }

    private void Update()
    {
        Debug.Log(m_activeCoroutine);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("textTrigger"))
        {
            return;
        }

        ShowTextOnce(collision.gameObject.name);
    }

    private void ShowTextOnce(string name)
    {
        if (name.Contains("jumpText") && m_isDisplayJumpTxt == true) return;
        if (name.Contains("crouchText") && m_isDisplayCrouchTxt == true) return;
        if (name.Contains("weatherText") && m_isDisplayWeatherTxt == true) return;

        //ほかのテキスト表示中なら古いテキストを非表示.
        if (m_activeCoroutine != null)
        {
            CancelDisplayText();
            StopCoroutine(m_activeCoroutine);
            m_activeCoroutine = null;
        }

        //テキストを表示.
        m_activeCoroutine = StartCoroutine(DelayDisableText(name));

        m_isDisplayMoveTxt = true;
        if (name.Contains("jumpText")) m_isDisplayJumpTxt = true;
        if (name.Contains("crouchText")) m_isDisplayCrouchTxt = true;
        if (name.Contains("weatherText")) m_isDisplayWeatherTxt = true;
    }

    //テキストを一定時間だけ表示.
    private IEnumerator DelayDisableText(string name)
    {
        //一定時間テキストを表示.
        ShowCorrectText(name);
        yield return new WaitForSeconds(m_txtActiveTime);

        CancelDisplayText();
        m_activeCoroutine = null;
        CancelDisplayText();
    }

    //テキストを表示する.
    private void ShowCorrectText(string name)
    {
        if (name.Contains("move")) m_moveExplainTxt.SetActive(true);
        if (name.Contains("jump")) m_jumpExplainTxt.SetActive(true);
        if (name.Contains("crouch")) m_crouchExplainTxt.SetActive(true);
        if (name.Contains("weather")) m_weatherExplainTxt.SetActive(true);
    }

    //テキストを非表示にする.
    void CancelDisplayText()
    {
        m_moveExplainTxt.SetActive(false);
        m_jumpExplainTxt.SetActive(false);
        m_crouchExplainTxt.SetActive(false);
        m_weatherExplainTxt.SetActive(false);
    }
}
