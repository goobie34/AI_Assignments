using UnityEngine;
public interface ICompanion
{
    bool HasPlayerGivenCommand();
    bool HasTargetBeenVisited();
    bool CanSenseOrbs();
    bool HasOrbs();
    bool HasSearched();
    bool GoToTarget();
    void FollowPlayer();
    bool DeliverOrbs();
    bool GoToOrb();
    void PickUpOrb();
    bool SearchForOrbs();
    bool ReturnToPlayer();
    bool LookAround();

    void GiveCommand(Vector3 targetPoint);
}  