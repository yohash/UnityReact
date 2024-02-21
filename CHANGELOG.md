# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.3.9] - 2024-02-23

### Changed

- Renamed `Element.Address` to `Element.Key` for clarity

## [0.3.8] - 2024-02-07

### Fixed

- Removed the `Component.initialized` bool, and internal checking, as an attempt to manage uninitialized components. This ends up blocking usage of `Monobehaviour.Update()`, which is common enough to hinder development if rendered useless. `Component` now subscribe to `Store` on `Start()`.

## [0.3.7] - 2023-12-11

### Fixed

- Corrected the `Component.updateChildren()` wait-method from `if (isUpdating)` to `while(isUpdating)` to properly wait until the current update loop completes
- Added a defensive null-check in `Component.updateChildren()` to prevent a null-reference to a `child.Component` that hadn't mounted yet.

## [0.3.6] - 2023-12-08

### Added

- Added requirement for `IComponent` to return a `UnityEngine.Object` representing itself
- Added to sample scene to show how a component can be mounted/unmounted as a behaviour

## [0.3.5] - 2023-12-05

### Fixed

- Corrected sample runtimes by including one per-folder, instead of at `Samples~/`

## [0.3.4] - 2023-12-05

### Added

- All scripts in the `Samples~/` subfolder now have their own Assembly Definition file and namespace

### Fixed

- Fixed a null-ref in the `StateViewer` tool when a null `IEnumerable` was sent to an internal `count(IEnumerable object)` method

## [0.3.3] - 2023-11-27

### Fixed

- The `StateViewer` tool will now view private state variables as well as public.

## [0.3.2] - 2023-11-27

### Fixed

- The `ComponentViewer` will gather all component(s) on a gameobject imlpementing `IComponent`, as opposed to only the first.

## [0.3.1] - 2023-11-24

### Added

- Added a call to update component after a `Component` initializes. This will allow components to immediately mount any child `Element`s.

## [0.3.0] - 2023-11-20

### Added

- Added `Elements` that can be mounted (returned) by any `Component`
- Created a rudimentary `Component` viewer to view all subscribed components in the scene

## [0.2.5] - 2023-11-06

### Added

- Created a recursive state viewer useful for viewing the contents of all state in your store in the editor

## [0.2.4] - 2023-10-28

### Fixed

- Added null check before unsubscribing in `Component.OnDestroy()`. Prevents throwing exception if the player is closed, destroying the store before the component

## [0.2.3] - 2023-10-24

### Fixed

- Changed `Component.cs` unsubscribe to `OnDestroy` vs. `OnDisable` to allow subscribed gameobjects to disable without losing connection to react subsystems

## [0.2.2] - 2023-10-05

### Fixed

- Correct an order-of-operations error in Component initialization, that could cause an infinite loop if the Component dispatched changes to its Props in the `InitializeComponent()` method.

## [0.2.1] - 2023-10-03

### Changed

- Swapped all sample-scene action types from `class` to `struct`

## [0.2.0] - 2023-10-02

### Changed

- `public abstract class Action` changed to `public interface IAction`
  - The base type is primarily used for pattern matching.
  - This type allows the freedom of passing structs.
- Upgrade Unity version to 2021.3.28f1