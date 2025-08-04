using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Tarot
{
	public class SelectButtonDialog : MonoBehaviour
	{
		[SerializeField] UITransition m_bgTransition = null;
		[SerializeField] Transform m_buttonParent = null;
		[SerializeField] SelectButton m_buttonPrefab = null;

		int m_response = -1;
		int m_index = -1;
		List<SelectButton> m_buttons = new List<SelectButton>();
		CancellationTokenSource m_token = new CancellationTokenSource();

		/// <summary>ボタンの生成</summary>
		public async UniTask<(int id, int index)> CreateButtons(bool bgOpen, string[] selectParams, CancellationToken token)
		{
			if (selectParams == null || selectParams.Length == 0)
				return (-1, -1);

			m_buttons = new List<SelectButton>();
			try
			{
				var tasks = new List<UniTask>();
				int selectId = 0;

				if (bgOpen == true)
				{
					m_bgTransition.gameObject.SetActive(true);
					tasks.Add(m_bgTransition.TransitionInWait(token));
				}
				else
				{
					m_bgTransition.gameObject.SetActive(false);
				}

				var index = 0;
				// ボタンの生成
				foreach (var param in selectParams)
				{
					selectId = int.Parse(param.Substring(0, param.IndexOf("@")));
					var text = param.Substring(param.IndexOf("@") + 1);
					var button = Instantiate(m_buttonPrefab, m_buttonParent);
					m_buttons.Add(button);
					var task = button.OnCreated(text, selectId, OnAnyButtonClicked, index, token);
					tasks.Add(task);
					index++;
				}

				// レイアウトグループの確実な反映のためにキャンバスを更新
				Canvas.ForceUpdateCanvases();

				await UniTask.WhenAll(tasks);

				// すでにキャンセルされているなら例外を投げる
				token.ThrowIfCancellationRequested();
				// 複数のTokenを合成
				var tokens = CancellationTokenSource.CreateLinkedTokenSource(new[] { token, this.GetCancellationTokenOnDestroy() });

				// 何かのボタンが押されるまで待つ
				await UniTask.WaitUntil(() => m_response != -1, PlayerLoopTiming.Update, tokens.Token);
				var res = m_response;
				// 閉じる
				await Close();
				return (res, m_index);
			}
			catch(OperationCanceledException e)
			{
				Debug.Log("キャンセルCrateButtons：" + e);
				throw e;
			}
		}

		/// <summary>ダイアログを閉じる</summary>
		public async UniTask Close()
		{
			var tasks = new List<UniTask>();
			foreach(var b in m_buttons)
			{
				tasks.Add(b.Close(m_token));
			}
			// 背景を閉じる
			if(m_bgTransition.gameObject.activeSelf == true)
			{
				tasks.Add(m_bgTransition.TransitionOutWait());
			}

			await UniTask.WhenAll(tasks);
			m_bgTransition.gameObject.SetActive(false);
			m_response = -1;
		}

		/// <summary>どれかのボタンを押したとき</summary>
		void OnAnyButtonClicked(int selectedId, int index)
		{
			m_response = selectedId;
			m_index = index;
		}
	}
}
