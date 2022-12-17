using System;
using UnityEngine;

namespace StackBuild
{
    public class PlayerHit : MonoBehaviour
    {
        const float radius = 2.05f;
        const float height = 20.0f;

        private void FixedUpdate()
        {
            var obj = CheckHit();
            if (obj == null)
                return;

            var vec = (transform.position - obj.transform.position).normalized;
            transform.position += vec * 0.1f;
        }

        GameObject CheckHit()
        {

            var diff = new Vector3(0f, height / 2.0f, 0f);
            var hit = Physics.OverlapCapsule(transform.position + diff, transform.position - diff, radius);
            foreach (var collider in hit)
            {
                //自分以外のプレイヤーだったら
                var layerMask = LayerMask.GetMask("P1", "P2") & ~(1 << gameObject.layer);
                if (((1 << collider.gameObject.layer) & layerMask) != 0)
                    return collider.gameObject;
            }
            return null;
        }
    }
}