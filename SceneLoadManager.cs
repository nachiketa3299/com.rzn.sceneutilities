using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RZN.SceneUtilities
{

	/// <summary>
	/// 여러 오브젝트들에 의해 로드/언로드되어야 하는 씬들을 관리
	/// </summary>
	[DisallowMultipleComponent]
	public class SceneLoadManager : MonoBehaviour
	{

		#region UnityCallbacks

		void Awake() => Instance = this;

		void Update()
		{
			if (!_isDirty)
			{
				return;
			}

			// 최종적으로 로드 / 언로드 되어야 할 씬 계산

			var allRequestedScenes = _loadedScenesByGameObjects.Values.SelectMany(_ => _).ToHashSet();
			var allCurrentScenes = SceneUtilities.RetrieveAllLoadedSceneNames(_sceneDependencyData.PersistentSceneName);

			var toLoadScenesQueue = allRequestedScenes
				.Except(allCurrentScenes)
				.Except(_loadingScenes)
				.ToHashSet();

			var toUnloadScenesQueue = allCurrentScenes
				.Except(allRequestedScenes)
				.Except(_unloadingScenes)
				.ToHashSet();

			foreach (var toLoad in toLoadScenesQueue)
			{
				_loadingScenes.Add(toLoad);

				SceneManager.LoadSceneAsync(toLoad, LoadSceneMode.Additive).completed += (AsyncOperation _) => { OnSceneLoadingComplete(toLoad); };
			}

			foreach (var toUnload in toUnloadScenesQueue)
			{
				_unloadingScenes.Add(toUnload);

				// 이게 *가끔* Null이 되는 경우가 있는데 왜 그런지 모르겠음
				SceneManager.UnloadSceneAsync(toUnload).completed += (AsyncOperation _) => { OnSceneUnloadingComplete(toUnload); };
			}

			_isDirty = false;
		}


		#endregion // UnityCallbacks

		public static SceneLoadManager Instance { get; private set; }

		void OnSceneLoadingComplete(string name) => _loadingScenes.Remove(name);

		void OnSceneUnloadingComplete(string name) => _unloadingScenes.Remove(name);

		/// <summary>
		/// <see cref="SceneLoadTrigger"/>를 가진 물체가 씬의 로딩 박스에 인접했을 때 호출
		/// </summary>
		public void Entered(SceneLoadTrigger entering, string sceneName, int depthToLoad)
		{
			var toAdd = RetrieveNearSceneNames(sceneName, depthToLoad).ToHashSet();

			if (!_loadedScenesByGameObjects.ContainsKey(entering))
			{
				_loadedScenesByGameObjects.Add(entering, toAdd);
			}
			else
			{
				_loadedScenesByGameObjects[entering] = toAdd;
			}

			_isDirty = true;
		}

		/// <summary>
		/// <see cref="SceneLoadTrigger"/>를 가진 물체가 어떤 이유에서든 비활성화 되었을 때 호출
		/// </summary>
		public void Disabled(SceneLoadTrigger dd) => _isDirty = _loadedScenesByGameObjects.Remove(dd);
		public IReadOnlyCollection<string> RetrieveNearSceneNames(string pivotSceneName, int depthToLoad) => _sceneDependencyData.RetrieveNearSceneUniqueNames(pivotSceneName, depthToLoad);

		bool _isDirty = false;
		HashSet<string> _loadingScenes = new();
		HashSet<string> _unloadingScenes = new();
		Dictionary<SceneLoadTrigger, HashSet<string>> _loadedScenesByGameObjects = new();
		[SerializeField] SceneDependencyData _sceneDependencyData;

	}

}