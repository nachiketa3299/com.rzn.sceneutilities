using System.Collections.Generic;
using System.Linq;

using UnityEngine;


namespace RZN.SceneUtilities
{

	/// <summary>
	/// 게임 오브젝트가 씬의 로딩 박스에 존재하게 되거나 존재하게 되지 않을 때,<br/>
	/// 주변 씬의 로드/언로드 요청을 보낸다.
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))] // 리지드 바디가 있어야, 컬리전을 검출할 수 있다.
	public partial class SceneLoadTrigger : MonoBehaviour
	{

		#region UnityCallbacks
		void OnDisable() => SceneLoadManager.Instance.Disabled(this);

		#endregion // UnityCallbacks

		#region UnityCollision

		void OnTriggerEnter(Collider collider)
		{
			// 씬 로딩 박스가 맞나?
			if (!IsSceneLoadingBoxLayer(collider.gameObject.layer))
			{
				return;
			}

			var enteringSceneName = collider.gameObject.scene.name;

			if (_inSceneName == enteringSceneName)
			{
				return;
			}

			// 현재 이 오브젝트가 존재하는 씬 이름이 달라진다
			_inSceneName = enteringSceneName;
			_sceneNamesToMaintain = new(SceneLoadManager.Instance.RetrieveNearSceneNames(_inSceneName, _depthToLoad));

			// 이름이 달라지면 당연히 로딩 요청을 새로 보내여야 함
			// 여기서 시작
			SceneLoadManager.Instance.Entered(entering: this, sceneName: _inSceneName, depthToLoad: _depthToLoad);
		}

		#endregion // UnityCollision

#if UNITY_EDITOR
		[UnityEditor.DrawGizmo(UnityEditor.GizmoType.Selected)]
		static void DrawSceneDependency(SceneLoadTrigger target, UnityEditor.GizmoType _)
		{
			var startPos = target.transform.position;
			foreach (var sceneName in target._sceneNamesToMaintain)
			{
				// Draw Line
				var scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
				if (!scene.isLoaded)
				{
					continue;
				}
				var rootGo = scene.GetRootGameObjects().FirstOrDefault();
				if (rootGo == null)
				{
					continue;
				}

				var endPos = rootGo.transform.position;

				UnityEditor.Handles.color = Color.gray;
				UnityEditor.Handles.DrawDottedLine(startPos, endPos, 1f);

				// Draw Label

				var labelStart = (endPos - startPos).normalized * 4.0f;
				var style = new GUIStyle() { alignment = TextAnchor.MiddleCenter };
				style.normal.textColor = UnityEditor.Handles.color;

				UnityEditor.Handles.Label(startPos + labelStart, sceneName, style);
			}
		}
#endif

		public bool IsSceneLoadingBoxLayer(in int layer) => layer == _sceneLoadingBoxLayer;

		string _inSceneName = string.Empty;
		[SerializeField] int _depthToLoad = 1;
		[SerializeField][HideInInspector] List<string> _sceneNamesToMaintain = new();
		readonly int _sceneLoadingBoxLayer = 6;
	}

}