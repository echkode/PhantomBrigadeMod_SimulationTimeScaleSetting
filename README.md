# SimulationTimeScaleSetting

A library mod for [Phantom Brigade](https://braceyourselfgames.com/phantom-brigade/) that adds two sliders to the game options to change the simulation time scaling factors for full and slow speed settings.

It is compatible with game release version **2.1.0**. It works with both the Steam and Epic installs of the game. All library mods are fragile and susceptible to breakage whenever a new version is released.

Phantom Brigade enters simulation mode when a turn is executed. Simulation speed can be changed by the player. There are four speed settings: pause, slow, half and full. Each setting computes a time scale based on the `timeScaleSlow` and `timeScaleMain` properties of the `simulation` configuration object. The slow speed setting uses the `timeScaleSlow` value directly as the time scale. Similarly, the full speed setting uses the `timeScaleMain` value as the time scale. The half setting is a linear interpolation of the midpoint between `timeScaleSlow` and `timeScaleMain`.

This mod adds two sliders to the game options so that the player can change the `timeScaleSlow` and `timeScaleMain` values at runtime. The two sliders are linked such that the `timeScaleSlow` value cannot be set through its slider to be larger than the `timeScaleMain` value.

<img width="1026" height="758" alt="time_scale_settings" src="https://github.com/user-attachments/assets/545430f7-460a-4d54-a42d-e3b6dc3463a8" />

## Time Scale Details

The game normally has 4 settings for time scale: pause, slow, half and full. The following is how the game sets the time scale in simulation for each of the four settings.
```
Pause : time scale = 0
Slow : time scale = lerp(timeScaleSlow, timeScaleMain, 0) ==> timeScaleSlow
Half : time scale = lerp(timeScaleSlow, timeScaleMain, 0.5)
Full : time scale = lerp(timeScaleSlow, timeScaleMain, 1) ==> timeScaleMain
```
where `lerp` is the linear interpolation function built into Unity. Both `timeScaleSlow` and `timeScaleMain` are properties of the `simulation` config object.

Replay also uses 4 settings for its playback speed. These values are hard-coded; you cannot change them without using a library mod.
```
Pause : playback speed = 0
Slow : playback speed = 0.12
Half : playback speed = 0.6
Full : playback speed = 1.2
```
Time scale and playback speed are used as scaling factors to calculate a time step. For the speeds to match between simulation and replay, the scaling factor for the speed setting in each of the modes has to be the same. The stock setting for `timeScaleMain` is 0.6. This makes the full setting in simulation run at the same speed as the half setting in replay.
