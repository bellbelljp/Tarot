using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Tarot
{
	public class SaveLoadSlot : MonoBehaviour
	{
		[SerializeField] TextMeshProUGUI m_dateText = null;
		[SerializeField] TextMeshProUGUI m_charaText = null;
		[SerializeField] TextMeshProUGUI m_talkText = null;
		[SerializeField] Image m_screenShot = null;
		[SerializeField] Sprite m_autoSaveImg = null;
		SaveData.Data m_saveData = null;
		public int Index = 0;

		public async void SetParam(int index, SaveData.Data saveData, List<StoryMaster.Data> storyData, string charaName)
		{
			m_saveData = saveData;
			Index = index;
			// セーブデータが空の場合
			if (m_saveData == null)
			{
				SetParamNull(Index);
				return;
			}

			var story = storyData.Find(it => it.Id == m_saveData.Phase);
			story.Talk = story.Talk.ReplaceName();
			var talkText = RemoveAtFromText(story.Talk);
			m_talkText.text = talkText;
			m_charaText.text = charaName;
			var slotNo = string.Empty;
			if (Index == 0)
			{
				//var text = await Language.Common.Auto.Localize("Common");
				//slotNo = text;
				slotNo = "Auto";	// tmp
				m_screenShot.sprite = m_autoSaveImg;
			}
			else
			{
				slotNo = string.Format("No.{0}", Index);
				if (m_saveData.ScreenshotName != string.Empty)
					m_screenShot.sprite = await SaveData.Instance.LoadScreenshotAsync(m_saveData.ScreenshotName);
				else
					m_screenShot.sprite = null;
			}
			m_dateText.text = string.Format("{0}:{1}", slotNo, m_saveData.GetDateTime());
		}

		/// <summary>セーブデータが空の場合</summary>
		public void SetParamNull(int index)
		{
			Index = index;
			m_saveData = null;
			m_dateText.text = string.Format("No.{0}", index); ;
			m_talkText.text = string.Empty;
			m_charaText.text = string.Empty;
			m_screenShot.sprite = null;
		}

		/// <summary>セーブデータが存在していればTRUE</summary>
		public bool HasSaveData()
		{
			return m_saveData != null;
		}

		/// <summary>セーブデータをロード</summary>
		public void Load()
		{
			// オートセーブに今のセーブデータを上書き
			SaveData.Instance.Save(m_saveData);
			SaveData.Instance.Logs.Clear();
		}

		/// <summary>@を取り除く</summary>
		string RemoveAtFromText(string talk)
		{
			// 選択肢の場合
			if (talk.Count('@') > 1)
			{
				// ","で分割
				string[] arr = talk.Split(',');
				for (int i = 0; i < arr.Length; i++)
				{
					var str = arr[i];
					arr[i] = str.Substring(str.IndexOf("@") + 1);
				}
				var text = string.Format("{0}, {1}", arr[0], arr[1]);
				return text;
			}
			else
			{
				var talkText = string.Empty;
				// 台詞遷移
				if (talk.Contains('＠'))
				{
					Debug.LogWarning("全角の＠が含まれてるよ");
					return string.Empty;
				}
				else if (talk.Contains('@'))
				{
					return talk.Substring(talk.IndexOf('@') + 1);
				}
			}
			return talk;
		}
	}
}
