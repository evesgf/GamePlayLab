using UnityEditor;

namespace Slate.Cinemachine
{
    [CustomEditor(typeof(CinemachineTrack))]
    public class CinemachineTrackEditor : CutsceneTrackInspector
    {
        public override void OnInspectorGUI()
        {
            CinemachineTrack cinemachineTrack = (CinemachineTrack)target;

            if (!cinemachineTrack.Brain)
                EditorGUILayout.HelpBox("No Brain Set.", MessageType.Error);
            base.OnInspectorGUI();
            cinemachineTrack.Priority = EditorGUILayout.IntField("Priority", cinemachineTrack.Priority);
        }
    }
}