using UnityEditor;
using UnityEngine;

namespace BeatThat
{
	[CustomEditor(typeof(TriggerStateParamBase), true)]
	[CanEditMultipleObjects]
	public class TriggerStateParamEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			OnTriggerStateParamGUI();
		}

		virtual protected void OnTriggerStateParamGUI() 
		{
			var param = this.target as TriggerStateParamBase;
			param.ResetPropertyToDefaultIfEmpty(this.serializedObject);
			param.WarnIfParamUndefined();

			base.OnInspectorGUI();
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