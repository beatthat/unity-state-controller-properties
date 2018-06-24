using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
#endif

namespace BeatThat.StateControllers
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

}


