using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPL
{
    public class WarpTunnelEngine : MonoBehaviour
    {
        public Vector3 target;
        public float minWarpDistance;
        public float maxWarpDistance;
        public float endWarpInterval;                   //最终跳点小于目标跳点则忽略

        public AnimationCurve startSpeedCurve;          //起跳速度渐变曲线
        public float maxWarpSpeed;

        public WarpTunnelEffect warpTunnelEffect;
        public float startDuration = 3f;

        Vector3 targetVel;
        Vector3 realTargetPos;
        float targetDistance;
        float nowTargetDistance;
        float nowWarpSpeed;

        float startWarpTunnelTime;

        bool warpTunnelStart, warpTunnelEnd;            //Start为进入Tunnel，End为离开Tunnel

        private WarpTunnelEffect nowWarpTunnelEffect;

        public void WarpTunnelStart(Vector3 target)
        {
            this.target = target;

            targetVel = (target - transform.position).normalized;
            //计算最大跳跃距离
            targetDistance = Vector3.Distance(transform.position, target);
            if (targetDistance < minWarpDistance)
            {
                print("距离过近");
                return;
            } else if(targetDistance> maxWarpDistance)
            {
                //超出最大跳跃距离时如果大于忽略值则跳至最大距离否则直接跳目标点
                realTargetPos = (targetDistance - maxWarpDistance) >= endWarpInterval ? (target - transform.position).normalized*maxWarpDistance : target;
            }

            StartCoroutine(OnWarpTunnelStart());

        }

        IEnumerator OnWarpTunnelStart()
        {
            nowWarpTunnelEffect = Instantiate(warpTunnelEffect, transform);
            nowWarpTunnelEffect.StartWarpTunnel(startDuration);
            warpTunnelStart = true;
            startWarpTunnelTime = 0;
            yield return new WaitForSeconds(startDuration);
        }

        private void Update()
        {
            //计算与目标点的位置
            nowTargetDistance = Vector3.Distance(transform.position, realTargetPos);

            if (warpTunnelStart)
            {
                if (startWarpTunnelTime >= startDuration)
                {
                    warpTunnelStart = false;
                }
                //如果在加速过程中已经走过一半的距离，则退出加速进入WarpTunnelEnd
                if (nowTargetDistance >= targetDistance / 2)
                {
                    warpTunnelStart = false;
                    warpTunnelEnd = true;
                }
                else
                {
                    nowWarpSpeed = maxWarpSpeed * startSpeedCurve.Evaluate(startWarpTunnelTime / startDuration);
                }
            }
            else if (warpTunnelEnd)
            {
                nowWarpSpeed = maxWarpSpeed * startSpeedCurve.Evaluate(1-startWarpTunnelTime / startDuration);
            }
            else
            {
                //if ()
                //{
                //    warpTunnelEnd = true;
                //}
                nowWarpSpeed = maxWarpSpeed;
            }

            transform.position += targetVel * nowWarpSpeed * Time.deltaTime;
            startWarpTunnelTime += Time.deltaTime;
        }
    }
}
