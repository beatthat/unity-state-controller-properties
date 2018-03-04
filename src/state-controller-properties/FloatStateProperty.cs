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

		override protected void Awake()
		{
			base.Awake ();
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
}