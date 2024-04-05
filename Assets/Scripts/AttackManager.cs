using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    [SerializeField] 
    private float m_launchForceLight;
    [SerializeField] 
    private float m_launchForceBig;
    [SerializeField] 
    private GameObject m_bullet;
    [SerializeField] 
    private GameObject m_biggyBoi;
    
    
    
    public void Attack()
    {
        var bullet = Instantiate(m_bullet, transform.position, transform.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(transform.forward * m_launchForceLight, ForceMode.VelocityChange);
    }
    
    public void AttackAlt()
    {
        var biggyBoi = Instantiate(m_biggyBoi, transform.position, transform.rotation);
        biggyBoi.GetComponent<Rigidbody>().AddForce(transform.forward * m_launchForceBig, ForceMode.VelocityChange);
    }
}
