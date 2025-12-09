using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    [SerializeField] GameObject m_moveExplainTxt;
    [SerializeField] GameObject m_jumpExplainTxt;
    [SerializeField] GameObject m_crouchExplainTxt;
    [SerializeField] GameObject m_weatherExplainTxt;

    private float m_txtDelay = 2f;
    private float m_txtActiveTime = 3f;
    private float m_displayTime;
    private float m_passedTime;

    private bool m_isDisplayMoveTxt;
    private bool m_isDisplayJumpTxt;
    private bool m_isDisplayCrouchTxt;
    private bool m_isDisplayWeatherTxt;

    private bool m_isCounting;

    void Start()
    {
        StartCoroutine(DelayDisableText(m_moveExplainTxt));
        m_isDisplayMoveTxt = true;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string name = collision.gameObject.name;

        if (TryShowText(name, collision.gameObject))
        {
            return;
        }
    }

    private bool TryShowText(string name, GameObject target)
    {
        //指定した文字列を含むオブジェクト以外は除外.
        if (!(
            name.Contains("jumpText") ||
            name.Contains("crouchText") ||
            name.Contains("weatherText")
            )) return false;

        //ほかのテキスト表示中なら古いテキストを非表示.
        if (m_isCounting == true)
        {
            CancelDisplayText();
        }

        //テキストを表示.
        StartCoroutine(DelayDisableText(target));

        if (name.Contains("jumpText")) m_isDisplayJumpTxt = true;
        else if (name.Contains("crouchText")) m_isDisplayCrouchTxt = true;
        else if (name.Contains("weatherText")) m_isDisplayWeatherTxt = true;

        return true;
    }

    //テキストを一定時間だけ表示.
    private IEnumerator DelayDisableText(GameObject txtObj)
    {
        if (txtObj != null)
        {
            //テキスト表示.
            txtObj.SetActive(true);

            //一定時間待機.
            m_isCounting = true;
            yield return new WaitForSeconds(m_txtActiveTime);
            m_isCounting = false;

            //テキスト非表示.
            txtObj.SetActive(false);
        }
    }

    void CancelDisplayText()
    {
        m_moveExplainTxt.SetActive(false);
        m_jumpExplainTxt.SetActive(false);
        m_crouchExplainTxt.SetActive(false);
        m_weatherExplainTxt.SetActive(false);

        StopCoroutine(DelayDisableText(null));
        m_isCounting = false;
    }
}
