using System;
using UnityEngine;

namespace SRS.Events.Routing
{
    [RequireComponent(typeof(SphereCollider))]
    public sealed class Checkpoint : MonoBehaviour
    {
        [SerializeField] private int index;
        private Action<Checkpoint> _onTriggered;

        public int Index => index;

        public void Initialize(int checkpointIndex, float radius, Action<Checkpoint> onTriggered)
        {
            index = checkpointIndex;
            _onTriggered = onTriggered;

            var sphere = GetComponent<SphereCollider>();
            sphere.isTrigger = true;
            sphere.radius = radius;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            _onTriggered?.Invoke(this);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            var sphere = GetComponent<SphereCollider>();
            var radius = sphere != null ? sphere.radius : 4f;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
#endif
    }
}
