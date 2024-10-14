using System;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RZN.SceneUtilities
{

	/// <summary>
	/// 매 씬마다 존재하는 앵커 오브젝트에 부착될 컴포넌트
	/// </summary>
	public class SceneSetter : MonoBehaviour
	{
		void Awake()
		{
			if (string.IsNullOrEmpty(_guid))
			{
				_guid = Guid.NewGuid().ToString();
			}
		}

		[SerializeField][HideInInspector] string _guid = string.Empty;
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(SceneSetter))]
	internal sealed class SceneSetterEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			serializedObject.Update();

			var guidProperty = serializedObject.FindProperty("_guid");

			if (guidProperty == null)
			{
				Debug.LogWarning($"프로퍼티 _guid를 찾을 수 없습니다.");
				return;
			}

			var guidString = !string.IsNullOrEmpty(guidProperty.stringValue) ? guidProperty.stringValue : "Not allocated";

			EditorGUI.BeginDisabledGroup(disabled: true);

			EditorGUILayout.TextField("Guid", guidString);

			EditorGUI.EndDisabledGroup();

			serializedObject.ApplyModifiedProperties();
		}
	}
#endif
}