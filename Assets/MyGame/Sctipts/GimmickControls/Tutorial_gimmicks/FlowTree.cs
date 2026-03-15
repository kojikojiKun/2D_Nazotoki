using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowTree : MonoBehaviour
{
    private enum MoveMode
    {
        dry, //�������Ȃ�.
        flow //�؂𗬂�.
    }

    [SerializeField] Vector3 m_flowDir; //�����̕���.
    [SerializeField] private float m_targetSpeed; //�ő�̗���̑���.
    [SerializeField] private float m_smooth; //�����̉����̋���.
    [SerializeField] private float m_maxHeight; //�����ԏ��.
    [SerializeField] private float m_maxBuoyancy; //���͂̍ő�l.
    [SerializeField] private float m_buoyancySpeed; //���͂̑����鑬�x.

    private float m_buoyancyProgress = 0; //�����̕����オ��̐i�s�x.
    private float m_initHeight; //������y���W.
    private Rigidbody m_rb;
    private MoveMode m_moveMode;

    // Start is called before the first frame update
    void Start()
    {
        //�K�v�ȗv�f���Q��.
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

        //�J�����ɉf��Ȃ��Ȃ����疳����.
        if (transform.position.z < -15f)
        {
            gameObject.SetActive(false);
        }
    }

    //�؂𗬂�.
    void Flow()
    {
        Vector3 targetVelocity = m_flowDir.normalized * m_targetSpeed;

        ApplyBuoyancy();

        //�����ɂ�����葬�x���߂Â���.
        m_rb.linearVelocity = Vector3.Lerp(
            m_rb.linearVelocity,
            targetVelocity,
            Time.deltaTime * m_smooth
            );
    }

    //���͂̏���.
    void ApplyBuoyancy()
    {
        //���݂�y���W�����߂�.
        float currentHeight = transform.position.y;

        //y���W������𒴂���ƕ��͂������Ȃ�.
        if (currentHeight > m_maxHeight)
        {
            return;
        }

        //�i���𑝂₷(0�`1)
        m_buoyancyProgress += Time.deltaTime * m_buoyancySpeed;
        m_buoyancyProgress = Mathf.Clamp01(m_buoyancyProgress);

        //���͂̌v�Z.
        float buoyancy = Mathf.Lerp(0f, m_maxBuoyancy, m_buoyancyProgress);

        //������ɗ͂�������.
        m_rb.AddForce(Vector3.up * buoyancy, ForceMode.Acceleration);
    }

    private void OnTriggerEnter(Collider other)
    {
        //�����ɐG�ꂽ�Ƃ�.
        if (other.CompareTag("waterTrigger"))
        {
            m_moveMode = MoveMode.flow;
            m_rb.isKinematic = false;
        }
    }
}
