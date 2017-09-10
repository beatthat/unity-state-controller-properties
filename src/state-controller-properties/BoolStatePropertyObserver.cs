
namespace BeatThat
{
	/// <summary>
	/// Base class for bool state params that can get their value by binding to an BoolProp.
	/// Usually you would extend BoolStatePropertyObserver instead of this class;
	/// use this class when you DON'T want to use the 
	/// 'param name is a public property w same name as class' convention
	/// </summary>
	public abstract class BoolStatePropObserver : BoolStateParamBase
	{
		public BoolProp m_driver;
		private BindBoolToBool m_binding;

		void Awake()
		{
			if(m_driver != null) {
				m_binding = this.gameObject.AddComponent<BindBoolToBool>();
				m_binding.m_defaultValue = m_resetValue;
				m_binding.ConfigureDriver(m_driver);
				m_binding.property = this;
			}
		}

	}

	/// <summary>
	/// Base class for a BoolStateProperty that can get its value by binding to a BoolProp
	/// and that uses the default 'param name is a public property w same name as class' behaviour 
	/// from BoolStateProperty
	/// </code>
	/// </summary>
	public class BoolStatePropertyObserver : BoolStateProperty
	{
		public BoolProp m_driver;
		private BindBoolToBool m_binding;

		override protected void Awake()
		{
			base.Awake();

			if(m_driver != null) {
				InitWithDriver(m_driver);
			}
		}

		public void InitWithDriver(BoolProp driver)
		{
			m_driver = driver;
			m_binding = this.gameObject.AddComponent<BindBoolToBool>();
			m_binding.m_defaultValue = m_resetValue;
			m_binding.ConfigureDriver(m_driver);
			m_binding.property = this;
		}

	}
}
