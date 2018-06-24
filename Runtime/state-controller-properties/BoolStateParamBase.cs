using BeatThat.GetComponentsExt;
using BeatThat.Properties;
using UnityEngine;
using System.Collections;
using System;


#if UNITY_EDITOR
using BeatThat.StateControllers.ParamsEditorExtensions;
#endif


namespace BeatThat.StateControllers
{

    /// <summary>
    /// A BoolProperty that binds to a bool parameter on an animator.
    /// </summary>
    public abstract class BoolStateParamBase : BoolProperty, Param, IHasValueChangedEvent<bool>
	{
		public abstract string param { get; }

		public Type paramType { get { return typeof(bool); } }

		public StateController m_state;
	
		override protected bool GetValue()
		{
			var s = this.state; 
			if(s == null || !s.isReady) {
				return base.GetValue();
			}

			#if BT_X
			return this.getter();
			#else
			return s.GetBool(this.param);
			#endif 
		}

		override public bool sendsValueObjChanged { get { return true; } }

		// EXPERIMENT: is there a performance improvement using hashed param names with animator?
		#if BT_X
		private Func<bool> getter { get { return m_getter?? (m_getter = this.state.GetterForBool(this.param)); } }
		private Func<bool> m_getter;

		private Action<bool> setter { get { return m_setter?? (m_setter = this.state.SetterForBool(this.param)); } }
		private Action<bool> m_setter;
		#endif

		public void Toggle()
		{
			this.value = !this.value;
		}

		override protected void _SetValue(bool s) 
		{
			EnsureValue (s);

			if(!this.isActiveAndEnabled) {
				return;
			}

			var st = this.state;
			if(!st.isReady) {
				StopAllCoroutines();
				StartCoroutine(SetValueWhenReady());
				return;
			}

			#if BT_X
			this.setter(val);
			#else
			st.SetBool(this.param, s, PropertyEventOptions.Force, StateParamOptions.RequireParam); 
			#endif
		}

		public StateController state { get { return m_state?? (m_state = this.AddIfMissing<StateController, AnimatorController>()); } }
			
		#if UNITY_EDITOR
		virtual protected void Reset()
		{
			this.ValidateAnimatorControllerParam (createIfMissing: true);
		}
		#endif

		private IEnumerator SetValueWhenReady()
		{
			while(!this.state.isReady) {
				yield return new WaitForEndOfFrame();
			}
			SetValue(m_value);
		}
	}
}




