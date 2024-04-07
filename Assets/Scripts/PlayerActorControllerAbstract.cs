using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActorControllerAbstract : MonoBehaviour
{

    protected GameInputManager.ActorInputManagerAbstract m_actorInputManager;


    public virtual void Init(GameInputManager.ActorInputManagerAbstract actorInputManager)
    {
        m_actorInputManager = actorInputManager;


    }
    protected virtual void Update()
    {
        Vector2 inputVector = m_actorInputManager.GetMovementVectorNormalized();
        Vector3 movementVector = new Vector3(inputVector.x, 0f, inputVector.y);
    }

    public virtual void Activate()
    {
        m_actorInputManager.OnChangeModeAction += M_actorInputManager_OnChangeModeAction; ;
        m_actorInputManager.OnChangeDayTimeAction += M_actorInputManager_OnChangeDaytimeAction; ;
        this.enabled = true;
    }

    public virtual void Deactivate()
    {
        m_actorInputManager.OnChangeModeAction -= M_actorInputManager_OnChangeModeAction; ;
        m_actorInputManager.OnChangeDayTimeAction -= M_actorInputManager_OnChangeDaytimeAction; ;
        this.enabled = false;
    }

    private void M_actorInputManager_OnChangeModeAction(object sender, System.EventArgs e)
    {
        GameEvents.OnChangeMode?.Invoke();
    }
    
    private void M_actorInputManager_OnChangeDaytimeAction(object sender, System.EventArgs e)
    {
        GameEvents.OnChangeDaytime?.Invoke();
    }


}
