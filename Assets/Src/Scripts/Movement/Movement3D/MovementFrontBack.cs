using System;
using UnityEditor.Overlays;
using UnityEngine;



public class MovementFrontBack : IMoveMode
{


    #region UpdateMethods

    public void SimulateMovement(PlayerControlData controlData, PlayerMovementData movementData)
    {
        if (movementData._InWater)
        {
            WaterMovement(controlData, movementData);
            return;
        }
        MoveAndTurn(controlData, movementData);
        SimulateJump(controlData, movementData);
    }

    void WaterMovement(PlayerControlData controlData, PlayerMovementData movementData)
    {
        Vector2 direction = controlData.moveValue.normalized;
        float ratio = controlData.moveValue.magnitude;
        Debug.Assert(ratio >= 0f && ratio <= 1.001f, "Assertion Faile (ratio : " + ratio.ToString() + ")");

        movementData.speed = movementData.normalSpeed * ratio;

        movementData.forwardMovement = Vector3.zero;

        if (ratio > 0f)
        {
            Vector2 targetVelocity = direction * movementData.speed;
            Vector2 VelocityDiff = targetVelocity - new Vector2(movementData.rb.linearVelocity.x, movementData.rb.linearVelocity.y);

            float accelRate = movementData.acceleration;

            movementData.forwardMovement = new Vector3(Mathf.Pow(Mathf.Abs(VelocityDiff.x) * accelRate, movementData.velPower) * Mathf.Sign(VelocityDiff.x),
                                                       Mathf.Pow(Mathf.Abs(VelocityDiff.y) * accelRate, movementData.velPower) * Mathf.Sign(VelocityDiff.y),
                                                       0f);
        }
    }

    void MoveAndTurn(PlayerControlData controlData, PlayerMovementData movementData)
    { 
        float direction = Mathf.Sign(controlData.moveValue.x);
        float ratio = Mathf.Abs(controlData.moveValue.x);
        Debug.Assert(ratio >= 0f && ratio <= 1.001f, "Assertion Faile (ratio : " + ratio.ToString() + ")");

        float speed = movementData.normalSpeed * ratio;

        movementData.forwardMovement = Vector3.zero;

        if (ratio > 0f)
        {
            float targetVelocity = direction * speed;
            float VelocityDiff = targetVelocity - movementData.rb.linearVelocity.x;

            float accelRate = movementData.acceleration;

            movementData.forwardMovement = new Vector3(Mathf.Pow(Mathf.Abs(VelocityDiff) * accelRate, movementData.velPower) * Mathf.Sign(VelocityDiff), 0f, 0f);
        }

    }


    void SimulateJump(PlayerControlData controlData, PlayerMovementData movementData)
    {

        if (controlData._JumpButtonDown)
        {
            movementData.jumpBufferCounter = movementData.jumpBufferTime;
        }
        else
        {
            movementData.jumpBufferCounter -= Time.deltaTime;
        }

        if (movementData._Grounded)
        {
            movementData.coyoteTimeCounter = movementData.coyoteTime;
        }
        else
        {
            movementData.coyoteTimeCounter -= Time.deltaTime;
        }

        if ((movementData.jumpBufferCounter > 0f) && (movementData.coyoteTimeCounter > 0f))
        {
            movementData._Jump = true;
        }
        else if (controlData._JumpButtonCancelled&& movementData.rb.linearVelocity.y > 0f)
        {
            movementData._JumpReleased = true;
        }
    }


    public void UpdateStart(PlayerControlData controlData, PlayerMovementData movementData)
    {
        Physics.gravity = Vector3.down * movementData.gravityScale;
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
    }

    public void FixedUpdateEnd(PlayerControlData controlData, PlayerMovementData movementData)
    {
        movementData._Grounded          = false;
        movementData._Jump              = false;
        movementData._JumpReleased      = false;
    }

    public void MoveRigidBody(PlayerControlData controlData, PlayerMovementData movementData)
    {
        if (movementData._InWater)
        {
            MoveInWater(controlData, movementData);
            return;
        }

        Move(controlData, movementData);
        Fall(controlData, movementData);
        
        if (movementData._InWind)
        {
            WindBlow(controlData, movementData);
            return;
        }

        Jump(controlData, movementData);
    }

