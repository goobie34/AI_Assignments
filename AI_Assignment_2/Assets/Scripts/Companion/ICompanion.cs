using UnityEngine;
public interface ICompanion
{
    bool HasPlayerGivenCommand();
    bool HasTargetBeenVisited();
    bool CanSenseOrbs();
    void GoToTarget();
    void FollowPlayer();
    void GoToOrb();
    bool SearchForOrbs();
    void ReturnToPlayer();

    void GiveCommand(Vector3 targetPoint);
}  