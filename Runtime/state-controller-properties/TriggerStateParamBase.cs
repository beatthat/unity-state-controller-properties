using System;
using System.Collections;
using BeatThat.GetComponentsExt;
using BeatThat.OptionalComponents;
using BeatThat.Properties;
using BeatThat.TransformPathExt;
using BeatThat.UnityEvents;
using UnityEngine;


#if UNITY_EDITOR
using BeatThat.StateControllers.ParamsEditorExtensions;
#endif

namespace BeatThat.StateControllers
{


    /// <summary>
    /// Base class for an Invocable component that fires a trigger on a (sibling)StateController/Animator.
    /// </summary>
    public abstract class TriggerStateParamBase : MonoBehaviour, Param, Invocable 
	{
		public bool m_debug;
		public bool m_breakOnInvoke;

		[Tooltip("set TRUE to disable the behaviour that checks/ensures sibling components defined by the [OptionalComponent] attribute.")]
		public bool m_disableEnsureOptionalComponentsOnStart;

		virtual protected void Start()
		{
			HandleOptionalComponents ();

			this.didStart = true;

			if(m_invokePending) {
				Invoke();
			}
		}

		virtual protected void HandleOptionalComponents()
		{
			if (!m_disableEnsureOptionalComponentsOnStart) {
				this.EnsureAllOptionalComponents ();
			}
		}

		public ComponentEvent onInvoked 
		{ 
			get { return m_onInvoked?? (m_onInvoked = new ComponentEvent()); } 
			set { m_onInvoked = value; } 
		}
		[SerializeField]private ComponentEvent m_onInvoked;


		#region Invocable implementation
		public void Invoke ()
		{
			#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
			if(m_debug) {
				Debug.Log("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::Invoke param='" + this.param + "'");
			}
			#endif

			#if UNITY_EDITOR
			if(m_breakOnInvoke) {
				Debug.LogWarning("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + " BREAK ON INVOKE");
				Debug.Break();
			}
			#endif
				
			if(!this.isActiveAndEnabled) {
				m_invokePending = true;
				return;
			}

			var s = this.state;
			if(!s.isReady) {
				#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
				if(m_debug) {
					Debug.LogWarning("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::Invoke param='" + this.param + "' NOT READY!");
				}
				#endif

				StopAllCoroutines();
				StartCoroutine(InvokeWhenReady());

				return;
			}

			s.SetTrigger(this.param, PropertyEventOptions.Force, StateParamOptions.RequireParam); 

			if(m_onInvoked != null) {
				m_onInvoked.Invoke(this);
			}
		}
		#endregion

		public void Clear()
		{
			#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
			if(m_debug) {
				Debug.Log("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::Clear param='" + this.param + "'");
			}
			#endif

			m_invokePending = false;

			if(!this.isActiveAndEnabled) {
				return;
			}

			StopAllCoroutines();

			var s = this.state;
			if(!s.isReady) {
				#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
				if(m_debug) {
					Debug.LogWarning("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::Clear param='" + this.param + "' NOT READY!");
				}
				#endif

				// if the controller (animator) is not ready, shouldn't need to schedule a clear; will be clear by default on init
				return;
			}

			s.ClearTrigger(this.param, PropertyEventOptions.Force, StateParamOptions.DontRequireParam); 
		}

		public abstract string param { get; }

		public Type paramType { get { return typeof(Invocable); } }

		public StateController state { get { return m_state?? (m_state = this.AddIfMissing<StateController, AnimatorController>()); } }
		public StateController m_state;


		private bool didStart { get; set; }



		#if UNITY_EDITOR
		virtual protected void Reset()
		{
			this.ValidateAnimatorControllerParam (true);
		}
		#endif


		void OnDisable()
		{
			m_invokePending = false;
		}

		void OnEnable()
		{
			if(!this.didStart) {
				return;
			}

		}

		private bool m_invokePending;

		private IEnumerator InvokeWhenReady()
		{
			while(!this.state.isReady) {
				yield return new WaitForEndOfFrame();
			}
			Invoke();
		}
	}
}





