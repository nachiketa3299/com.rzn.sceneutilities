#if UNITY_EDITOR

using System.Collections.Generic;
using System.Reflection;

using UnityEditor;

namespace RZN.SceneUtilities.Editors
{

	[CustomEditor(typeof(SceneLoadManager))]
	internal sealed class SceneLoadManagerEditor : Editor
	{
		public override bool RequiresConstantRepaint() => true;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var sceneLoadManager = target as SceneLoadManager;

			var loadedScenesByGameObjects = sceneLoadManager.GetType()
				.GetField("_loadedScenesByGameObjects", BindingFlags.NonPublic | BindingFlags.Instance)
				.GetValue(sceneLoadManager)
				as Dictionary<SceneLoadTrigger, HashSet<string>>;

			EditorGUILayout.LabelField("Requested Game Objects by SceneLoadTriggers ", EditorStyles.boldLabel);

			EditorGUI.BeginDisabledGroup(disabled: true);

			foreach (var sceneLoadTrigger in loadedScenesByGameObjects.Keys)
			{
				EditorGUILayout.BeginVertical(EditorStyles.helpBox);

				EditorGUILayout.ObjectField(sceneLoadTrigger, typeof(SceneLoadTrigger), true);

				EditorGUI.indentLevel++;

				foreach (var sceneName in loadedScenesByGameObjects[sceneLoadTrigger])
				{
					EditorGUILayout.LabelField(sceneName, EditorStyles.miniLabel);
				}

				EditorGUI.indentLevel--;

				EditorGUILayout.EndVertical();
				EditorGUILayout.Space();
			}

			EditorGUI.EndDisabledGroup();

			EditorGUILayout.LabelField("Loading Scenes", EditorStyles.boldLabel);

			var loadingScenes = sceneLoadManager.GetType()
				.GetField("_loadingScenes", BindingFlags.NonPublic | BindingFlags.Instance)
				.GetValue(sceneLoadManager)
				as HashSet<string>;

			EditorGUI.indentLevel++;
			foreach (var sceneName in loadingScenes)
			{
				EditorGUILayout.LabelField(sceneName, EditorStyles.miniLabel);
			}
			EditorGUI.indentLevel--;

			EditorGUILayout.LabelField("Unloading Scenes", EditorStyles.boldLabel);

			var unloadingScenes = sceneLoadManager.GetType()
				.GetField("_unloadingScenes", BindingFlags.NonPublic | BindingFlags.Instance)
				.GetValue(sceneLoadManager)
				as HashSet<string>;

			EditorGUI.indentLevel++;
			foreach (var sceneName in unloadingScenes)
			{
				EditorGUILayout.LabelField(sceneName, EditorStyles.miniLabel);
			}
			EditorGUI.indentLevel--;
		}
	}
}

#endif
