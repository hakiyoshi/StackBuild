using UnityEngine;

namespace StackBuild.Extensions
{
    public static class VectorExtensions
    {

        public static Vector3 WithY(this Vector3 self, float y)
        {
            self.y = y;
            return self;
        }

    }
}
