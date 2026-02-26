using System.Collections.Generic;
using UnityEngine;

namespace SRS.Events.Routing
{
    [CreateAssetMenu(fileName = "RouteData", menuName = "SRS/Events/Route Data")]
    public sealed class RouteData : ScriptableObject
    {
        public string routeId;
        public List<Vector3> checkpointPositions = new List<Vector3>();
        public Vector3 startPosition;
        public Vector3 startForward = Vector3.forward;
        [Min(0.25f)] public float checkpointRadius = 8f;

        public int CheckpointCount => checkpointPositions == null ? 0 : checkpointPositions.Count;

        private void OnValidate()
        {
            if (startForward.sqrMagnitude < 0.0001f)
            {
                startForward = Vector3.forward;
            }

            if (checkpointRadius < 0.25f)
            {
                checkpointRadius = 0.25f;
            }
        }
    }
}
