# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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