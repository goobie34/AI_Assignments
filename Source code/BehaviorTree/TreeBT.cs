using UnityEngine;
using SimpleBehaviorTree;
public abstract class TreeBT : MonoBehaviour
{
    TaskBT root;
    void Start()
    {
        root = BuildTree();
    }

    void FixedUpdate()
    {
        root.Tick();
    }

    protected abstract TaskBT BuildTree();
    
}
