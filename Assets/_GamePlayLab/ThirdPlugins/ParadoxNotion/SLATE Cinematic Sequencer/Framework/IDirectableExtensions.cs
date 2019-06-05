using System;
using System.Linq;
using UnityEngine;

namespace Slate
{

    public static class IDirectableExtensions
    {

        ///The length of the directable
        public static float GetLength(this IDirectable directable) {
            return directable.endTime - directable.startTime;
        }

        ///The local current time of the directable
        public static float RootToLocalTime(this IDirectable directable) {
            return directable.RootToLocalTime(directable.root.currentTime);
        }

        ///The local time based on provided root time
        public static float RootToLocalTime(this IDirectable directable, float time) {
            return Mathf.Clamp(time - directable.startTime, 0, directable.GetLength());
        }

        ///The local current time of the directable
        public static float RootToLocalTimeUnclamped(this IDirectable directable) {
            return directable.RootToLocalTimeUnclamped(directable.root.currentTime);
        }

        ///The local time based on provided root time
        public static float RootToLocalTimeUnclamped(this IDirectable directable, float time) {
            return time - directable.startTime;
        }

        ///Is root current time within directable range?
        public static bool RootTimeWithinRange(this IDirectable directable) {
            return directable.root.currentTime >= directable.startTime && directable.root.currentTime <= directable.endTime && directable.root.currentTime > 0;
        }

        ///Should two provided directables be able to cross-blend?
        public static bool CanCrossBlend(this IDirectable directable, IDirectable other) {
            if ( directable == null || other == null ) { return false; }
            if ( ( directable.canCrossBlend || other.canCrossBlend ) && directable.GetType() == other.GetType() ) {
                return true;
            }
            return false;
        }

        ///----------------------------------------------------------------------------------------------

        ///Utility to check if delta (time - previous time) are close enough to trigger something that should only trigger when they are
        public static bool WithinBufferTriggerRange(this IDirectable directable, float time, float previousTime, bool bypass = false) {
            if ( directable.root.isReSampleFrame ) { return false; }
            return ( time - previousTime ) <= ( 0.1f * directable.root.playbackSpeed ) || bypass;
        }

        ///----------------------------------------------------------------------------------------------

        ///Returns the first child directable of provided name
        public static IDirectable FindChild(this IDirectable directable, string name) {
            if ( directable.children == null ) { return null; }
            return directable.children.FirstOrDefault(d => d.name.ToLower() == name.ToLower());
        }

        ///Returns the previous sibling in the parent (eg previous clip)
        public static T GetPreviousSibling<T>(this IDirectable directable) where T : IDirectable { return (T)GetPreviousSibling(directable); }
        public static IDirectable GetPreviousSibling(this IDirectable directable) {
            if ( directable.parent != null ) {
                return directable.parent.children.LastOrDefault(d => d != directable && d.startTime < directable.startTime);
            }
            return null;
        }

        ///Returns the next sibling in the parent (eg next clip)
        public static T GetNextSibling<T>(this IDirectable directable) where T : IDirectable { return (T)GetNextSibling(directable); }
        public static IDirectable GetNextSibling(this IDirectable directable) {
            if ( directable.parent != null ) {
                return directable.parent.children.FirstOrDefault(d => d != directable && d.startTime > directable.startTime);
            }
            return null;
        }

        ///Going upwards, returns the first parent of type T
        public static T GetFirstParentOfType<T>(this IDirectable directable) where T : IDirectable {
            var current = directable.parent;
            while ( current != null ) {
                if ( current is T ) {
                    return (T)current;
                }
                current = current.parent;
            }
            return default(T);
        }

        ///----------------------------------------------------------------------------------------------

        ///The current weight based on blend properties and based on root current time.
        public static float GetWeight(this IDirectable directable) { return GetWeight(directable, RootToLocalTime(directable)); }
        ///The weight at specified local time based on its blend properties.
        public static float GetWeight(this IDirectable directable, float time) { return GetWeight(directable, time, directable.blendIn, directable.blendOut); }
        ///The weight at specified local time based on provided override blend in/out properties
        public static float GetWeight(this IDirectable directable, float time, float blendInOut) { return GetWeight(directable, time, blendInOut, blendInOut); }
        public static float GetWeight(this IDirectable directable, float time, float blendIn, float blendOut) {
            var length = GetLength(directable);
            if ( time <= 0 ) {
                return blendIn <= 0 ? 1 : 0;
            }

            if ( time >= length ) {
                return blendOut <= 0 ? 1 : 0;
            }

            if ( time < blendIn ) {
                return time / blendIn;
            }

            if ( time > length - blendOut ) {
                return ( length - time ) / blendOut;
            }

            return 1;
        }

