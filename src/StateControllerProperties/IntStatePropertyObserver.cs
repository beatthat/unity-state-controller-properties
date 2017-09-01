
namespace BeatThat
{
	/// <summary>
	/// Base class for int state params that can get their value by binding to an IntProp
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
