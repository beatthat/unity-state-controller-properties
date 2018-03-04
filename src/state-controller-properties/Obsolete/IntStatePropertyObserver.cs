using System;


namespace BeatThat
{
	/// <summary>
	/// Base class for bool state params that can get their value by binding to an bool param on a StateController/Animator.
	/// Usually you would extend IntStatePropertyObserver instead of this class;
	/// use this class when you DON'T want to use the
	/// 'param name is a public property w same name as class' convention
	/// </summary>
	[Obsolete("behaviour moved to base class")]
	public abstract class IntStatePropObserver : IntStateParamBase
	{
		public IntProp m_driver;
		private BindIntToInt m_binding;

		override protected void Awake()
		{
			this.enablePropertyBinding = true;
			base.Awake();
		}

	}

	// <summary>
	/// Base class for a BoolStateProperty that can get its value by binding to a BoolProp
	/// and that uses the default 'param name is a public property w same name as class' behaviour 
	/// from BoolStateProperty
	/// </code>
	/// </summary>
	[Obsolete("behaviour moved to base class")]
	public class IntStatePropertyObserver : IntStateProperty
	{
		override protected void Awake()
		{
			this.enablePropertyBinding = true;
			base.Awake();
		}
	}
}
