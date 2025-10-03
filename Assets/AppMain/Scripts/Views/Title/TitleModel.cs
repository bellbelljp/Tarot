namespace Tarot
{
	public class TitleModel
	{

		public float GetVolume()
		{
			return SoundManager.Instance.BgmVolume;
		}

		public void ChangeBGMVolume(float value)
		{
			SoundManager.Instance.SetBGMVolume(value);
		}
	}
}
