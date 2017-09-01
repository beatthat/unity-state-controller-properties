#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace BeatThat
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

			foreach(var ap in controller.parameters) {
				if(ap.name == p.param) {
					return;
				}
			}
			EditorGUILayout.HelpBox("Param '" + p.param + "' is not defined in sibling Animator", MessageType.Warning);
		}
	}
}
#endif