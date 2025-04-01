using UnityEngine;

namespace CustomizableControls
{
    public static class ColliderExtensions
    {
        public static Vector3 GetLocalSize(this Collider collider)
        {
            if (collider is BoxCollider boxCollider)
            {
                return boxCollider.size;
            }
            else if (collider is CapsuleCollider capsuleCollider)
            {
                Vector3 size = capsuleCollider.radius * Vector3.one;
                size[capsuleCollider.direction] = capsuleCollider.height;

                return size;
            }
            else if (collider is SphereCollider sphereCollider)
            {
                return sphereCollider.radius * Vector3.one;
            }
            else
            {
                return collider.bounds.size;
            }
        }

        public static Vector3 GetSize(this Collider collider)
        {
            return Vector3.Scale(collider.transform.lossyScale, collider.GetLocalSize());
        }
    }
}