using UnityEngine;

namespace StackBuild.Extensions
{
    public static class VectorExtensions
    {

        public static Vector2 WithX(this Vector2 self, float x)
        {
            self.x = x;
            return self;
        }

        public static Vector2 WithY(this Vector2 self, float y)
        {
            self.y = y;
            return self;
        }

        public static Vector3 WithY(this Vector3 self, float y)
        {
            self.y = y;
            return self;
        }

    }
}
