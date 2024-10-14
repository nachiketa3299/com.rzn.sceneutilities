using System;

namespace RZN.SceneUtilities
{
	/// <summary>
	/// 어떤 씬과 인접한 씬을 정의한다.
	/// </summary>
	[Serializable]
	internal struct SceneNode
	{
		public SceneReference sceneRef;
		public SceneReference[] nearSceneRefs;
	}
}
