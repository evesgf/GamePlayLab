using System;
using UnityEditor;
using UnityEngine;

namespace Slate.UTJ.FrameCapturer
{
    [CustomEditor(typeof(AudioRecorder))]
    public class AudioRecorderEditor : RecorderBaseEditor
    {
        public override void OnInspectorGUI()
        {
            var so = serializedObject;

            CommonConfig();
            EditorGUILayout.Space();
            FramerateControl();
            EditorGUILayout.Space();
            RecordingControl();

            so.ApplyModifiedProperties();
        }
    }
}
