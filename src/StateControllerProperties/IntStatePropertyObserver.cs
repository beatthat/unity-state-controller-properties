
namespace BeatThat
{
	/// <summary>
	/// Base class for bool state params that can get their value by binding to an bool param on a StateController/Animator.
	/// Usually you would extend IntStatePropertyObserver instead of this class;
	/// use this class when you DON'T want to use the
	/// 'param name is a public property w same name as class' convention
	/// </summary>
	public abstract class IntStatePropObserver : IntStateParamBase
	{
		public IntProp m_driver;
		private BindIntToInt m_binding;

		void Awake()
		{
			if(m_driver != null) {
				m_binding = this.gameObject.AddComponent<BindIntToInt>();
				m_binding.m_defaultValue = m_resetValue;
				m_binding.ConfigureDriver(m_driver);
				m_binding.property = this;
			}
		}

	}

	// <summary>
	/// Base class for a BoolStateProperty that can get its value by binding to a BoolProp
	/// and that uses the default 'param name is a public property w same name as class' behaviour 
	/// from BoolStateProperty
	/// </code>
	/// </summary>
	public class IntStatePropertyObserver : IntStateProperty
	{
		public IntProp m_driver;
		private BindIntToInt m_binding;

		override protected void Awake()
		{
			base.Awake();

			if(m_driver != null) {
				InitWithDriver(m_driver);	
			}
		}

		public void InitWithDriver(IntProp driver)
		{
			m_driver = driver;
			m_binding = this.gameObject.AddComponent<BindIntToInt>();
			m_binding.m_defaultValue = m_resetValue;
			m_binding.ConfigureDriver(m_driver);
			m_binding.property = this;
		}

	}
}
