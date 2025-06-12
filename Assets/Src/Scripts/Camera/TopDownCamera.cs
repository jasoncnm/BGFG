using UnityEngine;

public class TopDownCamera : GameCamera
{
    [SerializeField] Vector2 minPosition, maxPosition;

    [SerializeField] float smoothTime = 0.15f;

    [SerializeField] float yOffset;

    Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        type = CameraType.TD;
    }

    public override void InitPosition(Transform target)
    {
        transform.position = new Vector3(Mathf.Clamp(target.position.x, minPosition.x, maxPosition.x),
                                          target.position.y + yOffset,
                                          Mathf.Clamp(target.position.z, minPosition.y, maxPosition.y));
    }

    public override void FollowTarget(Transform target)
    {

        Vector3 newPosition = new Vector3(Mathf.Clamp(target.position.x, minPosition.x, maxPosition.x),
                                          target.position.y + yOffset,
                                          Mathf.Clamp(target.position.z, minPosition.y, maxPosition.y));

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

}
