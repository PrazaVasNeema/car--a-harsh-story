using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSizeOverTime : MonoBehaviour
{
    [SerializeField] private bool m_shouldChangeAnother;
    [SerializeField] private Transform m_anotherTransform;
    [SerializeField]
    private float m_changeScale = 5f;

    [SerializeField] private Vector3 m_AxisScale = Vector3.one;
    
    [SerializeField] private float m_speedScale = 1;

    private Vector3 m_initialScale;
    private Transform targetTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        targetTransform = m_shouldChangeAnother ? m_anotherTransform : transform;

        m_initialScale = targetTransform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        targetTransform.localScale = Vector3.one;
        var newScale =  m_changeScale * Mathf.Cos(Time.time * m_speedScale) + m_changeScale * 2;
        var lossyScale = targetTransform.lossyScale / 1;
        var newActualScale = new Vector3(m_AxisScale.x == 0 ? 1 : newScale * m_AxisScale.x / lossyScale.x,
            m_AxisScale.y == 0 ? 1 : newScale * m_AxisScale.y / lossyScale.y,
            m_AxisScale.z == 0 ? 1 : newScale * m_AxisScale.z / lossyScale.z);
        targetTransform.localScale = Vector3.Scale(m_initialScale, newActualScale);
    }
}
