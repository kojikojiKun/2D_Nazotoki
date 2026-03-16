using UnityEngine;
using System.Collections;

public class FreezeEnemyGimmick : MonoBehaviour
{

    [SerializeField] Renderer m_water;
    [SerializeField] GameObject m_enemy;

    private Color m_defColor;
    private bool m_isWaveStopped = false;

    BaseGimmickCtrl m_gimmickCtrl;   
    Enemy_1Ctrl m_enemyCtrl;
    Animator m_enemyAnimator;

    private void Awake()
    {
        m_gimmickCtrl = GetComponent<BaseGimmickCtrl>();
        m_enemyCtrl = m_enemy.GetComponent<Enemy_1Ctrl>();
        m_enemyAnimator = m_enemy.GetComponent<Animator>();
    }

    private void Start()
    {
        m_defColor = m_water.material.GetColor("_Color");
    }

    private void Update()
    {
        if (m_gimmickCtrl.CurrentWeather == WeatherManager.WeatherType.snow && !m_isWaveStopped)
        {
            StartCoroutine(SetMaterialValue(0, 0));
            FreezeEnemy();
            m_isWaveStopped = !m_isWaveStopped;
        }
        else if (m_gimmickCtrl.CurrentWeather == WeatherManager.WeatherType.sunny && m_isWaveStopped)
        {
            StartCoroutine(SetMaterialValue(0.1f, 0.1f));
            DeFrostEnemy();
            m_isWaveStopped = !m_isWaveStopped;
        }
    }

    void FreezeEnemy()
    {
        m_enemyAnimator.enabled = false;
        m_enemyCtrl.CanMove = false;
    }

    void DeFrostEnemy()
    {
        m_enemyAnimator.enabled = true;
        m_enemyCtrl.CanMove = true;
    }

    void ChangeWaterMaterialBrihgtness(bool isFrose)
    {
        if (!isFrose)
        {
            m_water.material.SetColor("_Color", m_defColor);
            return;
        }

        Color color = m_water.material.GetColor("_Color");

        Color.RGBToHSV(color, out float h, out float s, out float v);
        v += 5f;
        v = Mathf.Clamp01(v);

        Color newColor = Color.HSVToRGB(h, s, v);
        m_water.material.SetColor("_Color", newColor);
    }

    private IEnumerator SetMaterialValue(float targetSpeed, float tartgetDisSpeed)
    {
        while (true)
        {
            float speed = m_water.material.GetFloat("_Speed");
            float disSpeed = m_water.material.GetFloat("_Displacement_Speed");

            speed = Mathf.MoveTowards(speed, targetSpeed, 0.1f);
            disSpeed = Mathf.MoveTowards(disSpeed, tartgetDisSpeed, 0.1f);

            m_water.material.SetFloat("_Speed", Mathf.Lerp(speed, targetSpeed,0.2f));
            m_water.material.SetFloat("_Displacement_Speed", Mathf.Lerp(disSpeed, tartgetDisSpeed, 0.2f));

            yield return new WaitForSeconds(0.5f);
        }
    }
}
