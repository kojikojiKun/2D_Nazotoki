using UnityEngine;
using System.Collections;

public class IcicleGimmick : MonoBehaviour
{
    [SerializeField] Icicle[] m_icicle;
    [SerializeField] private float m_generateInterval;
    [SerializeField] Transform m_generatePos;
    BaseGimmickCtrl m_gimmickCtrl;

    private bool m_isGenerated = false;

    private void Awake()
    {
        m_gimmickCtrl = GetComponent<BaseGimmickCtrl>();
    }

    private void Start()
    {
        for(int i=0; i < m_icicle.Length; i++)
        {
            m_icicle[i].gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!m_gimmickCtrl.IsGimmickActive())
            return;

        if (m_gimmickCtrl.CurrentWeather == WeatherManager.WeatherType.snow && m_isGenerated==true)
        {
            m_isGenerated = false;
            StartCoroutine(GenerateIcicle());
        }
    }

    private IEnumerator GenerateIcicle()
    {
        yield return new WaitForSeconds(m_generateInterval);
        int random = Random.Range(0, m_icicle.Length);

        m_icicle[random].gameObject.SetActive(true);
        m_icicle[random].StartEnlarge();
        m_isGenerated = true;
    }
}
