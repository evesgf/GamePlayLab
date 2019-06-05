#if SLATE_USE_EXPRESSIONS

using System;
using System.Reflection;
using System.Linq;
using UnityEngine;
using StagPoint.Eval;
using Environment = StagPoint.Eval.Environment;

namespace Slate.Expressions {

    ///Helpers for StagPoint.Eval expression environment
    public static class ExpressionsUtility{

		///Wrap an object if wrapper available, within provided environment
		public static void Wrap(object o, Environment env){
			var wrapper = DecoratorFactory.GetDecorator<IExpressionDecorator>(o);
			if (wrapper != null){ wrapper.Wrap(env); }
		}

		///Compile an expression within target environment
		public static void CompileExpression(string scriptExpression, Environment environment, AnimatedParameter animParam, out Exception compileException, out Expression compiledExpression){
			compiledExpression = null;
			compileException = null;
			if (string.IsNullOrEmpty(scriptExpression)){ return; }

			var expectedType = animParam.animatedType;

			try
			{
				compiledExpression = StagPoint.Eval.EvalEngine.Compile(scriptExpression, environment);
				if (compiledExpression.Type == typeof(void)){
					throw new System.Exception( string.Format("Expression must return a value of type '{0}'", expectedType.FriendlyName()) );
				}
				var value = compiledExpression.Execute();
				if (value is object[]){
					var arr = (value as object[]).Cast<float>().ToArray();
					var requiredLength = animParam.parameterModel.RequiredCurvesCount();
					if (arr.Length != requiredLength){
						throw new System.Exception(string.Format("Result Expression Array returns '{0}' values, but '{1}' values are required.", arr.Length, requiredLength ));
					}
				} else {
					var valueType = value.GetType();
					if ( !expectedType.RTIsAssignableFrom(valueType) ){
						throw new System.Exception(string.Format("Result Expression Type is '{0}', but '{1}' is required.", valueType.FriendlyName(), expectedType.FriendlyName()));
					}
				}
			}
			catch (System.Exception exc)
			{
				compileException = exc;
				compiledExpression = null;
			}
		}

		///----------------------------------------------------------------------------------------------

		public static void AddVariable(this Environment environment, string name, object target){
			environment.AddVariable( name, target, target.GetType() );
		}

		public static void AddBoundProperty(this Environment environment, string name, object target, string propertyName){
			AddBoundProperty(environment, name, target, target.GetType().RTGetProperty(propertyName));
		}

		public static void AddBoundProperty(this Environment environment, string name, object target, PropertyInfo property){
			var boundVariable = new BoundVariable( name, target, property );
			environment.AddVariable( boundVariable );
		}

		public static void AddBoundMethod(this Environment environment, string name, object target, string methodName){
			AddBoundMethod(environment, name, target, target.GetType().RTGetMethod(methodName));
		}

		public static void AddBoundMethod(this Environment environment, string name, object target, MethodInfo method ){
			var boundVariable = new BoundVariable( name, target, method );
			environment.AddVariable( boundVariable );
		}
	}
}

#endif