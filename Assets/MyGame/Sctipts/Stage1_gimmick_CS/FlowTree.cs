using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowTree : MonoBehaviour
{
    private enum MoveMode
    {
        dry, //動かさない.
        flow //木を流す.
    }

    [SerializeField] Vector3 m_flowDir; //水流の方向.
    [SerializeField] float m_targetSpeed; //最大の流れの速さ.
    [SerializeField] float m_smooth; //水流の加速の強さ.

    private BaseGimmickCtrl m_gimmickCtrl;
    private Rigidbody m_rb;
    private MoveMode m_moveMode;
    private Vector3 m_targetVelosity; //目標速度.

    // Start is called before the first frame update
    void Start()
    {
        //必要な要素を参照.
        m_rb = GetComponent<Rigidbody>();
        m_gimmickCtrl = GetComponentInParent<BaseGimmickCtrl>();

        //目標速度を計算.
        m_targetVelosity = m_flowDir.normalized * m_targetSpeed;

        m_moveMode = MoveMode.dry;
    }

    // Update is called once per frame
    void Update()
    {
        //ギミックをの解除条件（正しい天候を選択）を満たしたとき.
        if (m_gimmickCtrl.IsGimmickActive() == true)
        {
            if (m_moveMode == MoveMode.flow)
            {
                Flow();
            }
        }
    }

    //木を流す.
    void Flow()
    {
        //水流にゆっくり速度を近づける.
        m_rb.velocity = Vector3.Lerp(
            m_rb.velocity,
            m_targetVelosity,
            Time.deltaTime * m_smooth
            );
    }

    private void OnTriggerEnter(Collider other)
    {
        //水流に触れたとき.
        if (other.CompareTag("waterTrigger"))
        {
            m_moveMode = MoveMode.flow;
        }
    }
}
