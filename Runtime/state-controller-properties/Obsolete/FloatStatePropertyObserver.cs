using BeatThat.Properties;
using System;


namespace BeatThat.StateControllers
{
	/// <summary>
	/// Base class for float state params that can get their value by binding to an FloatProp.
	/// Usually you would extend FloatStatePropertyObserver instead of this class;
	/// use this class when you DON'T want to use the 
	/// 'param name is a public property w same name as class' convention
	/// </summary>
	[Obsolete("functionality moved to base class")]
	public abstract class FloatStatePropObserver : FloatStateParamBase
	{
		public FloatProp m_driver;
		private BindFloatToFloat m_binding;

		override protected void Awake()
		{
			base.bindOrDrivePropertyOptions = BindOrDrivePropertyOptions.BindToProperty;
			base.Awake ();
		}

	}

	/// <summary>
	/// Base class for a FloatStateProperty that can get its value by binding to a FloatProp
	/// and that uses the default 'param name is a public property w same name as class' behaviour 
	/// from FloatStateProperty
	/// </code>
	/// </summary>
	[Obsolete("functionality moved to base class")]
	public class FloatStatePropertyObserver : FloatStateProperty
	{
		override protected void Awake()
		{
			base.bindOrDrivePropertyOptions = BindOrDrivePropertyOptions.BindToProperty;
			base.Awake ();
		}
	}
}

