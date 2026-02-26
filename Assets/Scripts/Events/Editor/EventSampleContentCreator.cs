#if UNITY_EDITOR
using System.Collections.Generic;
using SRS.Events.Routing;
using UnityEditor;
using UnityEngine;

namespace SRS.Events.Editor
{
    public static class EventSampleContentCreator
    {
        [MenuItem("SRS/Events/Generate Sample Event Content")]
        public static void Generate()
        {
            EnsureFolders();

            var routes = new Dictionary<string, RouteData>
            {
                { "Route_Sprint_Downtown", CreateRoute("route_sprint_downtown", new Vector3(125f,0f,210f), Vector3.forward, 8f, new []{ new Vector3(150,0,250), new Vector3(200,0,295), new Vector3(260,0,330), new Vector3(320,0,365) }) },
                { "Route_Sprint_Harbor", CreateRoute("route_sprint_harbor", new Vector3(420f,0f,-80f), Vector3.left, 9f, new []{ new Vector3(390,0,-60), new Vector3(340,0,-35), new Vector3(290,0,10), new Vector3(240,0,55) }) },
                { "Route_Sprint_Hillside", CreateRoute("route_sprint_hillside", new Vector3(-120f,2f,540f), Vector3.right, 8f, new []{ new Vector3(-60,3,560), new Vector3(0,4,600), new Vector3(70,5,645), new Vector3(145,6,680) }) },
                { "Route_Circuit_OldTown", CreateRoute("route_circuit_oldtown", new Vector3(40f,0f,40f), Vector3.forward, 7f, new []{ new Vector3(75,0,100), new Vector3(130,0,85), new Vector3(145,0,20), new Vector3(90,0,-20), new Vector3(35,0,0) }) },
                { "Route_Circuit_Riverside", CreateRoute("route_circuit_riverside", new Vector3(-300f,0f,180f), Vector3.right, 8f, new []{ new Vector3(-240,0,200), new Vector3(-180,0,170), new Vector3(-170,0,110), new Vector3(-240,0,95) }) },
                { "Route_Circuit_Airport", CreateRoute("route_circuit_airport", new Vector3(600f,0f,-320f), Vector3.forward, 10f, new []{ new Vector3(660,0,-260), new Vector3(740,0,-290), new Vector3(730,0,-390), new Vector3(640,0,-430) }) },
            };

            var events = new List<EventDefinition>
            {
                CreateEvent("sprint_downtown_dash", "Downtown Dash", EventType.Sprint, routes["Route_Sprint_Downtown"], 1, 180f, 5, 450, 120),
                CreateEvent("sprint_harbor_blast", "Harbor Blast", EventType.Sprint, routes["Route_Sprint_Harbor"], 1, 190f, 8, 520, 150),
                CreateEvent("sprint_hillside_run", "Hillside Run", EventType.Sprint, routes["Route_Sprint_Hillside"], 1, 220f, 12, 700, 220),
                CreateEvent("circuit_oldtown_loop", "Old Town Loop", EventType.Circuit, routes["Route_Circuit_OldTown"], 3, 0f, 10, 900, 280),
                CreateEvent("circuit_riverside_ring", "Riverside Ring", EventType.Circuit, routes["Route_Circuit_Riverside"], 4, 0f, 14, 1200, 360),
                CreateEvent("circuit_airport_orbit", "Airport Orbit", EventType.Circuit, routes["Route_Circuit_Airport"], 5, 0f, 20, 1800, 520),
            };

            var registry = ScriptableObject.CreateInstance<EventRegistry>();
            var serialized = new SerializedObject(registry);
            var property = serialized.FindProperty("events");
            property.arraySize = events.Count;
            for (var i = 0; i < events.Count; i++)
            {
                property.GetArrayElementAtIndex(i).objectReferenceValue = events[i];
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.CreateAsset(registry, "Assets/Data/Events/EventRegistry.asset");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Sample event content generated.");
        }

        private static RouteData CreateRoute(string routeId, Vector3 startPosition, Vector3 startForward, float checkpointRadius, Vector3[] checkpoints)
        {
            var route = ScriptableObject.CreateInstance<RouteData>();
            route.routeId = routeId;
            route.startPosition = startPosition;
            route.startForward = startForward;
            route.checkpointRadius = checkpointRadius;
            route.checkpointPositions = new List<Vector3>(checkpoints);
            AssetDatabase.CreateAsset(route, $"Assets/Data/Routes/{ToAssetName(routeId)}.asset");
            return route;
        }

        private static EventDefinition CreateEvent(string id, string displayName, EventType type, RouteData route, int laps, float timeLimit, int respect, int cash, int respectReward)
        {
            var evt = ScriptableObject.CreateInstance<EventDefinition>();
            evt.eventId = id;
            evt.displayName = displayName;
            evt.type = type;
            evt.route = route;
            evt.laps = laps;
            evt.timeLimitSeconds = timeLimit;
            evt.recommendedRespect = respect;
            evt.baseCashReward = cash;
            evt.baseRespectReward = respectReward;
            AssetDatabase.CreateAsset(evt, $"Assets/Data/Events/{ToAssetName(id)}.asset");
            return evt;
        }

        private static string ToAssetName(string value)
        {
            return value.Replace(" ", "_");
        }

        private static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Data")) AssetDatabase.CreateFolder("Assets", "Data");
            if (!AssetDatabase.IsValidFolder("Assets/Data/Events")) AssetDatabase.CreateFolder("Assets/Data", "Events");
            if (!AssetDatabase.IsValidFolder("Assets/Data/Routes")) AssetDatabase.CreateFolder("Assets/Data", "Routes");
        }
    }
}
#endif
