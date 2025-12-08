using System;
using System.Collections.Generic;

namespace SimpleBehaviorTree
{
    public abstract class TaskBT
    {
        public enum TaskStatus { Running, Success, Failure};
        public abstract TaskStatus Tick();
        public virtual void Reset() { }
    }
    /// <summary>
    /// Simple action that runs and returns success.
    /// </summary>
    public class SimpleActionBT : TaskBT
    {
        Action behavior;

        public SimpleActionBT(Action behavior) => this.behavior = behavior;
        public override TaskStatus Tick()
        {
            if (behavior == null)
                return TaskStatus.Failure;

            behavior();
            return TaskStatus.Success;
        }
    }

    /// <summary>
    /// Action that can be completed. The passed in delegate behavior must have return type bool. It should return true when the action is completed.
    /// </summary>
    public class CompleteableActionBT : TaskBT
    {
        Func<bool> behavior;

        public CompleteableActionBT(Func<bool> behavior) => this.behavior = behavior;
        public override TaskStatus Tick()
        {
            if (behavior == null)
                return TaskStatus.Failure;

            //if behavior completes in this tick, status for this action becomes success, otherwise it is running
            return behavior() ? TaskStatus.Success : TaskStatus.Running;
        }
    }
    /// <summary>
    /// Tick() returns true if condition is met.
    /// </summary>
    public class ConditionBT : TaskBT
    {
        protected Func<bool> condition;
        public ConditionBT(Func<bool> condition) => this.condition = condition;

        public override TaskStatus Tick()
        {
            if (condition == null)
                return TaskStatus.Failure;

            return condition() ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
    /// <summary>
    /// Tick() returns true if condition is NOT met.
    /// </summary>
    public class InvertedConditionBT : ConditionBT
    {
        public InvertedConditionBT(Func<bool> condition) : base(condition) {}

        public override TaskStatus Tick()   
        {
            if (condition == null)
                return TaskStatus.Failure;

            return condition() ? TaskStatus.Failure : TaskStatus.Success;
        }
    }
    
    public abstract class CompositeTaskBT : TaskBT
    {
        protected List<TaskBT> childNodes = new();
        protected int currentChild;
        public CompositeTaskBT(List<TaskBT> childNodes) => this.childNodes = childNodes;
        public void AddChild(TaskBT child) => this.childNodes.Add(child);
        /// <summary>
        /// Resets entire subtree.
        /// </summary>
        public override void Reset()
        {
            currentChild = 0;
            foreach(TaskBT child in childNodes)
                child.Reset();
        }
    }

    public class SequenceBT : CompositeTaskBT
    {
        public SequenceBT(List<TaskBT> childNodes) : base(childNodes) {}

        public override TaskStatus Tick()
        {
            while(currentChild < childNodes.Count)
            {
                switch (childNodes[currentChild].Tick())
                {
                    case TaskStatus.Running:
                        {
                            return TaskStatus.Running;
                        }
                    case TaskStatus.Failure:
                        {
                            Reset();
                            return TaskStatus.Failure;
                        }
                }

                //if child succeded, try next child
                currentChild++;
            }

            //if we haven't yet returned from the while loop above, it means all tasks in this sequence have succeeded
            Reset();
            return TaskStatus.Success;
        }
    }

    public class SelectorBT : CompositeTaskBT
    {
        public SelectorBT(List<TaskBT> childNodes) : base(childNodes) {}

        public override TaskStatus Tick()
        {
            while (currentChild < childNodes.Count)
            {
                switch (childNodes[currentChild].Tick())
                {
                    case TaskStatus.Running:
                        {
                            return TaskStatus.Running;
                        }
                    case TaskStatus.Success:
                        {
                            Reset();
                            return TaskStatus.Success;
                        }
                }

                //if child failed, try next child
                currentChild++;
            }

            //if we did not return in the while loop, it means all tasks belonging to this selector failed 
            Reset();
            return TaskStatus.Failure;
        }
    }
}
