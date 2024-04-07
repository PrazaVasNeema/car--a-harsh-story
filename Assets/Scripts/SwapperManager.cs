using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapperManager : MonoBehaviour
{
    [SerializeField] 
    private List<GameObject> m_groupOne;
    [SerializeField] 
    private List<GameObject> m_groupTwo;

    [SerializeField] private bool m_swapStatus = false;
    public bool swapStatus => m_swapStatus;
    
    
    public void SwapIt()
    {
        m_swapStatus = !m_swapStatus;

        foreach (var groupMember in m_groupOne)
        {
            groupMember.SetActive(!m_swapStatus);
        }
        
        foreach (var groupMember in m_groupTwo)
        {
            groupMember.SetActive(m_swapStatus);
        }
       
    }
    
   
}
