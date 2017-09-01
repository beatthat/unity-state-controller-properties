using UnityEditor;
using UnityEngine;

namespace BeatThat
{
	[CustomEditor(typeof(FloatStateParamBase), true)]
	[CanEditMultipleObjects]
	public class FloatStateParamBaseEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			var param = this.target as FloatStateParamBase;
			param.ResetPropertyToDefaultIfEmpty(this.serializedObject);
			param.WarnIfParamUndefined();

			if(!Application.isPlaying) {
				base.OnInspectorGUI();
				return;
			}

			var valBefore = param.m_value;
			base.OnInspectorGUI();
			if(!Mathf.Approximately(valBefore, param.m_value)) {
				param.SetValue(param.m_value);
			}
		}
	}
}