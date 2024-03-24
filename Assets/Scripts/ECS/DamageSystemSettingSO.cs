using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageSystemSettings", menuName = "DamageProject/DamageSystemSettings")]
public class DamageSystemSettingSO : ScriptableObject
{
    [SerializeField] private Data m_data;
    public Data data => m_data;

    [System.Serializable]
    public class Data
    {
        public float spawnDecalsHPThreshold = 10;
        public float impactForceModifier;
    }
}
