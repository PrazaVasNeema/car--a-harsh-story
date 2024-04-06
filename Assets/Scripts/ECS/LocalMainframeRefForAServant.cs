using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalMainframeRefForAServant : MonoBehaviour
{
    [SerializeField]
    private GameObject m_refForMainframe;

    public GameObject refForMainframe => m_refForMainframe;
}
