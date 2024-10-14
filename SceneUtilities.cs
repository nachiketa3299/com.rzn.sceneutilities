using System.Collections.Generic;

using UnityEngine.SceneManagement;

namespace RZN.SceneUtilities
{
	public static class SceneUtilities
	{
		/// <summary>
		/// 현재 로드되어 있는 모든 씬을 리스트로 만들어 반환한다.
		/// </summary>
		public static IReadOnlyCollection<string> RetrieveAllLoadedSceneNames(string toIgnore)
		{
			var allLoadedSceneNames = new List<string>();

			for (var i = 0; i < SceneManager.sceneCount; ++i)
			{
				var loadedSceneName = SceneManager.GetSceneAt(i).name;

				if (loadedSceneName == toIgnore)
				{
					continue;
				}

				allLoadedSceneNames.Add(loadedSceneName);
			}

			return allLoadedSceneNames.AsReadOnly();
		}
	}

}