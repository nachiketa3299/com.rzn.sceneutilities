using System;

namespace RZN.SceneUtilities
{
	/// <summary>
	/// � ���� ������ ���� �����Ѵ�.
	/// </summary>
	[Serializable]
	internal struct SceneNode
	{
		public SceneReference sceneRef;
		public SceneReference[] nearSceneRefs;
	}
}
