using UnityEditor;
using UnityEngine;

namespace BeatThat
{
	[CustomEditor(typeof(BoolStateParamBase), true)]
	[CanEditMultipleObjects]
	public class BoolStateParamBaseEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			var param = this.target as BoolStateParamBase;
			param.ResetPropertyToDefaultIfEmpty(this.serializedObject);
			param.WarnIfParamUndefined();

			if(!Application.isPlaying) {
				base.OnInspectorGUI();
				return;
			}

			var valBefore = param.m_value;
			base.OnInspectorGUI();
			if(valBefore != param.m_value) {
				param.SetValue(param.m_value);
			}
		}
	}
}