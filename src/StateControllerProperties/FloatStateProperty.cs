using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;
using UnityEngine.Events;

namespace BeatThat
{
	/// <summary>
	/// Base class for a float state param that exposes the (StateController/Animator) param name as a unity-editable property
	/// and sets the default value of that param name to the camel-case version of the class name.
	/// 
	/// So the common usage is to extend this class for each param and use as follows:
	/// 
	/// <code>
	/// public class MyParam1 : FloatStateProperty {} // so param name will be 'myParam1' by default
	/// 
	/// var someController; // this component has an AnimatorController attached and also an instace of MyParam1
	/// someController.SetFloat<MyParam1>(1.234f); // extension setters/getters from properties pkg
	/// </code>
	/// </summary>
	public class FloatStateProperty : FloatStateParamBase
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
	/// Base class for a HasFloat component that gets and sets a param on a (sibling)StateController/Animator.
	/// </summary>
	public abstract class FloatStateParamBase : HasFloat, Param, IHasValueChangedEvent<float>
	{
		public UnityEvent<float> onValueChanged 
		{ 
			get { return m_valueChanged?? (m_valueChanged = new FloatEvent()); } 
			set { m_valueChanged = value; } 
		}
		[SerializeField]private UnityEvent<float> m_valueChanged;

		public abstract string param { get; }

		[Tooltip("set FALSE if you want a param to hold its value across disable/enable")]
		public bool m_resetValueOnDisable = true;

		[FormerlySerializedAs("m_value")]
		[Tooltip("the prop will resume this value OnDisable (when resetDefaultValueOnDisable is set)")]
		public int m_resetValue;

		[Tooltip("The value that should be synced to the state controller (Animator)")]
		public float m_value;
		public bool m_debug;
		public bool m_debugBreakOnSetValue;

		override public float value
		{ 
			get { 
				var s = this.state; 
				return !s.isReady ? m_value : s.GetFloat (this.param);
			} 
			set {
				SetValue(value);
			} 
		}

		override public bool sendsValueObjChanged { get { return true; } }

		public void SetValue(float val) 
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

			bool changed = !Mathf.Approximately(m_value, m_valueSet);// (m_value != m_valueSet);

			s.SetFloat(this.param, val, PropertyEventOptions.Force, StateParamOptions.RequireParam); 
			m_valueSet = val;

			if(changed && m_valueChanged != null) {
				SendValueObjChanged();
				m_valueChanged.Invoke(val);
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

		void OnDisable()
		{
			if(m_resetValueOnDisable) {
				m_value = m_resetValue;
			}
		}

		void OnDidApplyAnimationProperties()
		{
			#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
			if(m_debug) {
				Debug.Log("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::OnDidApplyAnimationProperties");
			}
			#endif
			SetValue(m_value);
		}

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
