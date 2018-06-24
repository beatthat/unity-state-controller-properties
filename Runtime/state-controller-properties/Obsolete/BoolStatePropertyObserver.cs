using BeatThat.Properties;
using System;


namespace BeatThat.StateControllers
{
	[Obsolete("functionality moved to base class BoolStateParamBase")]
	/// <summary>
	/// Base class for bool state params that can get their value by binding to an BoolProp.
	/// Usually you would extend BoolStatePropertyObserver instead of this class;
	/// use this class when you DON'T want to use the 
	/// 'param name is a public property w same name as class' convention
	/// </summary>
	public abstract class BoolStatePropObserver : BoolStateParamBase
	{
		override protected void Awake()
		{
			this.bindOrDrivePropertyOptions = BindOrDrivePropertyOptions.BindToProperty;
			base.Awake ();
		}

	}

	[Obsolete("functionality moved to base class BoolStateParamBase")]
	/// <summary>
	/// Base class for a BoolStateProperty that can get its value by binding to a BoolProp
	/// and that uses the default 'param name is a public property w same name as class' behaviour 
	/// from BoolStateProperty
	/// </code>
	/// </summary>
	public class BoolStatePropertyObserver : BoolStateProperty
	{
		override protected void Awake()
		{
			this.bindOrDrivePropertyOptions = BindOrDrivePropertyOptions.BindToProperty;
			base.Awake ();
		}
	}
}

