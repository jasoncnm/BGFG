using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public enum CameraType
    {
        FB, TD
    }

    public CameraType type { get; set; }


    private void Awake()
    {
    }

    public virtual void InitPosition(Transform target)
    {

    }

    public virtual void FollowTarget(Transform target)
    {
    }

}
