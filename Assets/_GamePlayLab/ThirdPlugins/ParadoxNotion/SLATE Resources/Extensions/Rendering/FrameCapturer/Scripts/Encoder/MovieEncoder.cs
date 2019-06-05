#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace Slate.UTJ.FrameCapturer
{
    [Serializable]
    public class MovieEncoderConfigs
    {
        public MovieEncoder.Type format = MovieEncoder.Type.PNG;
        public fcAPI.fcPngConfig pngEncoderSettings = fcAPI.fcPngConfig.default_value;
        public fcAPI.fcExrConfig exrEncoderSettings = fcAPI.fcExrConfig.default_value;
        public fcAPI.fcGifConfig gifEncoderSettings = fcAPI.fcGifConfig.default_value;
        public fcAPI.fcWebMConfig webmEncoderSettings = fcAPI.fcWebMConfig.default_value;
        public fcAPI.fcMP4Config mp4EncoderSettings = fcAPI.fcMP4Config.default_value;

        public MovieEncoderConfigs(MovieEncoder.Type t)
        {
            format = t;
        }

        public bool supportVideo
        {
            get {
                return
                  format == MovieEncoder.Type.PNG ||
                  format == MovieEncoder.Type.EXR ||
                  format == MovieEncoder.Type.GIF ||
                  format == MovieEncoder.Type.WebM ||
                  format == MovieEncoder.Type.MP4;
            }
        }

        public bool supportAudio
        {
            get
            {
                return
                  format == MovieEncoder.Type.WebM ||
                  format == MovieEncoder.Type.MP4;
            }
        }

        public bool captureVideo
        {
            get
            {
                switch (format)
                {
                    case MovieEncoder.Type.PNG: return true;
                    case MovieEncoder.Type.EXR: return true;
                    case MovieEncoder.Type.GIF: return true;
                    case MovieEncoder.Type.WebM: return webmEncoderSettings.video;
                    case MovieEncoder.Type.MP4: return webmEncoderSettings.video;
                }
                return false;
            }
            set
            {
                webmEncoderSettings.video =
                mp4EncoderSettings.video = value;
            }
        }
        public bool captureAudio
        {
            get
            {
                switch (format)
                {
                    case MovieEncoder.Type.PNG: return false;
                    case MovieEncoder.Type.EXR: return false;
                    case MovieEncoder.Type.GIF: return false;
                    case MovieEncoder.Type.WebM: return webmEncoderSettings.audio;
                    case MovieEncoder.Type.MP4: return webmEncoderSettings.audio;
                }
                return false;
            }
            set
            {
                webmEncoderSettings.audio =
                mp4EncoderSettings.audio = value;
            }
        }

        public void Setup(int w, int h, int ch = 4, int targetFrameRate = 60)
        {
            pngEncoderSettings.width =
            exrEncoderSettings.width =
            gifEncoderSettings.width = 
            webmEncoderSettings.videoWidth =
            mp4EncoderSettings.videoWidth = w;

            pngEncoderSettings.height =
            exrEncoderSettings.height =
            gifEncoderSettings.height =
            webmEncoderSettings.videoHeight =
            mp4EncoderSettings.videoHeight = h;

            pngEncoderSettings.channels =
            exrEncoderSettings.channels = ch;

            webmEncoderSettings.videoTargetFramerate =
            mp4EncoderSettings.videoTargetFramerate = targetFrameRate;
        }
    }

    public abstract class MovieEncoder : EncoderBase
    {
        public enum Type
        {
            PNG,
            EXR,
            GIF,
            WebM,
            MP4,
        }
        static public Type[] GetAvailableEncoderTypes()
        {
            var ret = new List<Type>();
            if (fcAPI.fcPngIsSupported()) { ret.Add(Type.PNG); }
            if (fcAPI.fcExrIsSupported()) { ret.Add(Type.EXR); }
            if (fcAPI.fcGifIsSupported()) { ret.Add(Type.GIF); }
            if (fcAPI.fcWebMIsSupported()) { ret.Add(Type.WebM); }
            if (fcAPI.fcMP4OSIsSupported()) { ret.Add(Type.MP4); }
            return ret.ToArray();
        }


        public abstract Type type { get; }

        // config: config struct (fcGifConfig, fcWebMConfig, etc)
        public abstract void Initialize(object config, string outPath);
        public abstract void AddVideoFrame(byte[] frame, fcAPI.fcPixelFormat format, double timestamp = -1.0);
        public abstract void AddAudioSamples(float[] samples);


        public static MovieEncoder Create(Type t)
        {
            switch (t)
            {
                case Type.PNG: return new PngEncoder();
                case Type.EXR: return new ExrEncoder();
                case Type.GIF: return new GifEncoder();
                case Type.WebM:return new WebMEncoder();
                case Type.MP4: return new MP4Encoder();
            }
            return null;
        }

        public static MovieEncoder Create(MovieEncoderConfigs c, string path)
        {
            var ret = Create(c.format);
            switch (c.format)
            {
                case Type.PNG: ret.Initialize(c.pngEncoderSettings, path); break;
                case Type.EXR: ret.Initialize(c.exrEncoderSettings, path); break;
                case Type.GIF: ret.Initialize(c.gifEncoderSettings, path); break;
                case Type.WebM:ret.Initialize(c.webmEncoderSettings,path); break;
                case Type.MP4: ret.Initialize(c.mp4EncoderSettings, path); break;
            }
            return ret;
        }
    }
}

#endif