using Cinemachine;
using UnityEngine;

namespace Slate.Cinemachine
{
    [ExecuteInEditMode, DisallowMultipleComponent]
    public class CinemachineSlateCamera : CinemachineVirtualCameraBase
    {
        /// <summary>Blended camera state</summary>
        private CameraState _state = CameraState.Default;

        private CinemachineVirtualCameraBase _gameplayCamera;
        private CinemachineBrain _brain;

        private CinemachineVirtualCameraBase Camera1 { get; set; }

        private CinemachineVirtualCameraBase Camera2 { get; set; }

        private float Weight { get; set; }



        public override CameraState State
        {
            get { return _state; }
        }

        protected override void OnEnable()
        {
            _brain = CinemachineCore.Instance.FindPotentialTargetBrain(this);
            if (_brain)
            {
                _gameplayCamera = (CinemachineVirtualCameraBase)_brain.ActiveVirtualCamera;
            }
            else
            {
                Debug.LogError("No Cinemachine Brain in scene, please create one.");
            }
            base.OnEnable();
        }

        public ICinemachineCamera LiveChild { set; get; }

        //public override ICinemachineCamera LiveChildOrSelf { get { return LiveChild; } }

        public override bool IsLiveChild(ICinemachineCamera vcam)
        {
            return vcam == LiveChild
                || ((CinemachineVirtualCameraBase)vcam == Camera1 || (CinemachineVirtualCameraBase)vcam == Camera2);
        }

        public override Transform LookAt { get; set; }

        public override Transform Follow { get; set; }

        public void SetCameras(CinemachineVirtualCameraBase camera1, CinemachineVirtualCameraBase camera2, float weight)
        {
            var tempEnabled = enabled;
            enabled = !(camera1 == null && camera2 == null);//if we have no cameras we are not enabled;
            if (tempEnabled != enabled)//from off to on IE CUT
            {
                if (_brain)
                {
                    if (_brain.ActiveVirtualCamera != null)
                    {
                        if (_brain.ActiveVirtualCamera.VirtualCameraGameObject == this.VirtualCameraGameObject)
                        {
                            if (_brain.ActiveBlend != null)
                            {
                                _brain.ActiveBlend.TimeInBlend = _brain.ActiveBlend.Duration;
                            }
                        }
                    }
                }
            }
            Camera1 = camera1 ? camera1 : _gameplayCamera;
            Camera2 = camera2 ? camera2 : _gameplayCamera;
            Weight = weight;
        }

        ///----------------------------------------------------------------------------------------------
#if CINEMACHINE_ASSET_STORE //Deprecated version of cinemachine
        public override void UpdateCameraState(Vector3 worldUp, float deltaTime)
        {
            if (!Camera2)
                return;
            if (!Camera1)
                return;
            _state = Camera2.State;
            _state = CameraState.Lerp(_state, Camera1.State, 1 - Weight);
            LiveChild = Weight > .5f ? Camera2 : Camera1;
        }
#else
        public override void InternalUpdateCameraState(Vector3 worldUp, float deltaTime)
        {
            if (!Camera2)
                return;
            if (!Camera1)
                return;
            _state = Camera2.State;
            _state = CameraState.Lerp(_state, Camera1.State, 1 - Weight);
            LiveChild = Weight > .5f ? Camera2 : Camera1;
        }
#endif

    }
}