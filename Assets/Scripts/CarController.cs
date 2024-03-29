using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameInputManager;

public class CarController : PlayerActorControllerAbstract
{
    [SerializeField] private HandleCarBehaviourComponent m_handleCarBehaviourComponent;

    private GameInputManager.CarActorInputManager m_carActorInputManager;

    public override void Init(GameInputManager.ActorInputManagerAbstract actorInputManager)
    {
        base.Init(actorInputManager);
        m_carActorInputManager = (GameInputManager.CarActorInputManager)m_actorInputManager;

    }

    public override void Activate()
    {
        base.Activate();
        m_carActorInputManager.OnHandbreakAction += M_carActorInputManager_OnHandbreakAction;
        m_carActorInputManager.OnHandbreakActionCanceled += M_carActorInputManager_OnHandbreakActionPerformed;
    }



    public override void Deactivate()
    {
        base.Deactivate();
        m_carActorInputManager.OnHandbreakAction -= M_carActorInputManager_OnHandbreakAction; ;
        m_carActorInputManager.OnHandbreakActionCanceled -= M_carActorInputManager_OnHandbreakActionPerformed;
    }

    protected override void Update()
    {
        base.Update();

        Vector2 inputVector = m_actorInputManager.GetMovementVector();
        m_handleCarBehaviourComponent.Move(inputVector);
    }

    private void M_carActorInputManager_OnHandbreakAction(object sender, System.EventArgs e)
    {
        m_handleCarBehaviourComponent.SetHandbrakeStatus(true);
    }
    private void M_carActorInputManager_OnHandbreakActionPerformed(object sender, System.EventArgs e)
    {
        m_handleCarBehaviourComponent.SetHandbrakeStatus(false);
    }
}
