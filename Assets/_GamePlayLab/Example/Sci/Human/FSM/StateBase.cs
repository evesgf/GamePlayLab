using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{

    public abstract class StateBase : MonoBehaviour, IState
    {
        internal StateMachine m_FSM;

        public abstract int GetStateID();

        private void Start()
        {
            m_FSM = GetComponent<StateMachine>();
            if (m_FSM != null && m_FSM.RegistState(this))
            {
                Debug.Log(this.name + ":RegistState Ok!");
            }
            else
            {
                Debug.Log(this.name + ":RegistState Error!");
            }
        }

        public virtual void OnEnter(StateMachine stateMachine, IState prevState, object param1, object param2)
        {
            Debug.Log("===> " + this.name + " OnEnter");
        }

        public abstract void OnUpdate(float elapseSeconds, float realElapseSeconds);

        public abstract void OnFixedUpdate(float elapseSeconds, float realElapseSeconds);

        public abstract void OnLateUpdate(float elapseSeconds, float realElapseSeconds);

        public virtual void OnLeave(IState nextState, object param1, object param2)
        {
            Debug.Log("<=== " + this.name + " OnLeave");
        }
    }
}
