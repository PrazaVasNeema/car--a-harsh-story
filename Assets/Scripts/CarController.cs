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
        m_carActorInputManager.OnHandbreakAction += M_carActorInputManager_OnHandbreakAction; ;
    }

    public override void Deactivate()
    {
        base.Deactivate();
        m_carActorInputManager.OnHandbreakAction -= M_carActorInputManager_OnHandbreakAction; ;
    }

    protected override void Update()
    {
        base.Update();

        Vector2 inputVector = m_actorInputManager.GetMovementVectorNormalized();
        m_handleCarBehaviourComponent.Move(inputVector);
    }

    private void M_carActorInputManager_OnHandbreakAction(object sender, System.EventArgs e)
    {
        m_handleCarBehaviourComponent.Handbrake();
    }

}
