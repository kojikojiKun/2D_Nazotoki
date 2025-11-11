using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ArrangeWheelSlices : MonoBehaviour
{
    [SerializeField] private RectTransform[] m_items;
    [SerializeField] private float m_radius = 100f;
    [SerializeField] private Transform m_center; // 中心を指定

    void Update()
    {
        if (m_items == null || m_items.Length == 0 || m_center == null)
            return;

        int cnt = m_items.Length;

        for (int i = 0; i < cnt; i++)
        {
            float angle = i * Mathf.PI * 2f / cnt;

            // m_centerからのXY相対位置を計算
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * m_radius;

            // Z座標はそのまま保持
            Vector3 newPos = new Vector3(m_center.position.x + offset.x,
                                         m_center.position.y + offset.y,
                                         m_items[i].position.z);

            m_items[i].position = newPos;

            // 中心に向けて回転させたい場合
            m_items[i].rotation = Quaternion.Euler(0, 0, -angle * Mathf.Rad2Deg);
        }
    }
}
