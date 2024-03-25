using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [SerializeField] 
    private float m_launchForce;
    [SerializeField] 
    private GameObject m_bullet;
    
    
    
    public void Attack()
    {
        var bullet = Instantiate(m_bullet, transform.position, transform.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(transform.forward * m_launchForce);
    }
}
