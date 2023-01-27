using UnityEngine;
using UnityEngine.UI;

namespace StackBuild.UI
{
    public class SkewedImage : Image
    {

        [SerializeField] private float skewFactor;
        [SerializeField] private bool skewLeft;
        [SerializeField] private bool skewRight;
        [SerializeField] private bool shrink;

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (skewFactor <= 0)
            {
                base.OnPopulateMesh(toFill);
                return;
            }

            var r = GetPixelAdjustedRect();
            var v = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);
            var skewAmount = r.height * skewFactor;

            if (!shrink)
            {
                skewAmount /= 2;
            }

            float skewedLeft1  = v.x           - (skewLeft && !shrink  ? skewAmount : 0);
            float skewedLeft2  = v.x           + (skewLeft             ? skewAmount : 0);
            float skewedRight2 = v.x + r.width + (skewRight && !shrink ? skewAmount : 0);
            float skewedRight1 = v.x + r.width - (skewRight            ? skewAmount : 0);
            float bottom       = v.y;
            float top          = v.w;

            if (skewedLeft2 > skewedRight2)
            {
                top -= (skewedLeft2 - skewedRight2) / skewFactor;
                skewedLeft2 = skewedRight2;
            }

            if (skewedRight1 < skewedLeft1)
            {
                bottom += (skewedLeft1 - skewedRight1) / skewFactor;
                skewedRight1 = skewedLeft1;
            }

            Color32 color32 = color;
            toFill.Clear();
            toFill.AddVert(new Vector3(skewedLeft1,  bottom), color32, new Vector2(0f, 0f));
            toFill.AddVert(new Vector3(skewedLeft2,  top),    color32, new Vector2(0f, 1f));
            toFill.AddVert(new Vector3(skewedRight2, top),    color32, new Vector2(1f, 1f));
            toFill.AddVert(new Vector3(skewedRight1, bottom), color32, new Vector2(1f, 0f));

            toFill.AddTriangle(0, 1, 2);
            toFill.AddTriangle(2, 3, 0);
        }

    }
}