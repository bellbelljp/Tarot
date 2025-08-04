using UnityEngine;

namespace Tarot
{
	public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{
		private static T m_instance;
		public static T Instance
		{
			get
			{
				if (m_instance == null)
					SetupInstance();
				return m_instance;
			}
		}

		public Singleton()
		{
			m_instance = (T)this;
		}

		private void Awake()
		{
			if (m_instance == null)
			{
				m_instance = (T)this;
				DontDestroyOnLoad(this.gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
		}

		static void SetupInstance()
		{
			m_instance = FindObjectOfType<T>();

			if (m_instance == null)
			{
				GameObject obj = new GameObject();
				m_instance = obj.AddComponent<T>();
				DontDestroyOnLoad(obj);
			}
		}
	}
}
