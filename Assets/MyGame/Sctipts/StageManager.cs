using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager s_instance;
    Transform m_checkPoint;

    private void Awake()
    {
        if (s_instance != null && s_instance != this)
        {
            Destroy(s_instance);
            return;
        }
        s_instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReceiveCheckPoint(Transform lastCheckPoint)
    {

    }

    public void Respown()
    {

    }
}
