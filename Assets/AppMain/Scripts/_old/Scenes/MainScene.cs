using UnityEngine;

namespace Tarot
{
	public class MainScene : SceneBase
	{
		public enum View
		{
			Select,
			Profile,
		}
		public const string Select = "UISelectView";
		public const string Profile = "UIProfileView";
		public const string Dating = "UIDatingView";
		public const string Selemony = "UISelemonyView";
		public const string Ending = "UIEndingView";
		public async void MoveToView(string name)
		{
			//await ChangeView(name, 0);
		}

		public async void MoveToView(View name)
		{
			//await ChangeView((string)name, 0);
		}
	}
}
