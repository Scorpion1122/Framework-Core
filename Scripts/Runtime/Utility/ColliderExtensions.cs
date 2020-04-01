using System;

namespace UnityEngine
{
	public static class ColliderExtensions
	{
		public static Bounds GetLocalBounds(this Collider collider)
		{
			if(collider is BoxCollider boxCollider)
			{
				return new Bounds(boxCollider.center, boxCollider.size);
			}

			if(collider is SphereCollider sphereCollider)
			{
				return new Bounds(sphereCollider.center, Vector3.one * sphereCollider.radius);
			}

			if(collider is CapsuleCollider capsuleCollider)
			{
				return new Bounds(
					capsuleCollider.center, 
					new Vector3(capsuleCollider.radius, capsuleCollider.height, capsuleCollider.radius));
			}
			
			throw new Exception($"Collider of type {collider.GetType()} is not supported");
		}
	}
}
