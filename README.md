# SimulationTimeScaleSetting

A library mod for [Phantom Brigade](https://braceyourselfgames.com/phantom-brigade/) that adds two sliders to the game options to change the simulation time scaling factors for full and slow speed settings.

It is compatible with game release version **2.1.0**. It works with both the Steam and Epic installs of the game. All library mods are fragile and susceptible to breakage whenever a new version is released.

Phantom Brigade enters simulation mode when a turn is executed. Simulation speed can be changed by the player. There are four speed settings: pause, slow, half and full. Each setting computes a time scale based on the `timeScaleSlow` and `timeScaleMain` properties of the `simulation` configuration object. The slow speed setting uses the `timeScaleSlow` value directly as the time scale. Similarly, the full speed setting uses the `timeScaleMain` value as the time scale. The half setting is a midpoint linear interpolation between `timeScaleSlow` and `timeScaleMain`.

This mod adds two sliders to the game options so that the player can change the `timeScaleSlow` and `timeScaleMain` values at runtime. The two sliders are linked such that the `timeScaleSlow` value cannot be set through its slider to be larger than the `timeScaleMain` value.
