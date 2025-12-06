using System.Collections.Generic;
using UnityEngine;
using SimpleBehaviorTree;

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
                new ConditionBT(companion.HasPlayerGivenCommand),
                new SelectorBT(new List<TaskBT>()
                {
                    new SequenceBT(new List<TaskBT>()
                    {
                        new ConditionBT(companion.CanSenseOrbs),
                        new CompleteableActionBT(companion.GoToOrb),
                        new SimpleActionBT(companion.PickUpOrb)
                    }),
                    new SequenceBT(new List<TaskBT>()
                    {
                        new InvertedConditionBT(companion.HasTargetBeenVisited),
                        new CompleteableActionBT(companion.GoToTarget)
                    }),
                    new SequenceBT(new List<TaskBT>()
                    {
                        new InvertedConditionBT(companion.HasSearched),
                        new CompleteableActionBT(companion.LookAround)
                    }),
                    new CompleteableActionBT(companion.ReturnToPlayer)
                })
            }),
            new SelectorBT(new List<TaskBT>()
            {
                new SequenceBT(new List<TaskBT>()
                {
                    new ConditionBT(companion.HasOrbs),
                    new CompleteableActionBT(companion.DeliverOrbs)
                }),
                new SimpleActionBT(companion.FollowPlayer)
            })
        });
    }
}
