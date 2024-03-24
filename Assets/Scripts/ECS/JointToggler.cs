using UnityEngine;
 
public class JointToggler : MonoBehaviour
{
    [SerializeField] private HingeJoint joint;
    private Rigidbody connectedBody;
 
    private void Awake()
    {
        joint = joint ? joint : GetComponent<HingeJoint>();
        if (joint) connectedBody = joint.connectedBody;
        else Debug.LogError("No joint found.", this);
    }
 
    private void OnEnable() { joint.connectedBody = connectedBody; }
 
    private void OnDisable()
    {
        joint.connectedBody = null;
        connectedBody.WakeUp();
    }
}