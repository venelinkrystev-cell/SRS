# Free Roam Module

## Setup
1. Open `Assets/Scenes/FreeRoam.unity`.
2. Add `FreeRoamBootstrap` to an empty `FreeRoamSystems` GameObject.
3. Assign:
   - `PlayerCar.prefab`
   - `EventRegistry.asset`
   - `MinimapController`
   - `WorldStreamer`
   - `EventMarker.prefab`
   - `EventTriggerZone.prefab`
4. Ensure a spawn Transform is tagged `PlayerSpawn`.
5. Create sectors under `WorldStreamer` configuration.

## Runtime Behavior
- Player spawns at `PlayerSpawn` and can drive freely.
- Minimap camera follows and projects player + event markers.
- Event zones show `Press E to Start Event` and call `IEventLauncher.StartEvent`.
- `WorldStreamer` keeps nearby sectors active and updates debug overlay.
