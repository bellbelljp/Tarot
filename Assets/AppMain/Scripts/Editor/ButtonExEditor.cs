using UnityEditor;

namespace Tarot
{
	[CustomEditor(typeof(ButtonEx))]
	public class ButtonExEditor : UnityEditor.UI.ButtonEditor
	{
		SerializedProperty m_isPinterDownText;
		SerializedProperty m_text;
		SerializedProperty m_animImageProp;
		SerializedProperty m_typeProp;

		protected override void OnEnable()
		{
			base.OnEnable();
			// ButtonExクラスのm_animImageプロパティの参照を保持
			m_animImageProp = serializedObject.FindProperty("m_animImage");
			m_typeProp = serializedObject.FindProperty("m_type");
			m_isPinterDownText = serializedObject.FindProperty("m_isPinterDownText");
			m_text = serializedObject.FindProperty("m_text");
		}

		public override void OnInspectorGUI()
		{
			// シリアル化されたオブジェクトを更新して最新の値を取得
			serializedObject.Update();
			// m_animImageをインスペクタに表示
			EditorGUILayout.PropertyField(m_animImageProp);
			EditorGUILayout.PropertyField(m_typeProp);
			EditorGUILayout.PropertyField(m_isPinterDownText);
			EditorGUILayout.PropertyField(m_text);
			// 変更された値をオブジェクトに適用
			serializedObject.ApplyModifiedProperties();

			base.OnInspectorGUI();
		}
	}
}
