
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerControlData
{
    public Vector2 moveValue;
    public bool _JumpButtonDown;
    public bool _JumpButtonCancelled;
}

public class PlayerMovementData
{
    // Player Model Data
    public Rigidbody rb;

    // Player Movement Data
    public float gravityScale;
    public float jumpBufferTime, coyoteTime;

    public float normalSpeed, maxSpeed, acceleration, groundFrictionalForce;

    public float jump, airFrictionalForce;

    public float fallMultiplier, lowJumpMultiplier, maxFallSpeed;


    public float velPower;
    public float speed;
    public float worldScale;
    public float windSpeed;

    public Vector3 forwardMovement;

    public bool _Grounded;

    public bool _JumpReleased;
    public bool _SlopeStop;
    public bool _OnSlope;
    public bool _Jump;
    public bool _HoldJump;
    public bool _InWater;
    public bool _InWind;

    public float jumpBufferCounter;
    public float coyoteTimeCounter;

    public Util.Direction windDir;

}

public class Movement : MonoBehaviour
{
    #region Variables


    public InputReader input;

    public IMoveMode moveMode;

    [SerializeField, Range(1f, 100f)]  float gravityScale = 10f;
    [SerializeField, Range(0f, 1f)]  float jumpBufferTime = 0.2f, coyoteTime = 0.2f, worldScale = 1f;

    [SerializeField]  float normalSpeed, maxSpeed, acceleration, groundFrictionalForce, airFrictionalForce;

    [SerializeField]  float jump;

    [SerializeField]  float fallMultiplier, lowJumpMultiplier, maxFallSpeed;

    public PlayerControlData controlData;
    public PlayerMovementData moveData;

    MovementFrontBack frontBackMode;
    MovementTopDown topDownMode;

    #endregion

    #region InputEvents
    private void OnEnable()
    {

        input.jumpEvent += OnJump;
        input.jumpCancelledEvent += OnJumpCancelled;
        input.moveEvent += OnMove;

    }

    private void OnDisable()
    {
        input.moveEvent -= OnMove;
        input.jumpEvent -= OnJump;
        input.jumpCancelledEvent -= OnJumpCancelled;
    }
    void OnJump()
    {
        controlData._JumpButtonDown = true;
        controlData._JumpButtonCancelled = false;
    }

    void OnJumpCancelled()
    {
        controlData._JumpButtonCancelled = true;
        controlData._JumpButtonDown = false;
    }

    void OnMove(Vector2 movement)
    {
        controlData.moveValue = movement;
    }



    #endregion

    #region Debug

    void DebugCode()
    {
        // debugPlayerRenderer.material.SetColor("_BaseColor", _AniGrounded ? Color.white : Color.black);
        // Debug.Log("HoldJump: " + _HoldJump);
        // Debug.Log(_AniGrounded);
        // Debug.Log(_Jumping);
    }

    private void OnDrawGizmos()
    {
        // if (rb != null) Gizmos.DrawLine(rb.position, Vector3.down * probeDistance);
    }
    #endregion

    #region Init Code


    private void OnValidate()
    {
        if (moveData != null)
        {

            moveData.worldScale = worldScale;
            moveData.gravityScale = gravityScale;
            moveData.jumpBufferTime = jumpBufferTime;
            moveData.coyoteTime = coyoteTime;

            moveData.normalSpeed = normalSpeed;
            moveData.maxSpeed = maxSpeed;
            moveData.acceleration = acceleration;
            moveData.groundFrictionalForce = groundFrictionalForce;

            moveData.jump = jump;
            moveData.airFrictionalForce = airFrictionalForce;

            moveData.fallMultiplier = fallMultiplier;
            moveData.lowJumpMultiplier = lowJumpMultiplier;
            moveData.maxFallSpeed = maxFallSpeed;
        }
    }

    void Awake()
    {
        frontBackMode = new MovementFrontBack();
        topDownMode = new MovementTopDown();

        controlData = new PlayerControlData();
        moveData = new PlayerMovementData();

        controlData.moveValue = Vector2.zero;
        controlData._JumpButtonDown = false;
        controlData._JumpButtonCancelled = false;

        moveData.rb = gameObject.GetComponent<Rigidbody>();
        moveData.rb.useGravity = false;
        // Player Movement Data

        OnValidate();

        moveData.velPower = 0.7f;
        moveData.speed = normalSpeed;

        moveData.forwardMovement = Vector3.zero;

        moveData._Grounded = false;

        moveData._JumpReleased = false;
        moveData._SlopeStop = false;
        moveData._OnSlope = false;
        moveData._Jump = false;
        moveData._HoldJump = true;
        moveData._InWater = false;
        moveData._InWind = false;

        moveData.jumpBufferCounter = 0f;
        moveData.coyoteTimeCounter = 0f;

        Physics.gravity = Vector3.down * moveData.gravityScale;

    }

    #endregion

    #region private helper functions

    #endregion

    #region public methods

    public void SetWaterMovement(bool enable)
    {
        moveData._InWater = enable;
    }

    public void SetWindMovement(bool enable, Util.Direction dir, float windSpeed)
    {
        moveData._InWind = enable;
        moveData.windSpeed = windSpeed;
        moveData.windDir = dir;
    }

    public void SetMovement(Util.Perspective perspective)
    {
        switch(perspective)
        {
            case Util.Perspective.FB:
                moveMode = frontBackMode;
                moveData.rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
                break;
            case Util.Perspective.TD:
                moveMode = topDownMode;
                moveData.rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
                break;

            case Util.Perspective.None:
                Assert.IsTrue(false, "ERROR: No Perspective is Given to Movement Script!!!");
                break;
        }
    }

    public void FreezeMovement()
    {
        moveData.rb.linearVelocity = Vector3.zero;
        moveData.rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    #endregion

    #region Update and FixedUpdate
    public void OnUpdate()
    {
        moveMode.UpdateStart(controlData, moveData);

        moveMode.SimulateMovement(controlData, moveData);
        
        moveMode.UpdateEnd(controlData, moveData);
    }

    public void OnFixedUpdate()
    {
        moveMode.FixedUpdateStart(controlData, moveData);

        moveMode.MoveRigidBody(controlData, moveData);

        moveMode.FixedUpdateEnd(controlData, moveData);
    }

    #endregion

    #region Collisions
    void EvaluateColllision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= 0.9f)
            {
                moveData._Grounded = true;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        EvaluateColllision(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        EvaluateColllision(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        moveData._Grounded = false;
    }


    #endregion


}
