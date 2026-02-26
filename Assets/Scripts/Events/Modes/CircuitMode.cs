using System;
using System.Collections.Generic;
using SRS.Events.Routing;
using SRS.UI;
using UnityEngine;

namespace SRS.Events.Modes
{
    public sealed class CircuitMode : IEventMode
    {
        public event Action<EventResult> OnFinished;

        private readonly Checkpoint _checkpointPrefab;
        private readonly Transform _checkpointRoot;
        private readonly EventHUD _hud;
        private readonly List<Checkpoint> _activeCheckpoints = new List<Checkpoint>(32);

        private EventSession _session;
        private int _nextCheckpointIndex;
        private int _currentLap;

        public CircuitMode(Checkpoint checkpointPrefab, Transform checkpointRoot, EventHUD hud)
        {
            _checkpointPrefab = checkpointPrefab;
            _checkpointRoot = checkpointRoot;
            _hud = hud;
        }

        public void Initialize(EventSession session)
        {
            _session = session;
            _nextCheckpointIndex = 0;
            _currentLap = 1;
            SpawnRouteCheckpoints(session.Route);
            _hud.SetProgress(0, _activeCheckpoints.Count, _currentLap, _session.Definition.laps);
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
            if (_nextCheckpointIndex >= _activeCheckpoints.Count)
            {
                _nextCheckpointIndex = 0;
                _currentLap++;
            }

            if (_currentLap > _session.Definition.laps)
            {
                OnFinished?.Invoke(new EventResult
                {
                    eventId = _session.Definition.eventId,
                    success = true,
                    elapsedSeconds = Time.time - _session.StartTime,
                    checkpointsPassed = _session.Definition.laps * _activeCheckpoints.Count,
                    completedLaps = _session.Definition.laps
                });
                return;
            }

            _hud.SetProgress(_nextCheckpointIndex, _activeCheckpoints.Count, _currentLap, _session.Definition.laps);
        }
    }
}
