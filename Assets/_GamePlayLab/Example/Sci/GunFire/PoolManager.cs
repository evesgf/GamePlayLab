using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class PoolManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    public class Pool
    {
        private readonly List<GameObject> objects;
        private readonly List<GameObject> releaseObjs;

        private float autoReleaseInterval;
        private float capacity;
        private float expireTime;

        private float autoReleaseTime = 0f;

        /// <summary>
        /// 初始化对象池
        /// </summary>
        /// <param name="autoReleaseInterval">自动释放间隔秒数</param>
        /// <param name="capacity">对象池容量</param>
        /// <param name="expireTime">对象过期秒数</param>
        /// <returns></returns>
        public Pool(float autoReleaseInterval,int capacity,float expireTime)
        {
            objects = new List<GameObject>();
            releaseObjs = new List<GameObject>();

            this.autoReleaseInterval = autoReleaseInterval;
            this.capacity = capacity;
            this.expireTime = expireTime;

            autoReleaseTime = 0f;
        }

        public int Count
        {
            get
            {
                return objects.Count;
            }
        }

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="obj"></param>
        public void Register(GameObject obj,Action<GameObject> action)
        {
            if (obj == null)
            {
                Debug.LogError("Register obj is null!");
                return;
            }

            GameObject go;
            //检查是否有闲置对象
            if (releaseObjs.Count > 0)
            {
                go = releaseObjs[0];
                releaseObjs.RemoveAt(0);
            }
            else
            {
                go = GameObject.Instantiate(obj);
                objects.Add(go);
            }

            //随后执行相关方法
            action.Invoke(go);

            //检查是否超出数量
            if (Count > capacity)
            {
                Release();
            }
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject Spawn(string name)
        {
            return null;
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="obj"></param>
        public void UnSpawn(GameObject obj,Action<GameObject> action)
        {
            if (obj == null)
            {
                Debug.LogError("UnSpawn obj is null!");
                return;
            }

            releaseObjs.Add(obj);

            action.Invoke(obj);

            //检查是否超出数量
            if (Count > capacity)
            {
                Release();
            }
        }

        /// <summary>
        /// 释放可供释放的对象
        /// </summary>
        public void Release()
        {

        }

        /// <summary>
        /// 释放所有未使用的对象
        /// </summary>
        public void ReleaseAllUnUsed()
        {

        }

        internal void Update(float elapseSeconds,float realElapseSeconds)
        {
            autoReleaseTime += realElapseSeconds;
            if (autoReleaseTime < autoReleaseInterval)return;

            Release();
        }
    }
}
