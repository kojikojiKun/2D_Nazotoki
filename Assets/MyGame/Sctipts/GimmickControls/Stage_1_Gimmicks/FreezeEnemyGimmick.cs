using UnityEngine;
using System.Collections;

public class FreezeEnemyGimmick : MonoBehaviour
{

    [SerializeField] Material m_water;
    [SerializeField] GameObject m_enemy;
    [SerializeField] Color m_defColor;
    [SerializeField] Collider2D m_collider;
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
        SetMaterialValue(0.1f, 0.1f);
        ChangeWaterMaterialBrihgtness();
    }

    private void Update()
    {
        if (!m_gimmickCtrl.IsGimmickActive())
            return;

        if (m_gimmickCtrl.CurrentWeather == WeatherManager.WeatherType.snow && !m_isWaveStopped)
        {
            m_isWaveStopped = true;
            m_collider.enabled = true;
            SetMaterialValue(0, 0);
            FreezeEnemy();
        }
        else if (m_gimmickCtrl.CurrentWeather == WeatherManager.WeatherType.sunny && m_isWaveStopped)
        {
            m_isWaveStopped = false;
            m_collider.enabled = false;
            SetMaterialValue(0.1f, 0.1f);
            DeFrostEnemy();
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

    void ChangeWaterMaterialBrihgtness()
    {
        if (!m_isWaveStopped)
        {
            m_water.SetColor("_Color", m_defColor);
            return;
        }

        Color color = m_water.GetColor("_Color");

        Color.RGBToHSV(color, out float h, out float s, out float v);
        v += 1f;
        v = Mathf.Clamp01(v);
        s -= 1f;
        s = Mathf.Clamp01(s);

        Color newColor = Color.HSVToRGB(h, s, v);
        m_water.SetColor("_Color", newColor);
    }

    //êÖÇÃmaterialÇÃílÇïœçXÇµÅAîgÇé~ÇﬂÇΩÇËìÆÇ©ÇµÇΩÇËÇ∑ÇÈ.
    void SetMaterialValue(float targetSpeed, float targetDisSpeed)
    {
        m_water.SetFloat("_Speed", targetSpeed);
        m_water.SetFloat("_Displacement_Speed", targetDisSpeed);

        ChangeWaterMaterialBrihgtness();
    }
}
