using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFlowTrigger : MonoBehaviour
{
    [SerializeField] private float m_delayTime; //移動までの待機時間.
    [SerializeField] private float m_flowSpeed; //水流の移動スピード

    private float m_passedTime; //経過時間.
    BaseGimmickCtrl m_gimmickCtrl;

    // Start is called before the first frame update
    void Start()
    {
        m_gimmickCtrl = GetComponentInParent<BaseGimmickCtrl>();
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーが正しい天候を選択したとき.
        if (m_gimmickCtrl.IsGimmickActive() == true)
        {
            MoveTrigger();
        }
    }

    void MoveTrigger()
    {
        m_passedTime += Time.deltaTime; //経過時間を保存.

        //待機時間を過ぎたとき.
        if (m_passedTime >= m_delayTime)
        {
            //オブジェクトを移動させる.
            transform.position += Vector3.back * m_flowSpeed * Time.deltaTime;
        }
    }
}
