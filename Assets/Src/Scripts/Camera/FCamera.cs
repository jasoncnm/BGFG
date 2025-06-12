using UnityEngine;

public class FCamera : GameCamera
{
    [SerializeField] Vector2 minPosition, maxPosition;

    [SerializeField] CameraController controller;
    
    [SerializeField] float smoothTime = 0.15f;

    [SerializeField] float yOffset, zOffset;

    Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        type = CameraType.FB;
    }
    
    public override void InitPosition(Transform target)
    {
        base.InitPosition(target);
        transform.position = new Vector3(Mathf.Clamp(target.position.x, minPosition.x, maxPosition.x),
                                         Mathf.Clamp(target.position.y, minPosition.y, maxPosition.y) + yOffset,
                                         target.position.z - zOffset);
    }

    public override void FollowTarget(Transform target)
    {
        Vector3 newPosition = new Vector3(Mathf.Clamp(target.position.x, minPosition.x, maxPosition.x),
                                          Mathf.Clamp(target.position.y, minPosition.y, maxPosition.y) + yOffset,
                                          target.position.z - zOffset);

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }


}
