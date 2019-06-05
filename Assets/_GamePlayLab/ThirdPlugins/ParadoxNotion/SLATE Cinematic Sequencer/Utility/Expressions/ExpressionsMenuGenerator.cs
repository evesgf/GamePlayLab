#if UNITY_EDITOR && SLATE_USE_EXPRESSIONS

using UnityEngine;
using UnityEditor;
using System.Reflection;
using StagPoint.Eval;

namespace Slate.Expressions {

    public static class ExpressionsMenuGenerator {

		///Creates and returns a menu of available options in target expression enviroment
		public static GenericMenu GetExpressionEnvironmentMenu(Environment env, System.Action<string> callback){
			var menu = new GenericMenu();
			while (env != null){

				foreach(var pair in env.Constants){
					if (!(pair.Value is System.Type)){
						var category = "Constants";
						var name = pair.Key;
						menu.AddItem( new GUIContent(category + "/" + name), false, ()=>{ callback(name); } );
					}
				}

				foreach(var pair in env.Variables){

					if (pair.Value is BoundVariable){
						var boundVariable = (BoundVariable)pair.Value;
						var targetType = boundVariable.Target.GetType();
						var categoryAtt = targetType.RTGetAttribute<CategoryAttribute>(true);
						var category = categoryAtt != null? categoryAtt.category : targetType.Name;
						var name = pair.Key;
						menu.AddItem( new GUIContent(category + "/" + name), false, ()=>{ callback(name); } );
						continue;
					}

					var variable = pair.Value as Variable;
					if (variable == null){
						continue;
					}

					var target = variable.Value;
					var type = target is System.Type? (System.Type)target : variable.Type;
					var flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
					if ( !(target is System.Type)){ flags |= BindingFlags.Instance; }
					foreach(var m in type.GetMembers(flags) ){
						
						var browsable = m.RTGetAttribute<System.ComponentModel.BrowsableAttribute>(true);
						if (browsable != null && browsable.Browsable == false){
							continue;
						}

						var category = target is System.Type? "CodeBase/" + pair.Key : pair.Key;
						var name = m.Name;
						if (m is PropertyInfo || m is FieldInfo){
							var template = pair.Key + "." + name;
							menu.AddItem( new GUIContent(category + "/" + name), false, ()=>{ callback(template); } );
							continue;
						}

						if (m is MethodInfo && !((MethodInfo)m).IsSpecialName){
							var method = (MethodInfo)m;
							if (method.ReturnType != typeof(void)){
								var template = name;
								template += "(";
								var parameters = method.GetParameters();
								for (var i = 0; i < parameters.Length; i++){
									var parameter = parameters[i];
									template += parameter.Name;
									if (i != parameters.Length - 1){
										template += ", ";
									}
								}
								template += ")";
								template = pair.Key + "." + template;
								menu.AddItem( new GUIContent(category +"/" + name), false, ()=>{ callback(template); } );
							}
						}

					}							
				}

				//continue with parent
				env = env.Parent;
			}

			return menu;
		}
	}
}

#endif
