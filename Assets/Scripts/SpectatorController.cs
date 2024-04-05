using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorController : PlayerActorControllerAbstract
{
    [SerializeField] private AttackManager m_attackManager;
    [SerializeField] private SpectatorMovementComponent m_spectatorMovementComponent;

    private GameInputManager.SpectatorActorInputManager m_spectatorActorInputManager;

    public override void Init(GameInputManager.ActorInputManagerAbstract actorInputManager)
    {
        base.Init(actorInputManager);
        m_spectatorActorInputManager = (GameInputManager.SpectatorActorInputManager)m_actorInputManager;

    }

    public override void Activate()
    {
        base.Activate();
        m_spectatorActorInputManager.OnFireAction += ActorInputManager_OnFireAction;
        m_spectatorActorInputManager.OnFireHeavyAction += ActorInputManager_OnFireHeavyAction;
        m_spectatorActorInputManager.OnFireSuperAction += ActorInputManager_OnFireSuperAction;
    }

    public override void Deactivate()
    {
        base.Deactivate();
        m_spectatorActorInputManager.OnFireAction -= ActorInputManager_OnFireAction;
        m_spectatorActorInputManager.OnFireHeavyAction -= ActorInputManager_OnFireHeavyAction;
        m_spectatorActorInputManager.OnFireSuperAction -= ActorInputManager_OnFireSuperAction;
    }

    protected override void Update()
    {
        base.Update();

        Vector2 inputVector = m_actorInputManager.GetMovementVectorNormalized();
        Vector3 inputCorrect = new Vector3(inputVector.x, 0f, inputVector.y);
        Vector2 inputVectorMouse = m_spectatorActorInputManager.GetMouseVec();

        float inputUpDown = m_spectatorActorInputManager.GetMovementUpDown();
        float inputSpeedSlow = m_spectatorActorInputManager.GetMovementSpeedSlow();

        m_spectatorMovementComponent.Move(inputVector, inputVectorMouse, inputUpDown, inputSpeedSlow);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void ActorInputManager_OnFireAction(object sender, System.EventArgs e)
    {
        if (m_attackManager != null)
        {
            m_attackManager.Attack();
        }
    }
    
    private void ActorInputManager_OnFireHeavyAction(object sender, System.EventArgs e)
    {
        if (m_attackManager != null)
        {
            m_attackManager.AttackAlt();
        }
    }
    
    private void ActorInputManager_OnFireSuperAction(object sender, System.EventArgs e)
    {
        if (m_attackManager != null)
        {
            m_attackManager.AttackSuper();
        }
    }
}
