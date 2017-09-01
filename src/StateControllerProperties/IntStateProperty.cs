using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;
using UnityEngine.Events;

namespace BeatThat
{
	/// <summary>
	/// Base class for a (HasFloat) int state param that exposes 
	/// the (StateController/Animator) param name as a unity-editable property
	/// and sets the default value of that param name to the camel-case version of the class name.
	/// 
	/// So the common usage is to extend this class for each param and use as follows:
	/// 
	/// <code>
	/// public class MyParam1 : IntStateProperty {} // so param name will be 'myParam1' by default
	/// 
	/// var someController; // this component has an AnimatorController attached and also an instace of MyParam1
	/// someController.SetFloat<MyParam1>(1234); // extension setters/getters from properties pkg
	/// </code>
	/// </summary>
	public class IntStateProperty : IntStateParamBase
	{
		[FormerlySerializedAs("m_param")]
		public string m_property;
		public override string param { get { return m_property; } }

		virtual protected string propertyNameDefault 
		{ 
			get { 
				return this.DefaultParamName();
			}
		}

		virtual protected void Awake()
		{
			if(string.IsNullOrEmpty(this.param)) {
				m_property = this.propertyNameDefault;
			}
		}

		#if UNITY_EDITOR
		void Reset()
		{
			m_property = this.propertyNameDefault;
		}
		#endif
	}
		
	/// <summary>
	/// Base class for a HasInt component that gets and sets a param on a (sibling)StateController/Animator.
	/// </summary>
	public abstract class IntStateParamBase : HasInt, Param, IHasValueChangedEvent<int>
	{
		public UnityEvent<int> onValueChanged 
		{ 
			get { return m_valueChanged?? (m_valueChanged = new IntEvent()); } 
			set { m_valueChanged = value; } 
		}
		[SerializeField]private UnityEvent<int> m_valueChanged;

		public abstract string param { get; }

		[Tooltip("set FALSE if you want a param to hold its value across disable/enable")]
		public bool m_resetValueOnDisable = true;

		[FormerlySerializedAs("m_value")]
		[Tooltip("the prop will resume this value OnDisable (when resetDefaultValueOnDisable is set)")]
		public int m_resetValue;

		[Tooltip("The value that should be synced to the state controller (Animator)")]
		public int m_value;

		public bool m_debug;
		public bool m_debugBreakOnSetValue;

		override public int value
		{ 
			get { 
				var s = this.state; 
				return !s.isReady ? m_value : s.GetInt (this.param);
			} 
			set {
				SetValue(value);
			} 
		}

		override public object valueObj { get { return this.value; } }

		override public bool sendsValueObjChanged { get { return true; } }

		void OnDisable()
		{
			if(m_resetValueOnDisable) {
				m_value = m_resetValue;
			}
		}
	
		public void SetValue(int val) 
		{
			#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
			if(m_debug) {
				Debug.Log("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::set_value to " + val + " param='" + this.param + "'");
			}
			#endif

			m_value = val;

			if(!this.isActiveAndEnabled) {
				return;
			}

			var s = this.state;
			if(!s.isReady) {
				StopAllCoroutines();
				StartCoroutine(SetValueWhenReady());
				return;
			}

			bool changed = (m_value != m_valueSet);

			s.SetInt(this.param, val, PropertyEventOptions.Force, StateParamOptions.RequireParam); 
			m_valueSet = val;

			if(changed) {
				SendValueObjChanged();
				if(m_valueChanged != null) {
					m_valueChanged.Invoke(val);
				}
			}

			#if UNITY_EDITOR
			if(m_debugBreakOnSetValue) {
				Debug.LogWarning("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::set_value to " + val + " BREAK ON SET VALUE is enabled");
				Debug.Break();
			}
			#endif
		}

		public StateController state { get { return m_state?? (m_state = this.AddIfMissing<StateController, AnimatorController>()); } }
		public StateController m_state;

		void Start()
		{
			this.didStart = true;
			SetValue(m_value);
		}

		private bool didStart { get; set; }

		void OnEnable()
		{
			if(!this.didStart) {
				return;
			}

			SetValue(m_value);
			#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
			if(m_debug) {
				Debug.LogWarning("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::OnEnable");
			}
			#endif
		}

		void OnDidApplyAnimationProperties()
		{
			this.enabled = true;
			SetValue(m_value);
		}

		private IEnumerator SetValueWhenReady()
		{
			while(!this.state.isReady) {
				yield return new WaitForEndOfFrame();
			}
			SetValue(m_value);
		}

		private int m_valueSet;

	}
}
