using SimpleBehaviorTree;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CompanionBT : TreeBT
{
    [SerializeField] CompanionScriptWrapper companionWrapper;
    [SerializeField] bool debugBool;

    [SerializeField] float debugTimer = 0;
    [SerializeField] float debugTimerCap = 5f;


    ICompanion companion;
    private void Awake()
    {
        companion = companionWrapper.GetICompanion;
    }
    protected override TaskBT BuildTree()
    {
        //return new SelectorBT(new List<TaskBT>()
        //{
        //    new SequenceBT(new List<TaskBT>()
        //    {
        //        new InvertedConditionBT(companion.HasPlayerGivenCommand),
        //        //check player far away?
        //        //aciton timer???
        //        new SimpleActionBT(companion.FollowPlayer)
        //    }),
        //    new SelectorBT(new List<TaskBT>()
        //    {
        //        //new ConditionBT(companion.HasPlayerGivenCommand),
        //        new SequenceBT(new List<TaskBT>()
        //            {
        //                new ConditionBT(companion.CanSenseOrbs),
        //                new SimpleActionBT(companion.GoToOrb),
        //                new SimpleActionBT(companion.LookAround)

        //            }),
        //        new SelectorBT(new List<TaskBT>()
        //            {
        //                new SequenceBT(new List<TaskBT>()
        //                {
        //                    new InvertedConditionBT(companion.HasTargetBeenVisited),
        //                    new SimpleActionBT(companion.GoToTarget)
        //                })
        //                //+ search (as fallback)
        //            }),
        //        new SequenceBT(new List<TaskBT>()
        //        {
        //            new SimpleActionBT(companion.ReturnToPlayer)
        //            //+ deliver
        //        })
        //    }),

        //});

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

    private Action DebugMsgAction(string msg) { return () => { Debug.Log(msg); }; }
    private Func<bool> DebugBoolCheck() { return () => { return debugBool; }; }
    private Func<bool> DebugCompletableAction() {
        return () => {
            debugTimer += Time.deltaTime;
            if (debugTimer > debugTimerCap)
            {
                debugTimer = 0;
                return true;
            } else
                return false;
        };
    }
}
