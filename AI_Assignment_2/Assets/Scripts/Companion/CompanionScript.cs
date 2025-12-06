using UnityEngine;

public class CompanionScript : MonoBehaviour, ICompanion
{
    //--- flags ---
    bool hasPlayerGivenComand = false;
    bool hasTargetBeenVisited = false;
    bool isSearching = false;
    bool hasSearched = false;

    //--- positions ---
    Vector3? closestOrb = null;
    Vector3? targetPoint = null;
    Vector3 currentMoveDir = Vector3.zero;

    [Header("References")]
    [SerializeField] GameObject player;
    [SerializeField] CharacterController controller;
    [SerializeField] Transform body;
    [SerializeField] OrbSensorScript sensorScript;
    [SerializeField] PickUpOrbScript orbPickupScript;
    [SerializeField] GameObject target;

    [Header("Movement")]
    [SerializeField] float speed;
    [SerializeField] float turnSpeed = 1f;

    [Header("Thresholds")]
    [SerializeField] float returnToPlayerThreshold;
    [SerializeField] float goToOrbThreshold;
    [SerializeField] float targetPointThreshold;

    [Header("Searching")]
    [SerializeField] float searchSpeed;
    [SerializeField] float maxSearchTime;
    float searchAngle;
    float searchTimer = 0f;

    [Header("Delivering Orbs")]
    [SerializeField] float deliverCooldown;
    [SerializeField] float deliverDistance;
    float deliverTimer;

    //--- Method called by player ---
    public void GiveCommand(Vector3 targetPosition)
    {
        hasPlayerGivenComand = true;
        hasTargetBeenVisited = false;
        targetPoint = targetPosition;

        target.transform.position = new Vector3(targetPosition.x, target.transform.position.y, targetPosition.z);
        target.SetActive(true);
    }

    //--- Methods called by behavior tree AI ---
    //Conditions
    public bool HasPlayerGivenCommand() => hasPlayerGivenComand;
    public bool HasTargetBeenVisited() => hasTargetBeenVisited;
    public bool CanSenseOrbs()
    {
        if (sensorScript != null)
        {
            closestOrb = sensorScript.GetClosestOrbPos(transform.position);
            return sensorScript.SensesOrbs;
        }

        closestOrb = null;
        return false;
    }
    public bool HasOrbs() => orbPickupScript.OrbCount > 0;
    public bool HasSearched() => hasSearched;

    //Completable actions
    public bool GoToTarget()
    {
        if (targetPoint == null) return false;

        if (CanSenseOrbs()) return true;

        Vector3 tempVec = (Vector3)targetPoint;
        tempVec.y = transform.position.y;

        if (Vector3.Distance(tempVec, transform.position) < targetPointThreshold)
        {
            targetPoint = null;
            hasTargetBeenVisited = true;
            target.SetActive(false);
            return true;
        }

        MoveTowards((Vector3)targetPoint);
        return false;
    }
    public bool GoToOrb()
    {
        if (closestOrb != null)
            MoveTowards((Vector3)closestOrb);

        if (Vector3.Distance(transform.position, (Vector3)closestOrb) < goToOrbThreshold)
        {
            return true;
        }
        return false;
    }
    public bool ReturnToPlayer()
    {
        if (CanSenseOrbs()) //interrupt this action if orbs are sensed nearby
        {
            return true;
        }

        if (Vector3.Distance(transform.position, player.transform.position) < deliverDistance)
        {
            hasPlayerGivenComand = false;
            return true;
        }

        MoveTowards(player.transform.position);

        return false;
    }
    public bool DeliverOrbs()
    {
        if (!HasOrbs()) return true;

        if (Vector3.Distance(transform.position, player.transform.position) < deliverDistance)
        {
            deliverTimer += Time.deltaTime;
            if (deliverTimer > deliverCooldown)
            {
                deliverTimer = 0;
                orbPickupScript.Remove();
                player.GetComponent<PickUpOrbScript>().Add();
            }
        }
        else
        {
            MoveTowards(player.transform.position);
        }

        return false;
    }
    public bool LookAround()
    {
        searchTimer += Time.deltaTime;
        searchAngle = searchSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, searchAngle);

        if (CanSenseOrbs()) return true;

        if (searchTimer > maxSearchTime)
        {
            searchTimer = 0;
            hasSearched = true;
            return true;
        }

        return false;
    }

    //Simple actions
    public void FollowPlayer()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > returnToPlayerThreshold)
            MoveTowards(player.transform.position);
    }
    public void PickUpOrb()
    {
        //trigger for animation, particle system or sound effect goes here
        hasSearched = false;
    }

    //--- Methods only for internal use ---

    //void MoveTowards(Vector3 targetPos)
    //{   
    //    //original, pre-gpt

    //    Vector3 moveDirectionWithSpeed = (targetPos - transform.position).normalized * speed;
    //    controller.Move(moveDirectionWithSpeed * Time.deltaTime);

    //    Vector3 moveDirectionWithSpeed_2d = new Vector3(moveDirectionWithSpeed.x, 0f, moveDirectionWithSpeed.z);

    //    if (moveDirectionWithSpeed_2d.sqrMagnitude > 0.001f)
    //    {
    //        body.rotation = Quaternion.LookRotation(moveDirectionWithSpeed_2d, Vector3.up);
    //    }
    //}

    void MoveTowards(Vector3 targetPos)
    {
        //this method was reworked with chatgpt to smooth rotation and movement with slerp

        Vector3 desiredDir = (targetPos - transform.position);
        desiredDir.y = 0f;

        if (desiredDir.sqrMagnitude > 0.001f)
        {
            desiredDir.Normalize();

            currentMoveDir = Vector3.Slerp(
                currentMoveDir,
                desiredDir,
                turnSpeed * Time.deltaTime
            );

            controller.Move(currentMoveDir * speed * Time.deltaTime);

            Quaternion targetRot = Quaternion.LookRotation(currentMoveDir, Vector3.up);
            body.rotation = Quaternion.Slerp(
                body.rotation,
                targetRot,
                turnSpeed * Time.deltaTime
            );
        }
        else
        {
            controller.Move(Vector3.zero);
        }
    }
}
