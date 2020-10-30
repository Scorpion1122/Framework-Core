namespace UnityEngine
{
	public static class GameObjectExtensions
	{
		public static Bounds GetLocalColliderBounds(this GameObject gameObject)
		{
			bool first = true;
			Bounds result = new Bounds();
			
			Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
			foreach(Collider collider in colliders)
			{
				Bounds bounds = collider.GetLocalBounds();
				bounds.center = collider.transform.TransformPoint(bounds.center);
				bounds.center = gameObject.transform.InverseTransformPoint(bounds.center);

				if(first)
				{
					result = bounds;
					first = false;
				}
				else
				{
					result.Encapsulate(bounds);
				}
			}

			return result;
		}
	}
}
