using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] GameObject m_moveExplainTxt;
    [SerializeField] GameObject m_jumpExplainTxt;
    [SerializeField] GameObject m_crouchExplainTxt;
    [SerializeField] GameObject m_tutorialEventTxt;
    [SerializeField] GameObject m_panel;
    [SerializeField] VideoPlayer m_videoPlayer;
    [SerializeField] Button m_closeButton;

    PlayerController m_playerCtrl;
    PlayerAnimation m_playerAnim;
    WeatherController m_weatherCtrl;

    private readonly float m_txtActiveTime = 3f;

    private bool m_isDisplayJumpTxt;
    private bool m_isDisplayCrouchTxt;
    private bool m_isDisplayWeatherTxt;
    private Coroutine m_activeCoroutine;

    void Start()
    {
        m_playerCtrl = GetComponent<PlayerController>();
        m_playerAnim = GetComponent<PlayerAnimation>();
        m_weatherCtrl = GetComponent<WeatherController>();

        //移動チュートリアルを表示.
        ShowTextOnce("moveText");

        //ボタンのクリックを検知.
        m_closeButton.onClick.AddListener(() =>
        {
            CloseTutorialWindow();
        });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("textTrigger"))
        {
            return;
        }

        //チュートリアルテキストを表示.
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
        if (name.Contains("weather"))
        {
            m_tutorialEventTxt.SetActive(true);

        　　//天気の操作チュートリアルの説明windowを表示.
            StartCoroutine(OpenTurorialWindow());
        }
    }

    //チュートリアルwindowを開く.
    private IEnumerator OpenTurorialWindow()
    {
        yield return new WaitForSeconds(2f);

        //時間停止.
        Time.timeScale = 0;

        //キャラクターの操作を不可能にする.
        DisablePlayerControls();

        //パネル表示.
        m_panel.SetActive(true);

        //動画の点滅防止.
        yield return null;

        //動画が再生可能になるまで待機.
        m_videoPlayer.Prepare();
        while (m_videoPlayer.isPrepared == false)
        {
            yield return null;
        }

        //動画再生.
        m_videoPlayer.Play();
    }

    //チュートリアルwindowを閉じる.
    void CloseTutorialWindow()
    {
        //パネルを非表示.
        m_panel.SetActive(false);

        //時間停止解除.
        Time.timeScale = 1f;

        //キャラクターの操作を可能にする.
        EnablePlayerControls();
    }

    //キャラクターの操作を無効化.
    void DisablePlayerControls()
    {
        m_playerCtrl.enabled = false;
        m_playerAnim.enabled = false;
        m_weatherCtrl.enabled = false;
    }

    //キャラクターの操作無効化を解除.
    void EnablePlayerControls()
    {
        m_playerCtrl.enabled = true;
        m_playerAnim.enabled = true;
        m_weatherCtrl.enabled = true;
    }

    //テキストを非表示にする.
    void CancelDisplayText()
    {
        m_moveExplainTxt.SetActive(false);
        m_jumpExplainTxt.SetActive(false);
        m_crouchExplainTxt.SetActive(false);
        m_tutorialEventTxt.SetActive(false);
    }
}
