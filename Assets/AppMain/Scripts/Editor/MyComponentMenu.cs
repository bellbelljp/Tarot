using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace JourneysOfRealPeople
{
	public class MyComponentMenu
	{
		[MenuItem("GameObject/UI/ButtonEx", false, 10)]
		static void CreateButtonEx(MenuCommand menuCommand)
		{
			GameObject obj = new GameObject("ButtonEx");
			obj.AddComponent<Image>();
			obj.AddComponent<ButtonEx>();

			GameObjectUtility.SetParentAndAlign(obj, menuCommand.context as GameObject);
			Undo.RegisterCreatedObjectUndo(obj, "Create" + obj.name);
			Selection.activeObject = obj;
		}
	}
}