    void WindBlow(PlayerControlData controlData, PlayerMovementData movementData)
    {
        Util.Direction dir = movementData.windDir;
        float speed = movementData.windSpeed;

        Vector3 windVel = Vector3.zero;

        switch (dir)
        {
            case Util.Direction.Up:
                windVel = Vector3.up * speed;
                break;
            case Util.Direction.Down:
                windVel = Vector3.down * speed;
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

    void MoveInWater(PlayerControlData controlData, PlayerMovementData movementData)
    {
        Rigidbody rb = movementData.rb;

        rb.AddForce(movementData.forwardMovement * movementData.worldScale);

        Vector2 hVel = new Vector2(movementData.rb.linearVelocity.x, rb.linearVelocity.y);
        if (hVel.magnitude > movementData.maxSpeed)
        {
            hVel = hVel.normalized * movementData.maxSpeed;
            rb.linearVelocity = new Vector3(hVel.x, rb.linearVelocity.y, hVel.y);
        }

        if (movementData.forwardMovement.magnitude < 0.01f)
        {
            Vector2 vi = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);

            Vector2 forwardDir = vi.normalized;
            Vector2 reverseDir = -forwardDir;

            Vector2 frictionalForce = movementData.groundFrictionalForce * reverseDir;

            Vector2 a = frictionalForce / rb.mass;

            Vector2 vf = vi + a * Time.deltaTime;


            if (Mathf.Sign(vf.x) != Mathf.Sign(vi.x) || (Mathf.Abs(vi.x) < 0.01f)) vf.x = 0f;
            if (Mathf.Sign(vf.y) != Mathf.Sign(vi.y) || (Mathf.Abs(vi.y) < 0.01f)) vf.y = 0f;

            rb.linearVelocity = new Vector3(vf.x, vf.y, 0f);

        }
    }

    void Jump(PlayerControlData controlData, PlayerMovementData movementData)
    {
        if (movementData._Jump)
        {
            movementData.rb.linearVelocity = new Vector3(movementData.rb.linearVelocity.x, 0f, 0f);
            movementData.jumpBufferCounter = 0f;
            movementData.coyoteTimeCounter = 0f;
            movementData.rb.AddForce(Vector3.up * movementData.jump * movementData.worldScale, ForceMode.Impulse);
        }

        if (movementData._JumpReleased && movementData._HoldJump)
        {
            movementData.rb.linearVelocity = Vector3.Scale(movementData.rb.linearVelocity, new Vector3(1f, 0.5f, 0f));
        }

    }

    void Fall(PlayerControlData controlData, PlayerMovementData movementData)
    {
        // Custom gravity at different falling stages assuming downwards gravity
        if (movementData.rb.linearVelocity.y < -1 * movementData.maxFallSpeed)
        {
            movementData.rb.linearVelocity = new Vector3(movementData.rb.linearVelocity.x, -1.0f * movementData.maxFallSpeed);
        }
        else if (movementData.rb.linearVelocity.y < 0)
        {
            movementData.rb.linearVelocity += Vector3.up * Physics.gravity.y * (movementData.fallMultiplier - 1) * Time.deltaTime;
        }
        else if (movementData.rb.linearVelocity.y > 0)
        {
            movementData.rb.linearVelocity += Vector3.up * Physics.gravity.y * (movementData.lowJumpMultiplier - 1) * Time.deltaTime;
        }
        else
        {
            movementData.rb.linearVelocity += Vector3.up * Physics.gravity.y * Time.deltaTime;
        }
    }

    void Move(PlayerControlData controlData, PlayerMovementData movementData)
    {
        Rigidbody rb = movementData.rb;
        if (!movementData._SlopeStop)
        {
            rb.AddForce(new Vector3(movementData.forwardMovement.x, 0f, 0f) * movementData.worldScale);
        }

        if (movementData._SlopeStop)
        {
            Debug.Log("WALLL");
        }
        
        float hVel = movementData.rb.linearVelocity.x;
        if (Mathf.Abs(hVel) > movementData.maxSpeed)
        {
            hVel = Mathf.Sign(hVel) * movementData.maxSpeed;
            rb.linearVelocity = new Vector3(hVel, movementData.rb.linearVelocity.y, 0);
        }


        float frictionForce;
        if (Mathf.Abs(movementData.forwardMovement.x) < 0.01f)
        {

            float vi = movementData.rb.linearVelocity.x;

            float forwardDir = Mathf.Sign(vi);
            float reverseDir = -forwardDir;

            if ((movementData.coyoteTimeCounter > 0f))
            {
                frictionForce = movementData.groundFrictionalForce;
            }
            else 
            {
                frictionForce = movementData.airFrictionalForce;
            }

            frictionForce *= reverseDir;

            float a = frictionForce / rb.mass;
            float vf = vi + a * Time.deltaTime;

            if (Mathf.Sign(vf) != Mathf.Sign(vi) || (Mathf.Abs(vi) < 0.01f)) vf = 0f;

            Debug.Log("InitVel: " + vi + " FinalVel: " + vf);

            rb.linearVelocity = new Vector3(vf, rb.linearVelocity.y, 0);
        }
    }

#endregion

    #region Public Methods

    public void ResetState(PlayerControlData controlData, PlayerMovementData movementData)
    {
        //transform.position = new Vector3(transform.position.x, transform.position.y, orgZ);
        Physics.gravity = Vector3.down * movementData.gravityScale;
        movementData.rb.linearVelocity = Vector3.zero;
    }

    #endregion

}
