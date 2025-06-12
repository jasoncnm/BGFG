
using UnityEngine;

public class MovementTopDown : IMoveMode
{

    #region UpdateMethods
    public void SimulateMovement(PlayerControlData controlData, PlayerMovementData movementData)
    {
        Vector2 direction = controlData.moveValue.normalized;
        float ratio = controlData.moveValue.magnitude;
        Debug.Assert(ratio >= 0f && ratio <= 1.001f, "Assertion Faile (ratio : " + ratio.ToString() + ")");

        movementData.speed = movementData.normalSpeed * ratio;

        movementData.forwardMovement = Vector3.zero;

        if (ratio > 0f)
        {
            Vector2 targetVelocity = direction * movementData.speed;
            Vector2 VelocityDiff = targetVelocity - new Vector2(movementData.rb.linearVelocity.x, movementData.rb.linearVelocity.z);

            float accelRate = movementData.acceleration;

            movementData.forwardMovement = new Vector3(Mathf.Pow(Mathf.Abs(VelocityDiff.x) * accelRate, movementData.velPower) * Mathf.Sign(VelocityDiff.x), 
                                                       0f,
                                                       Mathf.Pow(Mathf.Abs(VelocityDiff.y) * accelRate, movementData.velPower) * Mathf.Sign(VelocityDiff.y));
        }

    }


    public void UpdateStart(PlayerControlData controlData, PlayerMovementData movementData)
    {
    }

    public void UpdateEnd(PlayerControlData controlData, PlayerMovementData movementData)
    {
        controlData._JumpButtonCancelled = false;
        controlData._JumpButtonDown = false;
    }

    #endregion


    #region FixUpdateMethods


    public void FixedUpdateStart(PlayerControlData controlData, PlayerMovementData movementData)
    {
        Transform tr = movementData.rb.transform;
        Collider collider = tr.GetComponent<Collider>();
        if (Physics.BoxCast(collider.bounds.center, tr.localScale * 0.5f, Vector3.down, out RaycastHit hit))
        {
            if (hit.transform.CompareTag("Respawn"))
            {
                tr.GetComponent<NRespawn>().OnHitObject();
            }
        }
    }

    public void FixedUpdateEnd(PlayerControlData controlData, PlayerMovementData movementData)
    {
    }


    public void MoveRigidBody(PlayerControlData controlData, PlayerMovementData movementData)
    {
        Move(controlData, movementData);
        if (movementData._InWind)
        {
            WindBlow(controlData, movementData);
        }
    }

    void WindBlow(PlayerControlData controlData, PlayerMovementData movementData)
    {
        Util.Direction dir = movementData.windDir;
        float speed = movementData.windSpeed;

        Vector3 windVel = Vector3.zero;

        switch (dir)
        {
            case Util.Direction.Front:
                windVel = Vector3.forward * speed;
                break;
            case Util.Direction.Back:
                windVel = Vector3.back * speed;
                break;
            case Util.Direction.Left:
                windVel = Vector3.left * speed;
                break;
            case Util.Direction.Right:
                windVel = Vector3.right * speed;
                break;
        }

        movementData.rb.AddForce(windVel * movementData.worldScale);
    }

    void Move(PlayerControlData controlData, PlayerMovementData movementData)
    {
        Rigidbody rb = movementData.rb;

        rb.AddForce(movementData.forwardMovement * movementData.worldScale);

        Vector2 hVel = new Vector2(movementData.rb.linearVelocity.x, rb.linearVelocity.z);
        if (hVel.magnitude > movementData.maxSpeed)
        {
            hVel = hVel.normalized * movementData.maxSpeed;
            rb.linearVelocity = new Vector3(hVel.x, rb.linearVelocity.y, hVel.y);
        }

        if (movementData.forwardMovement.magnitude < 0.01f)
        {
            Vector2 vi = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z);

            Vector2 forwardDir = vi.normalized;
            Vector2 reverseDir = -forwardDir;

            Vector2 frictionalForce = movementData.groundFrictionalForce * reverseDir;

            Vector2 a = frictionalForce / rb.mass;

            Vector2 vf = vi + a * Time.deltaTime;

            if (Mathf.Sign(vf.x) != Mathf.Sign(vi.x) || (Mathf.Abs(vi.x) < 0.01f)) vf.x = 0f;
            if (Mathf.Sign(vf.y) != Mathf.Sign(vi.y) || (Mathf.Abs(vi.y) < 0.01f)) vf.y = 0f;

            rb.linearVelocity = new Vector3(vf.x, 0f, vf.y);

        }
    }

    #endregion

    #region Public Methods

    public void ResetState(PlayerControlData controlData, PlayerMovementData movementData)
    { 
        Physics.gravity = Vector3.zero;
        // rb.linearVelocity = Vector3.zero;
        //transform.position = new Vector3(transform.position.x, orgY + 0.1f, transform.position.z);
    }

    #endregion
}


