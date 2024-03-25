using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float m_bulletLifespan;

    private float m_currentBulletBirthTIme;
    // Start is called before the first frame update
    void Start()
    {
        // m_currentBulletBirthTIme = 0;
    }

    private void OnEnable()
    {
        m_currentBulletBirthTIme = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // m_currentBulletBirthTIme = Time.time;
        if (Time.time - m_currentBulletBirthTIme > m_bulletLifespan)
        {
            Destroy(transform.gameObject);
        }
    }
}
