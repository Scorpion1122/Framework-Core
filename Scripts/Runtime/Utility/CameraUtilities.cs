using UnityEngine;

namespace TKO.Core.Utilities
{
	public static class CameraUtilities
	{
		public static Vector2[] GetOrthoCorners()
		{
			Camera camera = Camera.main;
			float size = camera.orthographicSize;
			return new Vector2[]
			{
				new Vector2(-size * camera.aspect, -size),
				new Vector2(-size * camera.aspect, size),
				new Vector2(size * camera.aspect, size),
				new Vector2(size * camera.aspect, -size),
				new Vector2(-size * camera.aspect, -size),
			};
		}

	}
}
