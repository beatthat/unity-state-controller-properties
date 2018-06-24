using BeatThat.TransformPathExt;
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
    /// An IntProperty that binds to a bool parameter on an animator.
    /// </summary>
    public abstract class IntStateParamBase : IntProperty, Param
	{
		public abstract string param { get; }

		public Type paramType { get { return typeof(int); } }

		override protected int GetValue()
		{ 
			var s = this.state; 
			return !s.isReady ? m_value : s.GetInt (this.param);
		}


		#if UNITY_EDITOR
		virtual protected void Reset()
		{
			this.ValidateAnimatorControllerParam (createIfMissing:true);
		}
		#endif

	
		override protected void _SetValue(int val) 
		{
			EnsureValue (val);

			if(!this.isActiveAndEnabled) {
				#if UNITY_EDITOR || DEBUG_UNSTRIP
				if(m_debug) {
					Debug.Log("[" + Time.frameCount + "] " + this.Path() + "-" + GetType() + " set value to " + val + " - CANCEL BECAUSE INACTIVE");
				}
				#endif
				return;
			}

			var s = this.state;
			if(!s.isReady) {
				#if UNITY_EDITOR || DEBUG_UNSTRIP
				if(m_debug) {
					Debug.Log("[" + Time.frameCount + "] " + this.Path() + "-" + GetType() + " set value to " + val + " - START COROUTINE BECAUSE NOT READY TO SET");
				}
				#endif

				StopAllCoroutines();
				StartCoroutine(SetValueWhenReady());
				return;
			}

			#if UNITY_EDITOR || DEBUG_UNSTRIP
			if(m_debug) {
				Debug.Log("[" + Time.frameCount + "] " + this.Path() + "-" + GetType() + " set value to " + val + " - SETTING VALUE");
			}
			#endif

			s.SetInt(this.param, val, PropertyEventOptions.Force, StateParamOptions.RequireParam); 
		}

		public StateController state { get { return m_state?? (m_state = this.AddIfMissing<StateController, AnimatorController>()); } }
		public StateController m_state;

		private IEnumerator SetValueWhenReady()
		{
			while(!this.state.isReady) {
				yield return new WaitForEndOfFrame();
			}
			SetValue(m_value);
		}

	}
}





