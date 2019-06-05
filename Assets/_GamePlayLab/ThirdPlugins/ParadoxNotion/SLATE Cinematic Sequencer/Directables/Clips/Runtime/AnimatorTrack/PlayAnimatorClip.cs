using UnityEngine;
using System.Collections;

namespace Slate.ActionClips
{

    [Name("Animation Clip")]
    [Description("Play an Animation Clip on target actor with Animator component attached.\nIf this is the Layer 0 Animator Track, you can choose to explicitely set a 'Starting Position' and a 'Starting Rotation' for when the clip starts. This also takes into account Root Motion if enabled in the Animator Track.")]
    [Attachable(typeof(AnimatorTrack))]
    public class PlayAnimatorClip : ActorActionClip, ISubClipContainable
    {

        public enum StartingTransformsMode
        {
            AutoMatchTransforms = 0,
            ManualSetTransforms = 1
        }

        [SerializeField]
        [HideInInspector]
        private float _length = 1f;
        [SerializeField]
        [HideInInspector]
        private float _blendIn = 0f;
        [SerializeField]
        [HideInInspector]
        private float _blendOut = 0f;

        [Header("Animation Clip Settings")]
        [Required]
        public AnimationClip animationClip;
        public float clipOffset;
        [Range(-5, 5)]
        public float playbackSpeed = 1f;

        [Header("Starting Transforms Settings")]
        [EnabledIf("isMasterTrack", 1)]
        public StartingTransformsMode startingTransformsMode;
        [EnabledIf("isMasterAndManualSet", 1)]
        public MiniTransformSpace transformSpace;
        [EnabledIf("isMasterAndManualSet", 1)]
        public Vector3 startingPosition;
        [EnabledIf("isMasterAndManualSet", 1)]
        public Vector3 startingRotation;

        [Header("---")]
        [AnimatableParameter("Local Rotation Offset")]
        [HelpBox("'Local Rotation Offset' is only used if this clip is part of the Layer 0 Track and RootMotion is enabled in that Track.\nYou can animate this parameter to rotate the actor while using it's RootMotion within the Animation Clip.\nThis can be very useful for animations like Walk or Run. Most of the times, simply animating 'Y' will suffice.")]
        public Vector2 steerLocalRotation;

        private Vector3 wasPosition;
        private Quaternion wasRotation;

        private bool isMasterAndManualSet {
            get { return isMasterTrack && startingTransformsMode == StartingTransformsMode.ManualSetTransforms; }
        }

        ///----------------------------------------------------------------------------------------------

        float ISubClipContainable.subClipOffset {
            get { return clipOffset; }
            set { clipOffset = value; }
        }

        float ISubClipContainable.subClipLength {
            get { return animationClip != null ? animationClip.length : 0; }
        }

        float ISubClipContainable.subClipSpeed {
            get { return playbackSpeed; }
        }

        public override string info {
            get { return animationClip != null ? animationClip.name : base.info; }
        }

        public override bool isValid {
            get { return base.isValid && animator != null && animationClip != null && !animationClip.legacy; }
        }

        public override float length {
            get { return _length; }
            set { _length = value; }
        }

        public override float blendIn {
            get { return _blendIn; }
            set { _blendIn = value; }
        }

        public override float blendOut {
            get { return _blendOut; }
            set { _blendOut = value; }
        }

        public override bool canCrossBlend {
            get { return true; }
        }

        private AnimatorTrack track { get { return (AnimatorTrack)parent; } }
        private Animator animator { get { return track.animator; } }
        private bool isMasterTrack { get { return track.isMasterTrack; } }

        ///----------------------------------------------------------------------------------------------

        protected override void OnEnter() {

            wasPosition = animator.transform.position;
            wasRotation = animator.transform.rotation;

            if ( !isMasterTrack || startingTransformsMode == StartingTransformsMode.AutoMatchTransforms ) {
                startingPosition = animator.transform.position;
                startingRotation = animator.transform.eulerAngles;
            }

            track.EnableClip(this);
        }

        protected override void OnReverseEnter() { track.EnableClip(this); }

        protected override void OnUpdate(float time, float previousTime) {

            if ( track.useRootMotion && steerLocalRotation != default(Vector2) ) {
                var rot = startingRotation + (Vector3)steerLocalRotation;
                animator.transform.SetLocalEulerAngles(rot);
            }

            var doLerp = time <= blendIn || previousTime <= blendIn;
            if ( doLerp && isMasterTrack && startingTransformsMode == StartingTransformsMode.ManualSetTransforms ) {
                var blend = blendIn > 0 ? ( time / blendIn ) : 1;
                var targetPosition = TransformPosition(startingPosition, (TransformSpace)transformSpace);
                animator.transform.position = Vector3.Lerp(wasPosition, targetPosition, blend);
                var targetRotation = TransformRotation(startingRotation, (TransformSpace)transformSpace);
                animator.transform.rotation = Quaternion.Lerp(wasRotation, targetRotation, blend);
            }

            track.UpdateClip(this, ( time - clipOffset ) * playbackSpeed, ( previousTime - clipOffset ) * playbackSpeed, GetClipWeight(time));
        }

        protected override void OnExit() {
            track.DisableClip(this);
        }

        protected override void OnReverse() {
            animator.transform.position = wasPosition;
            animator.transform.rotation = wasRotation;
            track.DisableClip(this);
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        private bool pendingResample;

        protected override void OnClipGUI(Rect rect) {
            if ( animationClip != null ) {
                EditorTools.DrawLoopedLines(rect, animationClip.length / playbackSpeed, this.length, clipOffset);
            }
        }

        protected override void OnSceneGUI() {

            if ( startingTransformsMode == StartingTransformsMode.ManualSetTransforms ) {
                pendingResample |= SceneGUIUtility.DoVectorPositionHandle(this, (TransformSpace)transformSpace, startingRotation, ref startingPosition);
                pendingResample |= SceneGUIUtility.DoVectorRotationHandle(this, (TransformSpace)transformSpace, startingPosition, ref startingRotation);
            }

            if ( pendingResample && GUIUtility.hotControl == 0 ) {
                pendingResample = false;
                root.ReSample();
            }
        }

#endif

    }
}