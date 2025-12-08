using UnityEngine;
public interface ICompanion
{
    //--- Called by player ---
    void GiveCommand(Vector3 targetPoint);

    //--- Called by behavior tree ---
    //Conditions
    bool HasPlayerGivenCommand();
    bool HasTargetBeenVisited();
    bool CanSenseOrbs();
    bool HasOrbs();
    bool HasSearched();

    //Completable actions
    bool GoToTarget();
    bool GoToOrb();
    bool ReturnToPlayer();
    bool DeliverOrbs();
    bool SearchForOrbs();

    //Simple actions
    void FollowPlayer();
    void PickUpOrb();
}  