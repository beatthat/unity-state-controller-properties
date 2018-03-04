using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;
using System;

#if UNITY_EDITOR
using BeatThat.ParamsEditorExtensions;
#endif

namespace BeatThat
{
	

	/// <summary>
	/// Base class for a (Invocable) component param that exposes 
	/// the (StateController/Animator) param name as a unity-editable property
	/// and sets the default value of that param name to the camel-case version of the class name.
	/// 
	/// So the common usage is to extend this class for each param and use as follows:
	/// 
	/// <code>
	/// public class MyParam1 : TriggerStateProperty {} // so param name will be 'myParam1' by default
	/// 
	/// var someController; // this component has an AnimatorController attached and also an instace of MyParam1
	/// someController.Invoke<MyParam1>(); // extension method from properties pkg, fires the trigger
	/// </code>
	/// </summary>
	public class TriggerStateProperty : TriggerStateParamBase
	{
		[FormerlySerializedAs("m_param")]
		public string m_property;
		public override string param { get { return m_property; } }

		virtual protected string propertyNameDefault 
		{ 
			get { 
				Debug.Log ("[" + Time.frameCount + "] DefaultParamNameRemovingSuffixes=" + this.DefaultParamNameRemovingSuffixes ("Trigger", "Param", "Prop"));
				return this.DefaultParamNameRemovingSuffixes("Trigger", "Param", "Prop");
			}
		}

		virtual protected void Awake()
		{
			if(string.IsNullOrEmpty(this.param)) {
				m_property = this.propertyNameDefault;
			}
		}

		#if UNITY_EDITOR
		override protected void Reset()
		{
			m_property = this.propertyNameDefault;
			base.Reset ();
		}
		#endif

	}

	/// <summary>
	/// Base class for an Invocable component that fires a trigger on a (sibling)StateController/Animator.
	/// </summary>
	public abstract class TriggerStateParamBase : MonoBehaviour, Param, Invocable 
	{
		public bool m_debug;
		public bool m_breakOnInvoke;

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

		virtual protected void Start()
		{
			this.didStart = true;

			if(m_invokePending) {
				Invoke();
			}
		}

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
