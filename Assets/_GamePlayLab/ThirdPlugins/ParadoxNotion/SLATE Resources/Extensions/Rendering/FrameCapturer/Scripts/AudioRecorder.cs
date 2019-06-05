#if UNITY_EDITOR

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;


namespace Slate.UTJ.FrameCapturer
{
    [AddComponentMenu("UTJ/FrameCapturer/Audio Recorder")]
    [RequireComponent(typeof(AudioListener))]
    [ExecuteInEditMode]
    public class AudioRecorder : RecorderBase
    {
        #region fields
        [SerializeField] AudioEncoderConfigs m_audioEncoderConfigs = new AudioEncoderConfigs();
        AudioEncoder m_encoder;
        #endregion


        public override bool BeginRecording()
        {
            if (m_recording) { return false; }

            m_outputDir.CreateDirectory();

            // initialize encoder
            {
                string outPath = m_outputDir.GetFullPath() + "/" + DateTime.Now.ToString("yyyyMMdd_HHmmss");

                m_audioEncoderConfigs.Setup();
                m_encoder = AudioEncoder.Create(m_audioEncoderConfigs, outPath);
                if (m_encoder == null || !m_encoder.IsValid())
                {
                    EndRecording();
                    return false;
                }
            }

            base.BeginRecording();
            Debug.Log("AudioMRecorder: BeginRecording()");
            return true;
        }

        public override void EndRecording()
        {
            if (m_encoder != null)
            {
                m_encoder.Release();
                m_encoder = null;
            }

            if (m_recording)
            {
                Debug.Log("AudioMRecorder: EndRecording()");
            }
            base.EndRecording();
            
        }


        #region impl
        void LateUpdate()
        {
            ++m_frame;
        }

        void OnAudioFilterRead(float[] samples, int channels)
        {
            if (m_recording && m_encoder != null)
            {
                m_encoder.AddAudioSamples(samples);
                m_recordedSamples += samples.Length;
            }
        }
        #endregion
    }
}

#endif