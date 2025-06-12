using UnityEngine;

public interface IMoveMode
{
    public void SimulateMovement(PlayerControlData controlData, PlayerMovementData movementData);
    public void UpdateStart(PlayerControlData controlData, PlayerMovementData movementData);

    public void UpdateEnd(PlayerControlData controlData, PlayerMovementData movementData);

    public void FixedUpdateEnd(PlayerControlData controlData, PlayerMovementData movementData);

    public void FixedUpdateStart(PlayerControlData controlData, PlayerMovementData movementData);

    public void MoveRigidBody(PlayerControlData controlData, PlayerMovementData movementData);

    public void ResetState(PlayerControlData controlData, PlayerMovementData movementData);

}
