using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private List<Transform> m_virtualCamerasList;

    public void ChangeActiveVirtualCamera(int index, bool shouldStickToBrother = false)
    {
        for (int i = 0; i< m_virtualCamerasList.Count; ++i)
        {
            if (i==index)
            {
                m_virtualCamerasList[i].gameObject.SetActive(true);
            }
            else
            {
                m_virtualCamerasList[i].gameObject.SetActive(false);

            }
        }

        if (shouldStickToBrother && index == 1) 
        {
            m_virtualCamerasList[1].position = m_virtualCamerasList[0].position;
            m_virtualCamerasList[1].rotation = m_virtualCamerasList[0].rotation;
        }
    }
}
