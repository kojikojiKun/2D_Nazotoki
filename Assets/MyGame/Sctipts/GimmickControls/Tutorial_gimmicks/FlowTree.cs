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
    [SerializeField] private float m_targetSpeed; //最大の流れの速さ.
    [SerializeField] private float m_smooth; //水流の加速の強さ.
    [SerializeField] private float m_maxHeight; //浮かぶ上限.
    [SerializeField] private float m_maxBuoyancy; //浮力の最大値.
    [SerializeField] private float m_buoyancySpeed; //浮力の増える速度.

    private float m_buoyancyProgress = 0; //原因の浮き上がりの進行度.
    private float m_initHeight; //初期のy座標.
    private Rigidbody m_rb;
    private MoveMode m_moveMode;

    // Start is called before the first frame update
    void Start()
    {
        //必要な要素を参照.
        m_rb = GetComponent<Rigidbody>();

        m_moveMode = MoveMode.dry;
        m_initHeight = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_moveMode == MoveMode.flow)
        {
            Flow();
        }

        //カメラに映らなくなったら無効化.
        if (transform.position.z < -15f)
        {
            gameObject.SetActive(false);
        }
    }

    //木を流す.
    void Flow()
    {
        Vector3 targetVelocity = m_flowDir.normalized * m_targetSpeed;

        ApplyBuoyancy();

        //水流にゆっくり速度を近づける.
        m_rb.velocity = Vector3.Lerp(
            m_rb.velocity,
            targetVelocity,
            Time.deltaTime * m_smooth
            );
    }

    //浮力の処理.
    void ApplyBuoyancy()
    {
        //現在のy座標を求める.
        float currentHeight = transform.position.y;

        //y座標が上限を超えると浮力をかけない.
        if (currentHeight > m_maxHeight)
        {
            return;
        }

        //進捗を増やす(0～1)
        m_buoyancyProgress += Time.deltaTime * m_buoyancySpeed;
        m_buoyancyProgress = Mathf.Clamp01(m_buoyancyProgress);

        //浮力の計算.
        float buoyancy = Mathf.Lerp(0f, m_maxBuoyancy, m_buoyancyProgress);

        //上方向に力を加える.
        m_rb.AddForce(Vector3.up * buoyancy, ForceMode.Acceleration);
    }

    private void OnTriggerEnter(Collider other)
    {
        //水流に触れたとき.
        if (other.CompareTag("waterTrigger"))
        {
            m_moveMode = MoveMode.flow;
            m_rb.isKinematic = false;
        }
    }
}
