using System;
using System.Linq;
using Cinemachine;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Slate.Cinemachine
{
    [Attachable(typeof(CinemachineTrack))]
    [Description(
        "Each clip requires a Cinemachine Virtual Camera reference. \nDefault Cinemachine transitions are overriden by Slate-controlled blends or cuts. \nEasing options will be exposed when a blend is present. \nThe priority of the camera is inherited from the Cinemachine Track."
        )]
    public class CinemachineClip : DirectorActionClip
    {
        [Required]
        public CinemachineVirtualCameraBase VirtualCamera;

        [SerializeField, HideInInspector]
        private float _length = 5f;

        [SerializeField, HideInInspector]
        private float _blendIn;

        [SerializeField, HideInInspector]
        private float _blendOut;


        [SerializeField, HideInInspector]
        private BlendType _blendInType = BlendType.EaseInOut;

        public BlendType BlendInType
        {
            get { return _blendInType; }
            set
            {
                if (_blendInType == value)
                    return;
                switch (value)
                {
                    case BlendType.EaseInOut:
                        BlendInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
                        break;
                    case BlendType.Linear:
                        BlendInCurve = AnimationCurve.Linear(0, 0, 1, 1);
                        break;
                    case BlendType.Custom:
                        break;
                }
                _blendInType = value;
            }
        }

        [SerializeField, HideInInspector]
        private BlendType _blendOutType = BlendType.EaseInOut;

        public BlendType BlendOutType
        {
            get { return _blendOutType; }
            set
            {
                if (_blendOutType == value)
                    return;
                switch (value)
                {
                    case BlendType.EaseInOut:
                        BlendOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
                        break;
                    case BlendType.Linear:
                        BlendOutCurve = AnimationCurve.Linear(0, 1, 1, 0);
                        break;
                    case BlendType.Custom:
                        break;
                }
                _blendOutType = value;
            }
        }

        [SerializeField, HideInInspector]
        public AnimationCurve BlendInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [SerializeField, HideInInspector]
        public AnimationCurve BlendOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        private bool _previousEnabledState = false;

        private Transform _previousParent;

        public new GameObject actor
        {
            get { return VirtualCamera ? VirtualCamera.gameObject : base.actor; }
        }

        public override string info
        {
            get
            {
#if UNITY_EDITOR
                return VirtualCamera != null
                    ? (Prefs.showShotThumbnails && length > 0 ? null : VirtualCamera.name)
                    : "Please add a camera.";
#else
				return VirtualCamera != null? VirtualCamera.name : "Please add a camera.";
#endif
            }
        }

        public override bool isValid
        {
            get { return VirtualCamera != null; }
        }

        public override float blendIn
        {
            get { return _blendIn; }
            set { _blendIn = value; }
        }

        public override float blendOut
        {
            get { return _blendOut; }
            set { _blendOut = value; }
        }

        public float GetShotWeight(float time)
        {
            if (time <= 0)
            {
                return blendIn <= 0 ? 1 : 0;
            }

            if (time >= length)
            {
                return blendOut <= 0 ? 1 : 0;
            }

            if (time < blendIn)
            {
                return BlendInCurve.Evaluate(time / blendIn);
            }

            if (time > length - blendOut)
            {
                return BlendInCurve.Evaluate((length - time) / blendOut);
            }

            return 1;
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            if (VirtualCamera != null)
            {
                _previousParent = VirtualCamera.transform.parent;
                _previousEnabledState = VirtualCamera.enabled;

                VirtualCamera.enabled = true;
                VirtualCamera.transform.SetParent(((CinemachineTrack)parent).Camera.transform, true);
            }
        }

        protected override void OnExit()
        {
            base.OnExit();
            if (VirtualCamera != null)
            {
                VirtualCamera.enabled = _previousEnabledState;
                VirtualCamera.transform.SetParent(_previousParent, true);
            }
        }

        protected override void OnReverse()
        {
            base.OnReverse();
            if (VirtualCamera != null)
            {
                VirtualCamera.enabled = _previousEnabledState;
                VirtualCamera.transform.SetParent(_previousParent, true);
            }
        }

        protected override void OnReverseEnter()
        {
            base.OnReverseEnter();
            if (VirtualCamera != null)
            {
                _previousParent = VirtualCamera.transform.parent;
                _previousEnabledState = VirtualCamera.enabled;
                VirtualCamera.enabled = true;
                VirtualCamera.transform.SetParent(((CinemachineTrack)parent).Camera.transform, true);
            }
        }

        public override bool canCrossBlend
        {
            get { return true; }
        }

        public override float length
        {
            get { return _length; }
            set { _length = value; }
        }


#if UNITY_EDITOR
        [System.NonSerialized]
        private int thumbRefresher;
        [System.NonSerialized]
        private Texture2D thumbnail;

        protected override void OnClipGUI(Rect rect)
        {
            if (VirtualCamera == null || rect.width < 40)
            {
                return;
            }

            if (Prefs.showShotThumbnails)
            {
                if (thumbRefresher == 0 || thumbRefresher % Prefs.thumbnailsRefreshInterval == 0)
                {
                    var res = EditorTools.GetGameViewSize();
                    var width = (int)res.x;
                    var height = (int)res.y;
                    thumbnail = GetRenderTexture(VirtualCamera, width, height);
                }

                thumbRefresher++;

                if (thumbnail != null)
                {
                    GUI.backgroundColor = Color.clear;
                    var style = new GUIStyle("Box");
                    style.alignment = TextAnchor.MiddleCenter;
                    var thumbRect = new Rect(0, 0, 100, rect.height);

                    if (blendIn > 0)
                    {
                        var previousClip = GetPreviousClip();
                        if (previousClip != null && previousClip.endTime > this.startTime)
                        {
                            thumbRect.x += (blendIn / length) * rect.width;
                        }
                    }

                    if (blendOut > 0)
                    {
                        var nextClip = GetNextClip();
                        if (nextClip != null && nextClip.startTime < this.endTime)
                        {
                            thumbRect.width = Mathf.Min(thumbRect.width, rect.width - ((blendOut / length) * rect.width));
                        }
                    }

                    GUI.Box(thumbRect, thumbnail, style);
                    GUI.backgroundColor = Color.white;
                }

                var info = VirtualCamera.name;
                var track = (parent as CinemachineTrack);


                var bevelRect = rect;
                bevelRect.center += new Vector2(1, 1);
                GUI.color = Color.white;
                GUI.Label(bevelRect, info, Styles.centerLabel);
                GUI.color = new Color(0, 0, 0, 0.7f);
                GUI.Label(rect, info, Styles.centerLabel);
                GUI.color = Color.white;

                if (startTime < root.currentTime && endTime >= root.currentTime)
                {
                    if (track != null &&
                        track.Camera.enabled &&
                        track.Camera.VirtualCameraGameObject !=
                        track.Brain.ActiveVirtualCamera.VirtualCameraGameObject)
                    {
                        EditorGUI.HelpBox(rect, "Track does not have priority", MessageType.Warning);
                    }
                }
            }
        }

        //Get a RenderTexture of this camera with specified width and height.
        public Texture2D GetRenderTexture(CinemachineVirtualCameraBase virtualCamera, int width, int height)
        {
            return CinemachineTrack.GetThumbnail(virtualCamera, width, height);
        }
#endif
    }

    public enum BlendType
    {
        EaseInOut,
        Linear,
        Custom
    }
}
