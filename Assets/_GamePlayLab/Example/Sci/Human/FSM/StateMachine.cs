using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GPL
{

    public class StateMachine : MonoBehaviour
    {
        public GameObject m_owner;
        public Text txt_showState;

        /// <summary>
        /// 存储所有注册进来的状态。key是状态ID，value是状态对象
        /// </summary>
        private Dictionary<int, IState> m_dictState;
        /// <summary>
        /// 当前运行的状态
        /// </summary>
        private IState m_curState;
        /// <summary>
        /// 上一个运行的状态
        /// </summary>
        private IState m_lastState;
        /// <summary>
        /// 下一个要跳转的状态
        /// </summary>
        private IState m_nextState;

        void Awake()
        {

        }

        public void Init(GameObject owner)
        {
            m_curState = null;
            m_dictState = new Dictionary<int, IState>();
            m_owner = owner;
        }

        IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            foreach (var item in m_dictState)
            {
                Debug.Log(item.Value);
            }
        }

        /// <summary>
        /// 注册一个状态
        /// </summary>
        /// <param name="state">要注册的状态</param>
        /// <returns>成功返回true，如果此状态ID已存在或状态为NULL，则返回false</returns>
        public bool RegistState(IState state)
        {
            if (null == state)
            {
                Debug.LogWarning("StateMachine::RegistState->state is null");
                return false;
            }

            if (m_dictState.ContainsKey(state.GetStateID()))
            {
                Debug.LogWarning("StateMachine::RegistState->state had exist! state id=" + state.GetStateID());
                return false;
            }

            m_dictState[state.GetStateID()] = state;

            return true;
        }

        /// <summary>
        /// 尝试获取一个状态
        /// </summary>
        /// <param name="iStateId"></param>
        /// <returns></returns>
        public IState GetState(int iStateId)
        {
            IState ret = null;
            m_dictState.TryGetValue(iStateId, out ret);
            return ret;
        }

        /// <summary>
        /// 停止当前正在运行的状态, 切换到空状态
        /// </summary>
        public void StopState(object param1, object param2)
        {
            if (null == m_curState)
            {
                return;
            }

            m_curState.OnLeave(null, param1, param2);

            m_curState = null;
        }

        /// <summary>
        /// 取消状态的注册
        /// </summary>
        /// <param name="iStateID">要取消的状态ID</param>
        /// <returns>如果找不到状态或状态正在运行，则会返回false</returns>
        public bool CancelState(int iStateID)
        {
            if (!m_dictState.ContainsKey(iStateID))
            {
                return false;
            }

            if (null != m_curState && m_curState.GetStateID() == iStateID)
            {
                return false;
            }

            return m_dictState.Remove(iStateID);
        }

        public delegate void BetweenSwitchState(IState from, IState to, object param1, object param2);

        /// <summary>
        /// 在切换状态之间回调
        /// </summary>
        public BetweenSwitchState BetweenSwitchStateCallBack { get; set; }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="iNewStateID">要切换的新状态</param>
        /// <returns>如果找不到新的状态，或者新旧状态一样，返回false</returns>
        public bool SwitchState(int iNewStateID, object param1, object param2)
        {
            //状态一样，不做转换//
            //if (null != m_curState && m_curState.GetStateID() == iNewStateID)

            IState newState = null;
            m_dictState.TryGetValue(iNewStateID, out newState);
            if (null == newState)
            {
                return false;
            }

            m_lastState = m_curState;
            m_nextState = newState;

            if (null != m_lastState)
            {
                m_lastState.OnLeave(newState, param1, param2);
            }

            if (BetweenSwitchStateCallBack != null) BetweenSwitchStateCallBack(m_lastState, newState, param1, param2);

            m_curState = newState;

            if (null != newState)
            {
                newState.OnEnter(this, m_lastState, param1, param2);
            }

            return true;
        }

        /// <summary>
        /// 获取当前状态
        /// </summary>
        /// <returns></returns>
        public IState GetCurState()
        {
            return m_curState;
        }

        /// <summary>
        /// 获取当前状态ID
        /// </summary>
        /// <returns></returns>
        public int GetCurStateID()
        {
            IState state = GetCurState();
            return (null == state) ? 0 : state.GetStateID();
        }

        /// <summary>
        /// 获取上一个状态
        /// </summary>
        /// <returns></returns>
        public IState GetLastState()
        {
            return m_lastState;
        }

        /// <summary>
        /// 获取上一个状态ID
        /// </summary>
        /// <returns></returns>
        public int GetLastStateID()
        {
            IState state = GetLastState();
            return (null == state) ? 0 : state.GetStateID();
        }

        /// <summary>
        /// 获取下一个状态
        /// </summary>
        /// <returns></returns>
        public IState GetNextState()
        {
            return m_nextState;
        }

        public int GetNextStateID()
        {
            IState state = GetNextState();
            return (null == state) ? 0 : state.GetStateID();
        }

        /// <summary>
        /// 判断当前是否在某个状态下
        /// </summary>
        /// <param name="iStateID"></param>
        /// <returns></returns>
        public bool IsInState(int iStateID)
        {
            if (null == m_curState)
            {
                return false;
            }

            return m_curState.GetStateID() == iStateID;
        }

        /// <summary>
        /// 每帧的更新回调
        /// </summary>
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (null != m_curState)
            {
                m_curState.OnUpdate(elapseSeconds, realElapseSeconds);
            }

            string info = (null != m_curState) ? ((PlayerState)m_curState.GetStateID()).ToString() : "null";
            Debug.Log(info);
        }

        /// <summary>
        /// 每帧的更新回调
        /// </summary>
        public void OnFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (null != m_curState)
            {
                m_curState.OnFixedUpdate(elapseSeconds, realElapseSeconds);
            }
        }

        /// <summary>
        /// 每帧的更新回调
        /// </summary>
        public void OnLateUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (null != m_curState)
            {
                m_curState.OnLateUpdate(elapseSeconds, realElapseSeconds);
            }
        }
    }

}