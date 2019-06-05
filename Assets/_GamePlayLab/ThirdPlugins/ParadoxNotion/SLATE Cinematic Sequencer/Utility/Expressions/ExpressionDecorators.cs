#if SLATE_USE_EXPRESSIONS

using System.Linq;
using UnityEngine;
using Environment = StagPoint.Eval.Environment;

namespace Slate.Expressions {

    ///Decorator interface
    public interface IExpressionDecorator : IDecorator{
		void Wrap(Environment env);
	}

	///Global Wrapper
	public static partial class GlobalEnvironment{
		private static Environment env;

		[System.ComponentModel.Browsable(false)]
		public static Environment Get(){ return env; }

		static GlobalEnvironment(){
			env = new Environment();
			env.AddConstant("Slate",		 typeof(GlobalEnvironment));
			env.AddConstant("Mathf",		 typeof(Mathf));
			env.AddConstant("Vector2",		 typeof(Vector2));
			env.AddConstant("Vector3",		 typeof(Vector3));
			env.AddConstant("Vector4",		 typeof(Vector4));
			env.AddConstant("Color",		 typeof(Color));
			env.AddConstant("EaseType",		 typeof(EaseType));
		}

		
		public static Transform FindGameObject(string name){ return GameObject.Find(name).transform; }

		public static float Lerp(float a, float b, float t){ return Mathf.Lerp(a, b, t); }
		public static Vector2 Lerp(Vector2 a, Vector2 b, float t){ return Vector2.Lerp(a, b, t); }
		public static Vector3 Lerp(Vector3 a, Vector3 b, float t){ return Vector3.Lerp(a, b, t); }
		public static Color Lerp(Color a, Color b, float t){ return Color.Lerp(a, b, t); }

		public static float Ease(float from, float to, float t, EaseType type){	return Easing.Ease(type, from, to, t); }
		public static Vector2 Ease(Vector2 from, Vector2 to, float t, EaseType type){ return Easing.Ease(type, from, to, t); }
		public static Vector3 Ease(Vector3 from, Vector3 to, float t, EaseType type){	return Easing.Ease(type, from, to, t); }
		public static Color Ease(Color from, Color to, float t, EaseType type){	return Easing.Ease(type, from, to, t); }

		public static float Noise(float freq, float mag, float t){ return Mathf.PerlinNoise(t * freq, 0) * mag; }
		public static Vector2 Noise(float freq, Vector2 mag, float t){ return new Vector2(Noise(freq, mag.x, t), Noise(freq, mag.y, t * 2f)); }
		public static Vector3 Noise(float freq, Vector3 mag, float t){ return new Vector3(Noise(freq, mag.x, t), Noise(freq, mag.y, t * 2f), Noise(freq, mag.z, t * 3f)); }
		public static Color Noise(float freq, Vector4 mag, float t){ return new Color(Noise(freq, mag.x, t), Noise(freq, mag.y, t * 2f), Noise(freq, mag.z, t * 3f), Noise(freq, mag.w, t * 4f)); }

		public static float Sin(float f){ return Mathf.Sin(f); }
	}

	///Cutscene Wrapper
	[Decorator(typeof(Cutscene))]
	[Category("Cutscene")]
	public partial class ExpressionCutsceneWrapper : IExpressionDecorator{
		private Cutscene target;
        object IDecorator.Target { get {return target;} set {target = (Cutscene)value;} }
        void IExpressionDecorator.Wrap(Environment env) {
			env.AddVariable("Cutscene", this);
        }

		public float time{ get {return target.currentTime;} }
		public float timeNorm{ get {return time/length;} }
		public float length{get {return target.length;}}
		public static IDirectableCamera directorCamera{get {return DirectorCamera.current;}}
		public static IDirectableCamera gameCamera{get {return DirectorCamera.gameCamera;}}

		public Transform FindActor(string name){ return target.groups.FirstOrDefault(g => g.name == name).actor.transform; }
    }

	///Clip Wrapper
	[Decorator(typeof(ActionClip))]
	[Category("Clip")]
	public partial class ExpressionActionClipWrapper : IExpressionDecorator{
		private ActionClip target;
		object IDecorator.Target { get {return target;} set {target = (ActionClip)value;} }
		void IExpressionDecorator.Wrap(Environment env){
			env.AddVariable("Clip", this);
		}

		public float time {get {return target.RootToLocalTime();}}
		public float timeNorm {get {return time/length;}}
		public float length{ get {return target.length;}}
		public float weight{ get {return target.GetWeight();} }
		public float blendIn{ get {return target.blendIn;} }
		public float blendOut{ get {return target.blendOut;} }
		public Transform actor{	get {return target.actor.transform;} }

	}

	///Parameter Wrapper
	[Decorator(typeof(AnimatedParameter))]
	[Category("Parameter")]
	public partial class ExpressionParameterWrapper : IExpressionDecorator{
		private AnimatedParameter target;
        object IDecorator.Target { get {return target;} set { target = (AnimatedParameter)value;} }
        void IExpressionDecorator.Wrap(Environment env) {
			
			if (target.animatedType == typeof(bool)){ env.AddBoundProperty("value", this, "value_bool"); }
			if (target.animatedType == typeof(int)){ env.AddBoundProperty("value", this, "value_int"); }
			if (target.animatedType == typeof(float)){ env.AddBoundProperty("value", this, "value_float"); }
			if (target.animatedType == typeof(Vector2)){ env.AddBoundProperty("value", this, "value_vector2"); }
			if (target.animatedType == typeof(Vector3)){ env.AddBoundProperty("value", this, "value_vector3"); }
			if (target.animatedType == typeof(Color)){ env.AddBoundProperty("value", this, "value_color"); }

			if (target.animatedType == typeof(bool)){ env.AddBoundMethod("Eval", this, "Eval_Bool"); }
			if (target.animatedType == typeof(int)){ env.AddBoundMethod("Eval", this, "Eval_Int"); }
			if (target.animatedType == typeof(float)){ env.AddBoundMethod("Eval", this, "Eval_Float"); }
			if (target.animatedType == typeof(Vector2)){ env.AddBoundMethod("Eval", this, "Eval_Vector2"); }
			if (target.animatedType == typeof(Vector3)){ env.AddBoundMethod("Eval", this, "Eval_Vector3"); }
			if (target.animatedType == typeof(Color)){ env.AddBoundMethod("Eval", this, "Eval_Color"); }

			env.AddVariable("Parameter", this);
        }

		bool value_bool{ get {return (bool)target.GetCurrentValueAsObject();} }
        int value_int{ get {return (int)target.GetCurrentValueAsObject();} }
		float value_float{ get {return (float)target.GetCurrentValueAsObject();} }
		Vector2 value_vector2{ get {return (Vector2)target.GetCurrentValueAsObject();} }
		Vector3 value_vector3{ get {return (Vector3)target.GetCurrentValueAsObject();} }
		Color value_color{ get {return (Color)target.GetCurrentValueAsObject();} }

		bool Eval_Bool(float time){ return (bool)target.GetEvalValue( time ); }
		int Eval_Int(float time){ return (int)target.GetEvalValue( time ); }
		float Eval_Float(float time){ return (float)target.GetEvalValue( time ); }
		Vector2 Eval_Vector2(float time){ return (Vector2)target.GetEvalValue( time ); }
		Vector3 Eval_Vector3(float time){ return (Vector3)target.GetEvalValue( time ); }
		Color Eval_Color(float time){ return (Color)target.GetEvalValue( time ); }
    }
}

#endif