        ///----------------------------------------------------------------------------------------------

        ///Returns the transform object used for specified Space transformations. Null if World Space.
        public static Transform GetSpaceTransform(this IDirectable directable, TransformSpace space, GameObject actorOverride = null) {

            if ( space == TransformSpace.CutsceneSpace ) {
                return directable.root != null ? directable.root.context.transform : null;
            }

            var actor = actorOverride != null ? actorOverride : directable.actor;

            if ( actor != null ) {
                if ( space == TransformSpace.ActorSpace ) {
                    return actor != null ? actor.transform : null;
                }

                if ( space == TransformSpace.ParentSpace ) {
                    return actor != null ? actor.transform.parent : null;
                }
            }

            return null; //world space
        }

        ///Transforms a point in specified space
        public static Vector3 TransformPosition(this IDirectable directable, Vector3 point, TransformSpace space) {
            var t = directable.GetSpaceTransform(space);
            return t != null ? t.TransformPoint(point) : point;
        }

        ///Inverse Transforms a point in specified space
        public static Vector3 InverseTransformPosition(this IDirectable directable, Vector3 point, TransformSpace space) {
            var t = directable.GetSpaceTransform(space);
            return t != null ? t.InverseTransformPoint(point) : point;
        }

        public static Quaternion TransformRotation(this IDirectable directable, Vector3 euler, TransformSpace space) {
            var t = directable.GetSpaceTransform(space);
            if ( t != null ) { return t.rotation * Quaternion.Euler(euler); }
            return Quaternion.Euler(euler);
        }

        public static Vector3 InverseTransformRotation(this IDirectable directable, Quaternion rot, TransformSpace space) {
            var t = directable.GetSpaceTransform(space);
            if ( t != null ) { return ( Quaternion.Inverse(t.rotation) * rot ).eulerAngles; }
            return rot.eulerAngles;
        }

        ///Returns the final actor position in specified Space (InverseTransform Space)
        public static Vector3 ActorPositionInSpace(this IDirectable directable, TransformSpace space) {
            return directable.actor != null ? directable.InverseTransformPosition(directable.actor.transform.position, space) : directable.root.context.transform.position;
        }

        ///----------------------------------------------------------------------------------------------

#if SLATE_USE_EXPRESSIONS

		public static StagPoint.Eval.Environment CreateExpressionEnvironment(this IDirector director){
			var env = Slate.Expressions.GlobalEnvironment.Get().Push();
			Slate.Expressions.ExpressionsUtility.Wrap(director, env);
			return env;			
		}

		public static StagPoint.Eval.Environment GetExpressionEnvironment(this IDirectable directable){
			var env = directable.root.CreateExpressionEnvironment().Push();
			Slate.Expressions.ExpressionsUtility.Wrap(directable, env);
			return env;
		}

#endif

        ///----------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        //Try record keys
        public static bool TryAutoKey(this IKeyable keyable, float time) {

            if ( Application.isPlaying || keyable.root.isReSampleFrame ) {
                return false;
            }

            if ( !Prefs.autoKey || GUIUtility.hotControl != 0 ) {
                return false;
            }

            var case1 = ReferenceEquals(CutsceneUtility.selectedObject, keyable);
            var activeTransform = UnityEditor.Selection.activeTransform;
            var actor = keyable.actor;
            var case2 = actor != null && activeTransform != null && activeTransform.IsChildOf(actor.transform);
            if ( case1 || case2 ) { return keyable.animationData.TryAutoKey(time); }
            return false;
        }

        ///Try add parameter in animation data of keyable
        public static bool TryAddParameter(this IKeyable keyable, Type type, string memberPath, string transformPath) {
            return keyable.animationData.TryAddParameter(keyable, type, memberPath, transformPath);
        }
#endif

    }
}