#if UNITY_EDITOR

using UnityEditor;

namespace RZN.SceneUtilities.Editors
{

	[CustomEditor(typeof(SceneDependencyData))]
	internal sealed class SceneDependencyDataEditor : Editor
	{

		#region UnityCallbacks

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			base.OnInspectorGUI();
		}

		#endregion // UnityCallbacks

	}

}

#endif