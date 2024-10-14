using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace RZN.SceneUtilities
{
	/// <summary>
	/// 씬들의 상호 인접 관계를 정의하는 데이터.
	/// </summary>
	[CreateAssetMenu(menuName = "RZN.SceneUtilities/Scene Dependency Data", fileName = "SceneDependencyData_Default")]
	public partial class SceneDependencyData : ScriptableObject
	{
		void Awake() => RefreshGraph();

		void OnValidate() => RefreshGraph();

		void RefreshGraph() => _graph = new(_dependencies);

		/// <summary>
		/// <paramref name="pivotSceneName"/> 이름의 씬과 거리 <paramref name="depth"/> 이하만큼 떨어진 씬을 찾아, 그 이름들을 반환한다.
		/// </summary>
		public IReadOnlyCollection<string> RetrieveNearSceneUniqueNames(string pivotSceneName, int depth)
		{
			var nearSceneNames = _graph.distances[pivotSceneName] // [(sceneName, distance), (seneName, distance), ...]
				.Where(p => p.Value <= depth)
				.Select(p => p.Key)
				.ToList()
				.AsReadOnly();

			return nearSceneNames;
		}

		public string InitialSceneName => _initialSceneReference.SceneName;
		public string PersistentSceneName => _persistentSceneReference.SceneName;
		public string MainMenuSceneName => _mainMenuSceneReference.SceneName;

		SceneGraph _graph;

		[SerializeField] SceneNode[] _dependencies;
		[SerializeField] SceneReference _initialSceneReference;
		[SerializeField] SceneReference _persistentSceneReference;
		[SerializeField] SceneReference _mainMenuSceneReference;
	}

}