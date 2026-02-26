using System;
using System.Collections.Generic;
using SRS.Events.Routing;
using SRS.UI;
using UnityEngine;

namespace SRS.Events.Modes
{
    public sealed class SprintMode : IEventMode
    {
        public event Action<EventResult> OnFinished;

        private readonly Transform _player;
        private readonly Checkpoint _checkpointPrefab;
        private readonly Transform _checkpointRoot;
        private readonly EventHUD _hud;

        private readonly List<Checkpoint> _activeCheckpoints = new List<Checkpoint>(32);
        private EventSession _session;
        private int _nextCheckpointIndex;

        public SprintMode(Transform player, Checkpoint checkpointPrefab, Transform checkpointRoot, EventHUD hud)
        {
            _player = player;
            _checkpointPrefab = checkpointPrefab;
            _checkpointRoot = checkpointRoot;
            _hud = hud;
        }

        public void Initialize(EventSession session)
        {
            _session = session;
            _nextCheckpointIndex = 0;
            SpawnRouteCheckpoints(session.Route);
            _hud.SetProgress(0, _activeCheckpoints.Count, 1, 1);
        }

        public void Tick(float dt)
        {
            if (_session == null)
            {
                return;
            }

            _hud.SetTimer(Time.time - _session.StartTime);
        }

        public void Dispose()
        {
            for (var i = 0; i < _activeCheckpoints.Count; i++)
            {
                if (_activeCheckpoints[i] != null)
                {
                    UnityEngine.Object.Destroy(_activeCheckpoints[i].gameObject);
                }
            }

            _activeCheckpoints.Clear();
            _session = null;
        }

        private void SpawnRouteCheckpoints(RouteData route)
        {
            for (var i = 0; i < route.checkpointPositions.Count; i++)
            {
                var checkpoint = UnityEngine.Object.Instantiate(_checkpointPrefab, route.checkpointPositions[i], Quaternion.identity, _checkpointRoot);
                checkpoint.Initialize(i, route.checkpointRadius, HandleCheckpointTriggered);
                _activeCheckpoints.Add(checkpoint);
            }
        }

        private void HandleCheckpointTriggered(Checkpoint checkpoint)
        {
            if (checkpoint.Index != _nextCheckpointIndex)
            {
                return;
            }

            _nextCheckpointIndex++;
            _hud.SetProgress(_nextCheckpointIndex, _activeCheckpoints.Count, 1, 1);

            if (_nextCheckpointIndex < _activeCheckpoints.Count)
            {
                return;
            }

            OnFinished?.Invoke(new EventResult
            {
                eventId = _session.Definition.eventId,
                success = true,
                elapsedSeconds = Time.time - _session.StartTime,
                checkpointsPassed = _nextCheckpointIndex,
                completedLaps = 1
            });
        }
    }
}
