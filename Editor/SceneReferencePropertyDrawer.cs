#if UNITY_EDITOR

using System;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace RZN.SceneUtilities.Editors
{
	// 내가 짠 것 아님
	[CustomPropertyDrawer(typeof(SceneReference))]
	[CanEditMultipleObjects]
	internal class SceneReferencePropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var isDirtyProperty = property.FindPropertyRelative("_isDirty");

			if (isDirtyProperty.boolValue)
			{
				isDirtyProperty.boolValue = false;
				// 강제로 프로퍼티를 dirty로 만든다.
				// 사용자가 세이브를 하면, 사라짐
			}

			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			const float buildSettingsWidth = 20f;
			const float padding = 2f;

			var assetPos = position;
			assetPos.width -= buildSettingsWidth + padding;

			var buildSettingsPos = position;
			buildSettingsPos.x += position.width - buildSettingsWidth + padding;
			buildSettingsPos.width = buildSettingsWidth;

			var sceneAssetProperty = property.FindPropertyRelative("_sceneAsset");
			var hadReference = sceneAssetProperty.objectReferenceValue != null;

			EditorGUI.PropertyField(assetPos, sceneAssetProperty, new GUIContent());

			var indexInSettings = -1;

			if (sceneAssetProperty.objectReferenceValue)
			{
				if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(sceneAssetProperty.objectReferenceValue, out string guid, out long localId))
				{
					indexInSettings = Array.FindIndex(EditorBuildSettings.scenes, s => s.guid.ToString() == guid);
				}
				else if (hadReference)
				{
					property.FindPropertyRelative("_scenePath").stringValue = string.Empty;
				}

				var settingsContent = indexInSettings != -1
					? new GUIContent("-", "씬이 이미 에디터 빌드 세팅에 존재합니다. 여기를 눌러 삭제합니다.")
					: new GUIContent("+", "씬이 에디터 빌드 세팅에 없습니다. 여기를 눌러 추가합니다.");

				var prevBackgroundColor = GUI.backgroundColor;

				GUI.backgroundColor = indexInSettings != -1 ? Color.red : Color.green;

				if (GUI.Button(buildSettingsPos, settingsContent, EditorStyles.miniButtonRight) && sceneAssetProperty.objectReferenceValue)
				{
					if (indexInSettings != -1)
					{
						var scenes = EditorBuildSettings.scenes.ToList();
						scenes.RemoveAt(indexInSettings);
						EditorBuildSettings.scenes = scenes.ToArray();
					}
					else
					{
						var newScenes = new EditorBuildSettingsScene[]
						{
								new (AssetDatabase.GetAssetPath(sceneAssetProperty.objectReferenceValue), true)
						};

						EditorBuildSettings.scenes = EditorBuildSettings.scenes.Concat(newScenes).ToArray();
					}
				}

				GUI.backgroundColor = prevBackgroundColor;
				EditorGUI.EndProperty();
			}
		}
	}
}
#endif