using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;
using UnityEngine.Events;

namespace BeatThat
{
	public class BoolStateProperty : BoolStateParamBase
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
	public abstract class BoolStateParamBase : HasBool, Param, IHasValueChangedEvent<bool>
	{
		public UnityEvent<bool> onValueChanged 
		{ 
			get { return m_valueChanged?? (m_valueChanged = new BoolEvent()); } 
			set { m_valueChanged = value; } 
		}
		[SerializeField]private UnityEvent<bool> m_valueChanged;

		public abstract string param { get; }

		public StateController m_state;

		[Tooltip("set FALSE if you want a param to hold its value across disable/enable")]
		public bool m_resetValueOnDisable = true;

		[FormerlySerializedAs("m_value")]
		[Tooltip("the prop will resume this value OnDisable (when resetDefaultValueOnDisable is set)")]
		public bool m_resetValue;

		[Tooltip("The value that should be synced to the state controller (Animator)")]
		public bool m_value;

		public bool m_debug;
		public bool m_debugBreakOnSetValue;

		override public bool value
		{ 
			get { 
				var s = this.state; 
				if(s == null || !s.isReady) {
					return m_value;
				}

				#if BT_X
				return this.getter();
				#else
				return s.GetBool(this.param);
				#endif
			} 
			set {
				SetValue(value);
			} 
		}

		override public object valueObj { get { return this.value; } }

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

		public void SetValue(bool val) 
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

		
			#if BT_X
			this.setter(val);
			#else
			s.SetBool(this.param, val, PropertyEventOptions.Force, StateParamOptions.RequireParam); 
			#endif

			m_valueSet = val;

			if(changed && m_valueChanged != null) {
				SendValueObjChanged();
				m_valueChanged.Invoke(val);
			}

			#if UNITY_EDITOR
			if(m_debugBreakOnSetValue) {
				Debug.LogWarning("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::set_value to " + val 
					+ " BREAK ON SET VALUE is enabled (changed=" + changed + ")");
				Debug.Break();
			}
			#endif
		}

		public StateController state { get { return m_state?? (m_state = this.AddIfMissing<StateController, AnimatorController>()); } }
			
		void Start()
		{
			this.didStart = true;
			SetValue(m_value);
		}

		private bool didStart { get; set; }
	
		void OnDisable()
		{
			if(m_resetValueOnDisable) {
				m_value = m_resetValue;
			}
		}

		void OnEnable()
		{
			if(!this.didStart) {
				return;
			}

			SetValue(m_value);

			if(m_debug) {
				Debug.Log("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::OnEnable");
			}
		}


		void OnDestroy()
		{
			#if BT_DEBUG_UNSTRIP || UNITY_EDITOR
			if(m_debug) {
				Debug.Log("[" + Time.frameCount + "][" + this.Path() + "] " + GetType() + "::OnDestroy");
			}
			#endif
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

		private bool m_valueSet;
	}
}
