using Cinemachine;
using Cinemachine.Editor;
using Slate.Cinemachine;
using UnityEditor;
using UnityEngine;

namespace Slate
{
    [CustomEditor(typeof(CinemachineClip))]
    public class CinemachineClipEditor : ActionClipInspector<CinemachineClip>
    {
        SerializedProperty blendInCurve;
        SerializedProperty blendOutCurve;

        void OnEnable()
        {
            blendInCurve = serializedObject.FindProperty("BlendInCurve");
            blendOutCurve = serializedObject.FindProperty("BlendOutCurve");
        }

        public override void OnInspectorGUI()
        {
            if (!action.VirtualCamera)
                EditorGUILayout.HelpBox("No Virtual Camera Set.", MessageType.Error);

            base.ShowCommonInspector();

            if (!action.VirtualCamera)
            {
                if (GUILayout.Button("Create Virtual Camera"))
                {
                    GameObject go = new GameObject(string.Format("CM vcam{0}", FindObjectsOfType<CinemachineVirtualCamera>().Length + 1));
                    var virtualCameraBase = go.AddComponent<CinemachineVirtualCamera>();
                    virtualCameraBase.transform.SetParent(action.root.context.transform);
                    action.VirtualCamera = virtualCameraBase;
                    if (SceneView.lastActiveSceneView)
                    {
                        Camera sceneCamera = SceneView.lastActiveSceneView.camera;
                        if (sceneCamera)
                        {
                            virtualCameraBase.transform.position = sceneCamera.transform.position;
                            virtualCameraBase.transform.rotation = sceneCamera.transform.rotation;
                        }

                    }
                    Undo.RegisterCreatedObjectUndo(virtualCameraBase.gameObject, "create " + virtualCameraBase.name);
                }

            }

            if (action.blendIn != 0)
            {
                action.BlendInType = (BlendType)EditorGUILayout.EnumPopup("Blend In Type", action.BlendInType);

                serializedObject.Update();

                if (action.BlendInType == BlendType.Custom)
                {
                    EditorGUILayout.PropertyField(blendInCurve);
                }

                serializedObject.ApplyModifiedProperties();

            }
            if (action.blendOut != 0)
            {
                if (action.GetNextClip() != null)
                {
                    if (action.GetNextClip().startTime > action.endTime)
                    {

                        action.BlendOutType =
                            (BlendType)EditorGUILayout.EnumPopup("Blend Out Type", action.BlendOutType);

                        serializedObject.Update();


                        if (action.BlendOutType == BlendType.Custom)
                        {
                            EditorGUILayout.PropertyField(blendOutCurve);
                        }

                        serializedObject.ApplyModifiedProperties();
                    }
                }
                else
                {

                    action.BlendOutType =
                        (BlendType)EditorGUILayout.EnumPopup("Blend Out Type", action.BlendOutType);

                    serializedObject.Update();


                    if (action.BlendOutType == BlendType.Custom)
                    {
                        EditorGUILayout.PropertyField(blendOutCurve);
                    }

                    serializedObject.ApplyModifiedProperties();
                }
            }

            base.ShowAnimatableParameters();

        }
    }
}