using UnityEngine;
using static InputMapper;

public class PushEvents : MonoBehaviour
{
    private float thetaX = 0;
    private float thetaY = 0;
    [SerializeField]
    private float speed = 0.5f;
    [SerializeField]
    private bool right = false;
    private float thetaZ;

    public void Push(object info)
    {
        Debug.Log("Pushed " + right + (Vector3)info);
        Vector3 delta = (Vector3)info;
        thetaX -= delta.x * speed;
        thetaZ += delta.y * speed;
        thetaX = Mathf.Clamp(thetaX, -20, 20);
        thetaZ = Mathf.Clamp(thetaZ, -20, 20);
        transform.localRotation = Quaternion.Euler(thetaZ, 0, thetaX);
    }
    public void Start()
    {
        InputMapper mapper = FindObjectsByType<InputMapper>(FindObjectsSortMode.None)[0];
        if(right)
        {
            mapper.Register(Actions.PUSH_RIGHT, (DataEventHandler)this.Push);
        }
        else
        {
            mapper.Register(Actions.PUSH_LEFT, (DataEventHandler)this.Push);
        }
    }
}
