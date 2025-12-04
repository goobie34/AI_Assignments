using SimpleBehaviorTree;
using System.Collections.Generic;
using UnityEngine;

public class CompanionBT : TreeBT
{
    [SerializeField] CompanionScriptWrapper companionWrapper;
    ICompanion companion;
    private void Awake()
    {
        companion = companionWrapper.GetICompanion;
    }
    protected override TaskBT BuildTree()
    {
        return new SelectorBT(new List<TaskBT>()
        {
            new SequenceBT(new List<TaskBT>()
            {
                new InvertedConditionBT(companion.HasPlayerGivenCommand),
                //check player far away?
                //aciton timer???
                new SimpleActionBT(companion.FollowPlayer)
            }),
            new SelectorBT(new List<TaskBT>()
            {
                //new ConditionBT(companion.HasPlayerGivenCommand),
                new SequenceBT(new List<TaskBT>()
                    {
                        new ConditionBT(companion.CanSenseOrbs),
                        new SimpleActionBT(companion.GoToOrb)
                        //+ look around
                    }),
                new SelectorBT(new List<TaskBT>()
                    {
                        new SequenceBT(new List<TaskBT>()
                        {
                            new InvertedConditionBT(companion.HasTargetBeenVisited),
                            new SimpleActionBT(companion.GoToTarget)
                        })
                        //+ search (as fallback)
                    }),
                new SequenceBT(new List<TaskBT>()
                {
                    new SimpleActionBT(companion.ReturnToPlayer)
                    //+ deliver
                })
            }),
            
        });
    }
}
