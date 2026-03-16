using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowTree : MonoBehaviour
{
    private enum MoveMode
    {
        dry,
        flow
    }

    [SerializeField] Vector3 m_flowDir;
    [SerializeField] private float m_targetSpeed;
    [SerializeField] private float m_smooth;
    [SerializeField] private float m_maxHeight;
    [SerializeField] private float m_maxBuoyancy;
    [SerializeField] private float m_buoyancySpeed;

    private float m_buoyancyProgress = 0;
    private Rigidbody m_rb;
    private MoveMode m_moveMode;

    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();

        m_moveMode = MoveMode.dry;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_moveMode == MoveMode.flow)
        {
            Flow();
        }

        if (transform.position.z < -15f)
        {
            gameObject.SetActive(false);
        }
    }

    void Flow()
    {
        Vector3 targetVelocity = m_flowDir.normalized * m_targetSpeed;

        ApplyBuoyancy();

        m_rb.linearVelocity = Vector3.Lerp(
            m_rb.linearVelocity,
            targetVelocity,
            Time.deltaTime * m_smooth
            );
    }
    void ApplyBuoyancy()
    {

        float currentHeight = transform.position.y;

        if (currentHeight > m_maxHeight)
        {
            return;
        }

        m_buoyancyProgress += Time.deltaTime * m_buoyancySpeed;
        m_buoyancyProgress = Mathf.Clamp01(m_buoyancyProgress);

        float buoyancy = Mathf.Lerp(0f, m_maxBuoyancy, m_buoyancyProgress);

        m_rb.AddForce(Vector3.up * buoyancy, ForceMode.Acceleration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("waterTrigger"))
        {
            m_moveMode = MoveMode.flow;
            m_rb.isKinematic = false;
        }
    }
}
