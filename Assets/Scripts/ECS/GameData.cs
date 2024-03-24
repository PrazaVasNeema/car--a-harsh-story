using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scellecs.Morpeh;


public class GameData : MonoBehaviour
{
    public static GameData instance { get; private set; }

    [SerializeField] private DamageSystemSettingSO m_damageSystemSetting;
    public DamageSystemSettingSO damageSystemSetting => m_damageSystemSetting;

    public World currentWorld;
    
    
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("instance not null");
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        Cursor.visible = false;
    }
}