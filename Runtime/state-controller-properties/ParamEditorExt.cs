using BeatThat.Controllers;
using BeatThat.Properties;
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System;

namespace BeatThat.StateControllers.ParamsEditorExtensions
{
	public static class ParamEditorExt 
	{
		public static void ResetPropertyToDefaultIfEmpty(this Param p, SerializedObject so)
		{
			var propField = so.FindProperty("m_property");
			if(propField != null && string.IsNullOrEmpty(propField.stringValue)) {
				propField.stringValue = p.DefaultParamName();
				so.ApplyModifiedProperties();
			}
		}

        public static void ResetPropertyToDefaultIfEmpty(this Param p, SerializedProperty serializedProp)
        {
            if(serializedProp == null)
            {
                return;
            }

            if(!string.IsNullOrEmpty(serializedProp.stringValue))
            {
                return;
            }

            serializedProp.stringValue = p.DefaultParamName();
        }


        public static void WarnIfParamUndefined(this Param p)
		{
			var animator = (p as Component).GetComponent<Animator>();
			if(animator == null) {
				return;
			}

			var controller = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
			if(controller == null) {
				return;
			}

            if(IsParamDefinedInController(p.param, controller))
            {
                return;
            }
			EditorGUILayout.HelpBox("Param '" + p.param + "' is not defined in sibling Animator", MessageType.Warning);
        }

        public static void HandleParamUndefinedInController(this Param p, SerializedProperty paramNameProp)
        {
            p.ResetPropertyToDefaultIfEmpty(paramNameProp);

            var animator = (p as Component).GetComponent<Animator>();
            if (animator == null)
            {
                return;
            }

            var controller = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            if (controller == null)
            {
                return;
            }

            if (IsParamDefinedInController(p.param, controller))
            {
                return;
            }
            EditorGUILayout.HelpBox("Param '" + p.param + "' is not defined in sibling Animator", MessageType.Warning);

            var saveColor = GUI.backgroundColor;
            
            var defaultParamName = p.DefaultParamName();
			if (IsParamDefinedInController (defaultParamName, controller)) {
				GUI.backgroundColor = Color.yellow;

				if (GUILayout.Button ("Fix as " + defaultParamName)) {
					paramNameProp.stringValue = defaultParamName;
				}
			} else if (GUILayout.Button ("Add param '" + defaultParamName + "' to animator controller")) {
				p.ValidateAnimatorControllerParam ();	
			}

            GUI.backgroundColor = saveColor;
        }

		/// <summary>
		/// Check whether the a given AnimatorController parameter associated 
		/// </summary>
		/// <returns><c>true</c>, if animator controller parameter was validated, <c>false</c> otherwise.</returns>
		/// <param name="createIfMissing">If set to <c>true</c> and the param is missing from the AnimatorController, creates it.</param>
		public static bool ValidateAnimatorControllerParam(this Param p, bool createIfMissing = true)
		{
			UnityEditor.Animations.AnimatorController controller;
			return p.GetAnimatorController (out controller) 
				&& ValidateAnimatorControllerParam (controller, p.param, p.paramType, createIfMissing);
		}

		/// <summary>
		/// Check whether the a given AnimatorController parameter associated 
		/// </summary>
		/// <returns><c>true</c>, if animator controller parameter was validated, <c>false</c> otherwise.</returns>
		/// <param name="createIfMissing">If set to <c>true</c> and the param is missing from the AnimatorController, creates it.</param>
		public static bool ValidateAnimatorControllerParam(UnityEditor.Animations.AnimatorController controller,
			string paramName, Type paramType, bool createIfMissing = true)
		{
			if(controller == null) {
				return false;
			}

			if (IsParamDefinedInController(paramName, controller))
			{
				return true;
			}

			if (!createIfMissing) {
				return false;
			}

			var addParam = new AnimatorControllerParameter ();

			if (typeof(bool).IsAssignableFrom (paramType)) {
				addParam.type = AnimatorControllerParameterType.Bool;
			} else if (typeof(int).IsAssignableFrom (paramType)) {
				addParam.type = AnimatorControllerParameterType.Int;
			} else if (typeof(float).IsAssignableFrom (paramType)) {
				addParam.type = AnimatorControllerParameterType.Float;
			} else if (typeof(Invocable).IsAssignableFrom (paramType)) {
				addParam.type = AnimatorControllerParameterType.Trigger;
			} else {
				return false;
			}

			addParam.name = paramName;
			controller.AddParameter (addParam);

			return true;
		}

		public static bool GetAnimatorController(this Param p, out UnityEditor.Animations.AnimatorController controller)
		{
			return GetAnimatorController (p as Component, out controller);
		}

		public static bool GetAnimatorController(Component c, out UnityEditor.Animations.AnimatorController controller)
		{
			var animator = c.GetComponent<Animator>();
			if (animator == null)
			{
				controller = null;
				return false;
			}

			return (controller = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController) != null;
		}

		public static bool IsParamDefinedInController(this Param p)
		{
			UnityEditor.Animations.AnimatorController controller;
			if(!p.GetAnimatorController(out controller)) {
				return false;
			}
			return IsParamDefinedInController(p.param, controller);
		}

        public static bool IsParamDefinedInController(string name, UnityEditor.Animations.AnimatorController controller)
        {
            if(controller == null)
            {
                return false;
            }

            foreach (var ap in controller.parameters)
            {
                if (ap.name == name)
                {
                    return true;
                }
            }

            return false;
        }
	}
}
#endif

