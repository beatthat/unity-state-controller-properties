using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
using BeatThat.ParamsEditorExtensions;
#endif

namespace BeatThat
{
	[CustomEditor(typeof(TriggerStateParamBase), true)]
	[CanEditMultipleObjects]
	public class TriggerStateParamEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			var param = this.target as TriggerStateParamBase;
			var so = this.serializedObject;
			var paramNameProp = so.FindProperty("m_property");

			if (paramNameProp != null)
			{
				param.HandleParamUndefinedInController(paramNameProp);
			}
			else
			{
				param.WarnIfParamUndefined();
			}
				
			if (!Application.isPlaying)
			{
				so.ApplyModifiedProperties();
				base.OnInspectorGUI();
				return;
			}	

			so.ApplyModifiedProperties();

			OnTriggerStateParamGUI();
		}

		virtual protected void OnTriggerStateParamGUI() 
		{
//			var param = this.target as TriggerStateParamBase;
//			param.ResetPropertyToDefaultIfEmpty(this.serializedObject);
//			param.WarnIfParamUndefined();
//
//			base.OnInspectorGUI();
			DrawInvokeButton();
		}

		protected void DrawInvokeButton()
		{
			if(!Application.isPlaying) {
				return;
			}
			using(var s = new EditorGUILayout.HorizontalScope()) {
				if(GUILayout.Button(new GUIContent("Invoke", "Invoke this trigger"))) {
					(this.target as Invocable).Invoke();
				}
			}
		}
	}
}