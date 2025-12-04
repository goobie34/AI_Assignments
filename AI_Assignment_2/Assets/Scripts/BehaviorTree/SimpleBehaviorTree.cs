using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

namespace SimpleBehaviorTree
{
    public abstract class TaskBT
    {
        public enum TaskStatus { Running, Success, Failure};
        public TaskStatus Status { get; protected set; }
        public abstract TaskStatus Tick();
    }
    /// <summary>
    /// Simple action that can run indefinately.
    /// </summary>
    public class SimpleActionBT : TaskBT
    {
        Action behavior;

        public SimpleActionBT(Action behavior) { this.behavior = behavior; }
        public override TaskStatus Tick()
        {
            if (behavior == null)
                return TaskStatus.Failure;

            behavior();
            return TaskStatus.Running;
        }
    }
    /// <summary>
    /// Action that can be completed. The passed in delegate behavior must have return type bool. It should return true when the action is completed.
    /// </summary>
    public class CompleteableActionBT : TaskBT
    {
        Func<bool> behavior;

        public CompleteableActionBT(Func<bool> behavior) { this.behavior = behavior; }
        public override TaskStatus Tick()
        {
            if (behavior == null)
                return TaskStatus.Failure;

            //if behavior completes in this tick, status for this action becomes success, otherwise it is running
            Status = behavior() ? TaskStatus.Success : TaskStatus.Running;
            
            return TaskStatus.Running;
        }
    }

    public class ConditionBT : TaskBT
    {
        protected Func<bool> condition;
        public ConditionBT(Func<bool> condition) { this.condition = condition; }

        public override TaskStatus Tick()
        {
            Status = condition() ? TaskStatus.Success : TaskStatus.Failure;
            return Status;
        }
    }
    public class InvertedConditionBT : ConditionBT
    {
        public InvertedConditionBT(Func<bool> condition) : base(condition) {}

        public override TaskStatus Tick()   
        {
            Status = condition() ? TaskStatus.Failure : TaskStatus.Success;
            return Status;
        }
    }

    public abstract class CompositeTaskBT : TaskBT
    {
        protected List<TaskBT> childNodes;
        public CompositeTaskBT(List<TaskBT> childNodes) { this.childNodes = childNodes; }
    }

    public class SequenceBT : CompositeTaskBT
    {
        public SequenceBT(List<TaskBT> childNodes) : base(childNodes) {}

        public override TaskStatus Tick()
        {
            bool isAnyChildRunning = false;
            
            foreach(var child in childNodes)
            {
                switch (child.Tick())
                {
                    case TaskStatus.Failure:
                        {
                            Status = TaskStatus.Failure;
                            return Status;
                        }
                    case TaskStatus.Running:
                        {
                            isAnyChildRunning = true;
                            continue;
                        }
                }
            }

            Status = isAnyChildRunning ? TaskStatus.Running : TaskStatus.Success;
            
            return Status;
        }
    }

    public class SelectorBT : CompositeTaskBT
    {
        public SelectorBT(List<TaskBT> childNodes) : base(childNodes) {}

        public override TaskStatus Tick()
        {
            foreach(var child in childNodes)
            {
                if (child.Tick() != TaskStatus.Failure)
                {
                    Status = child.Status;
                    return child.Status;
                }
            }

            Status = TaskStatus.Failure;
            return TaskStatus.Failure;
        }
    }
}
