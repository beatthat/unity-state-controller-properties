using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;
using UnityEngine.Events;
using System;

#if UNITY_EDITOR
using BeatThat.ParamsEditorExtensions;
#endif

namespace BeatThat
{
	/// <summary>
	/// Base class for a HasFloat component that gets and sets a param on a (sibling)StateController/Animator.
	/// </summary>
	public abstract class FloatStateParamBase : HasFloat, Param, IHasValueChangedEvent<float>
	{
		[Tooltip("set TRUE to bind this param to some other property (source value)")]
		[HideInInspector]public bool m_enablePropertyBinding;
		public bool enablePropertyBinding { get { return m_enablePropertyBinding; } set { m_enablePropertyBinding = value; } }
		[HideInInspector]public FloatProp m_bindToProperty;
		private bool hasConnectedBinding;
		virtual protected void Awake()
		{
			if (!this.enablePropertyBinding || this.hasConnectedBinding) {
				return;
			}

			if (BindFloatToFloat.Connect<BindFloatToFloat> (m_bindToProperty, this, m_resetValue)) {
				this.hasConnectedBinding = true;
				return;
			}

			#if UNITY_EDITOR || DEBUG_UNSTRIP
			Debug.LogWarning("[" + Time.frameCount + "] failed to bind to prop at " + GetType() + "[" + this.Path() + "]");
			#endif
		}

		public UnityEvent<float> onValueChanged 
		{ 
			get { return m_valueChanged?? (m_valueChanged = new FloatEvent()); } 
			set { m_valueChanged = value; } 
		}
		[SerializeField]private UnityEvent<float> m_valueChanged;

		public abstract string param { get; }

		public Type paramType { get { return typeof(float); } }

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

		#if UNITY_EDITOR
		virtual protected void Reset()
		{
			this.ValidateAnimatorControllerParam (createIfMissing:true);
		}
		#endif

		override protected void Start()
		{
			base.Start ();
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
