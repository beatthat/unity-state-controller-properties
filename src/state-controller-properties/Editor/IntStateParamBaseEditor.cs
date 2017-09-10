using UnityEditor;
using UnityEngine;

namespace BeatThat
{
	[CustomEditor(typeof(IntStateParamBase), true)]
	[CanEditMultipleObjects]
	public class IntStateParamBaseEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			var param = this.target as IntStateParamBase;
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