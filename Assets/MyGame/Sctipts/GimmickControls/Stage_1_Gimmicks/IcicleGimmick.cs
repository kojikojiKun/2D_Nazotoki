using UnityEngine;

public class IcicleGimmick : MonoBehaviour
{
    [SerializeField] GameObject m_iciclePrefab;
    BaseGimmickCtrl m_gimmickCtrl;

    private void Awake()
    {
        m_gimmickCtrl = GetComponent<BaseGimmickCtrl>();   
    }

    private void Update()
    {
        if (!m_gimmickCtrl.IsGimmickActive())
            return;
    }

    void GenerateIcicle()
    {

    }
}
