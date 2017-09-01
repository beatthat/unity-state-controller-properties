
namespace BeatThat
{
	/// <summary>
	/// Base class for bool state params that can get their value by binding to an BoolProp
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
