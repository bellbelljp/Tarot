using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace JourneysOfRealPeople
{
	public class UIGameViewLogWindow : MonoBehaviour
	{
		public class Log
		{
			public string LogText = string.Empty;
			public string NameText = string.Empty;
			public string[] SelectText = new string[2]{ string.Empty, string.Empty };
		}

		[SerializeField] GameObject m_logObj = null;
		[SerializeField] Transform m_logPos = null;
		[SerializeField] ScrollRect m_scroll = null;
		[SerializeField] UITransition m_bgTransition = null;
		[SerializeField] ButtonEx m_closeButton = null;

		GameObject[] m_logItems = new GameObject[MAX_LOG];

		public const int MAX_LOG = 20;

		private void Awake()
		{
			CreateItems();
		}

		void CreateItems()
		{
			if (m_logItems[0] != null)
				return;

			for (int i = 0; i < m_logItems.Length; i++)
			{
				var obj = Instantiate(m_logObj, m_logPos);
				m_logItems[i] = obj;
			}
		}

		public async void ShowLogs()
		{
			m_closeButton.enabled = false;
			m_bgTransition.Canvas.alpha = 0f;
			// アイテム作成
			if (m_logItems[0] == null)
				CreateItems();

			// データ数だけ表示
			for(int i = 0; i < m_logItems.Length; i++)
			{
				var hasData = SaveData.Instance.Logs.Count > i;
				if(m_logItems[i] != null)
					m_logItems[i].SetActive(hasData);
			}

			Queue<Log> newLogDatas = new Queue<Log>(SaveData.Instance.Logs);
			int j = 0;
			while(newLogDatas.Count != 0)
			{
				// 先頭のデータを取り出す
				var data = newLogDatas.Dequeue();
				var obj = m_logItems[j];
				var logItem = obj.GetComponent<UIGameViewLogItem>();
				logItem.SetParam(data);
				j++;
			}

			// 次のフレームまで待機
			await UniTask.Yield();

			m_closeButton.enabled = true;
			m_scroll.verticalNormalizedPosition = 0f;

			await m_bgTransition.TransitionInWait();
		}

		public async void Close()
		{
			m_closeButton.enabled = false;
			await m_bgTransition.TransitionOutWait();
			gameObject.SetActive(false);
		}
	}
}
