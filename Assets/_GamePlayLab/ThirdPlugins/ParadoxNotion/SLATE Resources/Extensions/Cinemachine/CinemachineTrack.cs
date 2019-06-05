using System.Collections.Generic;
using Cinemachine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Slate.Cinemachine
{
    [UniqueElement]
    [Attachable(typeof(DirectorGroup))]
    [Description("A Cinemachine Track composes a sequence of shots using Cinemachine Virtual Cameras. \nA Cinemachine Brain is required in the scene. \nTo ensure this track is current, the Priority value must be higher than any virtual cameras in the scene as well as Ciemamchine Tracks currrently playing in other Slate cutscenes.")]
    [Icon("Camera Icon")]
    public class CinemachineTrack : CutsceneTrack
    {
        private const float Version = 1.03f;
        [SerializeField, HideInInspector]
        private float _version = Version;

        public CinemachineVirtualCameraBase ExitCamera;

        [SerializeField, HideInInspector]
        private CinemachineSlateCamera _camera;
        public CinemachineSlateCamera Camera
        {
            get
            {
                if (_camera) return _camera;

                _camera = root.context.GetComponentInChildren<CinemachineSlateCamera>();

                if (_camera) return _camera;

                GameObject go = new GameObject("★ Cinemachine Slate Camera", typeof(CinemachineSlateCamera));
                go.transform.SetParent(root.context.transform);
                _camera = go.GetComponent<CinemachineSlateCamera>();
                _camera.Priority = 100;
                _camera.enabled = false;
                return _camera;
            }


        }

        public int Priority
        {
            get { return Camera.Priority; }
            set { Camera.Priority = value; }
        }

        public CinemachineBrain Brain;

        public override string info
        {
            get { return string.Format("Priority: {0}\nv{1}", Priority, _version.ToString("#0.0#")); }
        }

        protected override bool OnInitialize()
        {

            Brain = CinemachineCore.Instance.FindPotentialTargetBrain(Camera);
            Camera.enabled = false;
            Camera.Priority = Priority;
            return true;
        }

        protected override void OnEnter()
        {
            if (!Brain)
                Brain = CinemachineCore.Instance.FindPotentialTargetBrain(Camera);
        }

        protected override void OnExit()
        {
            if (!Brain)
                return;
            Camera.SetCameras(null, null, 0);
            if (ExitCamera)
            {
                ExitCamera.enabled = true;
            }
        }

        protected override void OnReverseEnter()
        {
            if (!Brain)
                Brain = CinemachineCore.Instance.FindPotentialTargetBrain(Camera);
        }

        protected override void OnReverse()
        {
            if (!Brain)
                return;
            Camera.SetCameras(null, null, 0);

        }

        protected override void OnUpdate(float time, float previousTime)
        {
            if (!Brain)
                return;
            int activeInputs = 0;
            CinemachineVirtualCameraBase camA = null;
            CinemachineVirtualCameraBase camB = null;
            float camWeight = 1f;
            for (int i = 0; i < clips.Count; ++i)
            {
                var shot
                    = clips[i] as CinemachineClip;
                var weight = shot.GetShotWeight(time - shot.startTime);
                if (shot.VirtualCamera != null && (shot.startTime < time && shot.endTime >= time))
                {
                    if (activeInputs == 1)
                        camB = camA;
                    camWeight = weight;
                    camA = shot.VirtualCamera;
                    ++activeInputs;
                    if (activeInputs == 2)
                        break;
                }
            }
            Camera.SetCameras(camB, camA, camWeight);
#if UNITY_EDITOR
            Brain.SendMessage("LateUpdate");
#endif
        }

#if UNITY_EDITOR

        public override float defaultHeight
        {
            get { return Prefs.showShotThumbnails ? 60f : base.defaultHeight; }
        }
#endif
#if UNITY_EDITOR
        public override void OnTrackTimelineGUI(Rect posRect, Rect timeRect, float cursorTime,
            System.Func<float, float> TimeToPos)
        {
            var current = Event.current;
            if (current.type == EventType.DragPerform)
            {
                if (posRect.Contains(current.mousePosition))
                {
                    if (DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length > 0)
                    {
                        int count = 0;
                        for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                        {
                            GameObject go = DragAndDrop.objectReferences[i] as GameObject;
                            if (go != null)
                            {
                                var virtualCameraBase = go.GetComponent<CinemachineVirtualCameraBase>();
                                if (virtualCameraBase != null)
                                {
                                    var action = AddAction<CinemachineClip>(cursorTime + count++ * 1f);
                                    action.length = 1f;
                                    action.VirtualCamera = virtualCameraBase;
                                }
                            }
                        }
                    }
                    DragAndDrop.AcceptDrag();
                    Event.current.Use();
                }
            }

            base.OnTrackTimelineGUI(posRect, timeRect, cursorTime, TimeToPos);

        }



        /// <summary>
        /// Shared Thumbnail Textures
        /// </summary>
        private static Dictionary<CinemachineVirtualCameraBase, Texture2D> _textures;

        private static Camera _tempCamera;

        public static RenderTexture _tempTexture;

        public static Camera TempCamera
        {
            get
            {
                if (!_tempCamera)
                {
                    GameObject go = GameObject.Find("_cinemachine_track_preview_camera");
                    if (go)
                    {
                        _tempCamera = go.GetComponent<Camera>();
                        _textures = new Dictionary<CinemachineVirtualCameraBase, Texture2D>();
                    }
                    else
                    {
                        go = new GameObject("_cinemachine_track_preview_camera");
                        _tempCamera = go.AddComponent<Camera>();
                        _textures = new Dictionary<CinemachineVirtualCameraBase, Texture2D>();
                        go.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSaveInBuild |
                                       HideFlags.DontSaveInEditor;
                    }
                    if (!_tempTexture)
                    {
                        _tempTexture = new RenderTexture(16, 16, 16);
                    }
                }
                return _tempCamera;
            }
        }

        public static Texture2D GetThumbnail(CinemachineVirtualCameraBase camera, int width, int height)
        {

            Camera cam = TempCamera;
            var rt = cam.targetTexture;
            if (rt == null)
            {
                rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
                rt.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
            }
            cam.targetTexture = rt;
            cam.transform.position = camera.transform.position;
            cam.transform.rotation = camera.transform.rotation;
            cam.transform.localScale = camera.transform.localScale;
            cam.fieldOfView = camera.State.Lens.FieldOfView;
            cam.Render();

            if (!_textures.ContainsKey(camera))
                _textures.Add(camera, new Texture2D(cam.targetTexture.width, cam.targetTexture.height, TextureFormat.ARGB32, false));
            if (_textures[camera] == null)
                _textures[camera] = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, TextureFormat.ARGB32, false);
            Graphics.CopyTexture(cam.targetTexture, _textures[camera]);
            return _textures[camera];
        }
#endif
    }
}