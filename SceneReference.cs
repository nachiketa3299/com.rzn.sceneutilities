
using System.IO;
using System;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;

#endif

namespace RZN.SceneUtilities
{
	/// <summary> SceneAsset 에 대한 참조를 저장하기 위한 클래스 </summary>
	/// <remarks> 어떻게 작동하는지 구체적으로 모름 (수정하지 말 것, 원본은 <see href="https://github.com/NibbleByte/UnitySceneReference/blob/master/Assets/DevLocker/Utils/SceneReference.cs">여기</see>에) </remarks>
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	[Serializable]
	public class SceneReference : ISerializationCallbackReceiver
	{

#if UNITY_EDITOR
		// 에디터에서 사용되는 애셋에 대한 레퍼런스로 빌드에서는 이 내용이 무의미하게됨
		// 에디터에서의 수정 사항을 반영하기 위한 용도
		[SerializeField] SceneAsset _sceneAsset;

#pragma warning disable 0414
		// 인스펙터 업데이트용 데이터 dirtiness 표시
		[SerializeField] bool _isDirty;
#pragma warning restore 0414
#endif

		// 빌드에서는 여기에 있는 씬 경로를 사용할 것
		[SerializeField] string _scenePath = string.Empty; //

		/// <summary>
		/// <see cref="UnityEngine.SceneManagement.SceneManager"/> API 에서 사용되는 씬 경로를 반환.
		/// 에디터에서 작업하는 경우, 이 경로는 애셋이 이동되거나 재명명되어도 항상 최신임이 보장됨.
		/// 만일 참조된 씬 애셋이 삭제된경우, 경로는 남음.
		/// </summary>
		public string ScenePath
		{
			get
			{
#if UNITY_EDITOR
				AutoUpdateReference();
#endif
				return _scenePath;
			}

			set
			{
				_scenePath = value;
#if UNITY_EDITOR
				if (string.IsNullOrEmpty(_scenePath))
				{
					_sceneAsset = null;
					return;
				}

				_sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(_scenePath);

				if (_sceneAsset == null)
				{
					Debug.LogError($"{nameof(SceneReference)}를 {value}로 설정하려고 시도하였으나, 어떤 씬도 할당될 수 없었다.");
				}
#endif
			}
		}

		public string SceneName => Path.GetFileNameWithoutExtension(ScenePath);
		public bool IsEmpty => string.IsNullOrEmpty(ScenePath);
		public SceneReference() { }
		public SceneReference(string scenePath)
		{
			ScenePath = scenePath;
		}

		public SceneReference(SceneReference other)
		{
			_scenePath = other._scenePath;

#if UNITY_EDITOR
			_sceneAsset = other._sceneAsset;
			_isDirty = other._isDirty;
			AutoUpdateReference();
#endif
		}

#if UNITY_EDITOR
		static bool _reloadingAssemblies = false;

		static SceneReference()
		{
			AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
		}

		static void OnBeforeAssemblyReload()
		{
			_reloadingAssemblies = true;
		}
#endif

		public SceneReference Clone() => new(this);

		public override string ToString()
		{
			return _scenePath;
		}

		[Obsolete("에디터 전용이며, 런타임에서는 사용하지 말 것", true)]
		public void OnBeforeSerialize()
		{
#if UNITY_EDITOR
			if (_reloadingAssemblies)
			{
				return;
			}

			AutoUpdateReference();
#endif
		}

		[Obsolete("에디터 전용이며, 런타임에서는 사용하지 말 것", true)]
		public void OnAfterDeserialize()
		{
#if UNITY_EDITOR
			EditorApplication.update += OnAfterDeserializeHandler;
#endif
		}

#if UNITY_EDITOR
		void OnAfterDeserializeHandler()
		{
			EditorApplication.update -= OnAfterDeserializeHandler;
			AutoUpdateReference();
		}

		void AutoUpdateReference()
		{

			if (_sceneAsset == null)
			{

				if (string.IsNullOrEmpty(_scenePath))
				{
					return;
				}

				SceneAsset foundAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(_scenePath);

				if (foundAsset)
				{
					_sceneAsset = foundAsset;
					_isDirty = true;

					if (!Application.isPlaying)
					{
						// Scriptable Object 에서는 작동하지 않음!
						EditorSceneManager.MarkAllScenesDirty();
					}
				}
			}
			else
			{
				string foundPath = AssetDatabase.GetAssetPath(_sceneAsset);
				if (string.IsNullOrEmpty(foundPath))
				{
					return;
				}

				if (foundPath != _scenePath)
				{
					_scenePath = foundPath;
					_isDirty = true;

					if (!Application.isPlaying)
					{
						EditorSceneManager.MarkAllScenesDirty();
					}
				}
			}
		}
#endif
	} // class SceneReference
} // namespace MC.Utility