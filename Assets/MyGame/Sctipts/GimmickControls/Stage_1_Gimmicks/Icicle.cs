using UnityEngine;
using System.Collections;

public class Icicle : MonoBehaviour
{
    [SerializeField] private float m_targetScale;
    [SerializeField] private float m_startScale;
    [SerializeField] private float m_enlargeMag;
    [SerializeField] private float m_fallDistance;
    
    private float m_currentScale;
    private float m_interpolation = 0;
    private float m_totalFall = 0;
    private bool m_isContacted = false;
    private Vector3 m_startPos;
    private Vector3 m_lastPos;
    Rigidbody2D m_rb2D;
    Coroutine m_coroutine;

    private void Awake()
    {
        m_rb2D = GetComponent<Rigidbody2D>();
        m_startPos = this.transform.position;
        m_lastPos = this.transform.position;
    }

    private void Update()
    {
        if(m_coroutine != null)
            return;

        //óéâ∫ãóó£ÇåvéZ.
        float distance = Vector3.Distance(this.transform.position, m_lastPos);
        m_totalFall += distance;
        m_lastPos = this.transform.position;
    }

    public bool IsFallen()
    {
        return m_totalFall >= m_fallDistance || m_isContacted;
    }

    public void StartEnlarge()
    {
        if (m_coroutine != null)
            StopCoroutine(m_coroutine);

        m_coroutine = StartCoroutine(EnlargeIcicle());    
    }

    private IEnumerator EnlargeIcicle()
    {      
        //ScaleÇèôÅXÇ…ëùâ¡.
        while (m_interpolation < 1f)
        {
            m_interpolation += Time.deltaTime * m_enlargeMag;
            m_currentScale = Mathf.Lerp(m_startScale, m_targetScale, m_interpolation);
            transform.localScale = Vector3.one * m_currentScale;

            yield return null;
        }

        transform.localScale = Vector3.one * m_targetScale;

        yield return new WaitForSeconds(1f);
        m_rb2D.gravityScale = 1;

        m_coroutine = null;
    }

    public void Initialize()
    {
        transform.position = m_startPos;
        transform.localScale = new Vector3(m_startScale, m_startScale, m_startScale);
        m_rb2D.linearVelocity = Vector3.zero;
        m_rb2D.gravityScale = 0;
        m_interpolation = 0;
        m_currentScale = m_startScale;
        m_totalFall = 0;
        m_isContacted = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("enemy"))
        {
            m_isContacted = true;

            var enemy = other.gameObject.GetComponentInParent<Enemy_2Ctrl>();
            if (enemy != null)
                enemy.Die();
        }
        else if (other.CompareTag("Player"))
        {
            m_isContacted = false;

            var player = other.GetComponentInParent<PlayerController>();
            if(player != null)
                player.KnockBackPlayer();

        }
    }
}
