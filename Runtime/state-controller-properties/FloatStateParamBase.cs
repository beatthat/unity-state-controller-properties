using BeatThat.GetComponentsExt;
using BeatThat.Properties;
using System.Collections;
using UnityEngine;
using System;

#if UNITY_EDITOR
using BeatThat.StateControllers.ParamsEditorExtensions;
#endif


namespace BeatThat.StateControllers
{
    /// <summary>
    /// A FloatProperty that binds to a bool parameter on an animator.
    /// </summary>
    public abstract class FloatStateParamBase : FloatProperty, Param, IHasValueChangedEvent<float>
	{
		public abstract string param { get; }

		public Type paramType { get { return typeof(float); } }

		override protected float GetValue()
		{ 
			var s = this.state; 
			return !s.isReady ? m_value : s.GetFloat (this.param);
		}

		override public bool sendsValueObjChanged { get { return true; } }

        override protected void _SetValue(float v) 
		{
			EnsureValue (v);

			if(!this.isActiveAndEnabled) {
				return;
			}

			var s = this.state;
			if(!s.isReady) {
				StopAllCoroutines();
				StartCoroutine(SetValueWhenReady());
				return;
			}

			s.SetFloat(this.param, v, PropertyEventOptions.Force, StateParamOptions.RequireParam); 
		}

		public StateController state { get { return m_state?? (m_state = this.AddIfMissing<StateController, AnimatorController>()); } }
		public StateController m_state;

#if UNITY_EDITOR
		virtual protected void Reset()
		{
			this.ValidateAnimatorControllerParam (createIfMissing:true);
		}
#endif

		private IEnumerator SetValueWhenReady()
		{
			while(!this.state.isReady) {
				yield return new WaitForEndOfFrame();
			}
			SetValue(m_value);
		}

		private float m_valueSet;
	}
}




