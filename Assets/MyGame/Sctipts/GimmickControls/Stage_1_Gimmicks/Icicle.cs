using UnityEngine;
using System.Collections;

public class Icicle : MonoBehaviour
{
    [SerializeField] private float m_targetScale;
    [SerializeField] private float m_startScale;
    [SerializeField] private float m_enlargeMag;
    private float m_currentScale;
    private float m_interpolation = 0;

    Rigidbody2D m_rb2D;

    private void Awake()
    {
        m_rb2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        StartEnlarge();
    }

    public void StartEnlarge()
    {
        m_rb2D.gravityScale = 0;
        StartCoroutine(EnlargeIcicle());    
    }

    private IEnumerator EnlargeIcicle()
    {
        while ((m_targetScale - m_currentScale) >= 0.01f)
        {
            m_interpolation += Time.deltaTime * m_enlargeMag;
            m_currentScale = Mathf.Lerp(m_startScale, m_targetScale, m_interpolation);
            transform.localScale = new Vector3(m_currentScale, m_currentScale, m_currentScale);

            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        
        m_rb2D.gravityScale = 1;
    }

    public void InitializeScale()
    {
        transform.localScale = new Vector3(m_startScale, m_startScale, m_startScale);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("enemy") || other.CompareTag("Player"))
        {
            InitializeScale();
            this.gameObject.SetActive(false);
        }
    }
}
