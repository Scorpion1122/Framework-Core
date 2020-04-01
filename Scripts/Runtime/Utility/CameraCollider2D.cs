using UnityEngine;

namespace TKO.Core.Utilities
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(EdgeCollider2D))]
	public class CameraCollider2D : MonoBehaviour
	{
		private EdgeCollider2D edgeCollider;
		private float lastSetSize = -1f;
		private float lastSetAspect = -1f;

		private void Awake()
		{
			edgeCollider = GetComponent<EdgeCollider2D>();
		}

		// Update is called once per frame
		private void Update()
		{
			Camera camera = Camera.main;
			if (camera.orthographicSize == lastSetSize && lastSetAspect == camera.aspect)
				return;

			float size = camera.orthographicSize;
			edgeCollider.points = CameraUtilities.GetOrthoCorners();

			lastSetAspect = camera.aspect;
			lastSetSize = camera.orthographicSize;
		}


	}
}
