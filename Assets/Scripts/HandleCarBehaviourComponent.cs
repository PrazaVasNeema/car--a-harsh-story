using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleCarBehaviourComponent : MonoBehaviour
{
    [SerializeField]
    private PrometeoCarController m_prometeoCarController;

    private bool m_handbreakStatus;

    public void Move(Vector2 moveVector)
    {
        //{
        //Debug.Log(moveVector);
        if (moveVector.y == 1)
        {
            m_prometeoCarController.CancelInvoke("DecelerateCar");
            m_prometeoCarController.SetDeceleratingCar(false);
            m_prometeoCarController.GoForward();
        }
        if (moveVector.y == -1)
        {
            m_prometeoCarController.CancelInvoke("DecelerateCar");
            m_prometeoCarController.SetDeceleratingCar(false);
            m_prometeoCarController.GoReverse();
        }
        if (moveVector.x == -1)
        {
            m_prometeoCarController.TurnLeft();
        }
        if (moveVector.x == 1)
        {
            m_prometeoCarController.TurnRight();
        }

        if (m_handbreakStatus)
        {
            m_prometeoCarController.CancelInvoke("DecelerateCar");
            m_prometeoCarController.SetDeceleratingCar(false);
            m_prometeoCarController.Handbrake();
        }
        if (!m_handbreakStatus)
        {

            m_prometeoCarController.RecoverTraction();
        }
        if ((!(moveVector.y == -1) && !(moveVector.y == 1)))
        {
            m_prometeoCarController.ThrottleOff();
        }
        if ((!(moveVector.y == -1) && !(moveVector.y == 1)) && !m_handbreakStatus && !m_prometeoCarController.GetDeceleratingCar())
        {
            m_prometeoCarController.InvokeRepeating("DecelerateCar", 0f, 0.1f);
            m_prometeoCarController.SetDeceleratingCar(true);
        }
        if (!(moveVector.x == -1) && !(moveVector.x == 1) && m_prometeoCarController.GetSteeringAxis() != 0f)
        {
            m_prometeoCarController.ResetSteeringAngle();
        }

        if(moveVector.y == -1 || m_handbreakStatus)
        {
            m_prometeoCarController.SetAcceleratingBackwards(true);
        }
        else
        {
            m_prometeoCarController.SetAcceleratingBackwards(false);
        }
    }

    public void SetHandbrakeStatus(bool handbreakStatus)
    {
        m_handbreakStatus = handbreakStatus;
        Debug.Log(handbreakStatus);
    }
}
