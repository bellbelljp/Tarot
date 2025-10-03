using System;
using System.Threading;
using UnityEngine;

namespace Tarot
{
	public class TitlePresenter
	{
		ITitleView m_view = null;
		TitleModel m_model = null;

		bool m_isMoving = false;

		public TitlePresenter(ITitleView view, TitleModel model)
		{
			m_view = view;
			m_model = model;
			m_isMoving = false;
		}

		public void StartGame(SceneBase scene, CancellationToken token)
		{
			if (m_isMoving)
				return;

			m_isMoving = true;

			ChangeViewAsync(scene, token);
		}

		public float GetVolume()
		{
			return m_model.GetVolume();
		}

		public void ChangeBGMVolume(float value)
		{
			m_model.ChangeBGMVolume(value);
		}

		async void ChangeViewAsync(SceneBase scene, CancellationToken token)
		{
			try
			{
				await scene.ChangeView(ViewName.Genre, 0, token);
			}
			catch (OperationCanceledException)
			{
				// キャンセルされた場合は何もしない
			}
			catch (Exception ex)
			{
				Debug.LogError($"ビュー変更中にエラーが発生しました: {ex.Message}");
			}
			finally
			{
				m_isMoving = false;
			}
		}
	}
}
