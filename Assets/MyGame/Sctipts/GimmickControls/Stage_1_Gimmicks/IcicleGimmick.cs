using UnityEngine;
using System.Collections;

public class IcicleGimmick : MonoBehaviour
{
    [SerializeField] Icicle m_icicle;
    [SerializeField] private float m_generateInterval;
    [SerializeField] Transform[] m_generatePos;
    BaseGimmickCtrl m_gimmickCtrl;
    Coroutine m_coroutine;

    private void Awake()
    {
        m_gimmickCtrl = GetComponent<BaseGimmickCtrl>();
    }

    private void Start()
    {
        m_icicle.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!m_gimmickCtrl.IsGimmickActive())
            return;

        if (m_gimmickCtrl.CurrentWeather == WeatherManager.WeatherType.snow)
        {
            if (m_coroutine == null)
            {
                m_coroutine = StartCoroutine(ActiveIcicle());
            }
        }
    }

    //一定時間ごとにオブジェクトActive
    private IEnumerator ActiveIcicle()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_generateInterval);

            int random = Random.Range(0, m_generatePos.Length);

            m_icicle.Initialize(m_generatePos[random]);
            m_icicle.gameObject.SetActive(true);

            if(m_icicle.IsMoveToLimit()==true)
            m_icicle.StartEnlarge();
        }
    }
}
