using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] PlayerController m_target; //追跡対象のオブジェクト.
    [SerializeField] private Vector3 offset; //追跡対象からの距離.
    private bool m_canChasing = true; //追跡フラグ.

    private void Awake()
    {
        if (m_target == null)
        {
            m_target = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }
    }

    //追跡中止.
    public void StopChase()
    {
        m_canChasing = false;
    }

    //位置を固定する.
    public void FixedPosition(Transform tr)
    {
        StopChase();
        gameObject.transform.position = tr.position;
    }

    //追跡再開.
    public void StartChase() 
    {
        m_canChasing = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_target != null)
        {
            //目標から一定距離離して追跡.
            if (m_canChasing == true)
            {
                transform.position = m_target.transform.position + new Vector3(offset.x, offset.y, offset.z);
            }
        }
    }
}
