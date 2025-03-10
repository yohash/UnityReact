# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.4.11] - 2025-03-03

### Added

- Added an `IDebugAction` type to collect `StackTrace` information upon an original `IAction` dispatch, while a `Store` has `log` activated.

## [0.4.10] - 2025-02-19

### Fixed

- Correct the `IAction` extension method `ToDetailedString()` output data format

## [0.4.9] - 2025-02-19

### Added

- Added an `IAction` reflection-based `ToDetailedString()` method for `Store` debugging to show action member variables

### Fixed

- Correct the state viewer to properly unpack and display nullable types

### Changed

- Increase reactivity of state viewer by adding state populate to the `Update` method
- Add try-catch around the `_store.Subscribe()` call inside `Component`, with deliberate logging to track any errors in the `async` subscribe-and-initialize loop

## [0.4.8] - 2025-01-08

### Added

- Provided a means to prescribe `Element` sibling index in their parent `Transform`.

## [0.4.7] - 2024-10-30

### Added

- Made `Component.OnEnable()` and `OnDisable()` methods `protected virtual` so sub classes can override.

## [0.4.6] - 2024-10-29

### Added

- Modify `IComponent` with an `InitializeElement(PropsContainer, State)` to explicitly initialize mounted `Element`s
- Added property `bool Props.HasCustomProps`, which is defined by code generation in `PropsGenerator`. We reference this bool in `Component.onStoreInitialize()` and bypass store initialization, waiting for `IComponent.InitializeElement()` instead. This guarantees one "initialization" call in a `Component`, whether it has custom props (an `Element`) or not.
- Created an asynchronous handle to `Store.Instance` to handle uncontrolled start-up order. If a `Component` subscribes before the `Store` has been created, this new handle allows for a `var store = await Store.Instance` syntax to handle the `null` store instance. 

### Fixed

- Fix a bug where `Element` would initialize (when it ran its `Start()` method) but would have `default` props when `Component.InitializeComponent()` ran until the `Element.UpdateElementWithProps()` ran.
- Minor fix to handle less-than-0 array indexing in the sample scene

### Changed

- Changed `IComponent.UpdateElementWithProps()` to `IComponent.UpdateElement()`
- `Component` re-worked to call `InitializeElement()` when new elements were added, instead of calling `UpdateElement()`
- Moved `Component` subscribe/unsubscribe code from `Start()`/`OnDestroy()` into `OnDisable()`/`OnDisable()`, respectively. This will enable `Component`s to be re-used (pooled).
- `StateViewer` and `ComponentViewer` modified to handle `async Store.Instance` handle.

## [0.4.5] - 2024-10-21

### Fixed

- Added a check to `Component.UpdateElementWithProps()` to also consider `Props.DidUpdate()` and then `Props.BuildProps()` if warranted. If the element's props are comprised of both `PropsContainer` and `StateContainer`, missing this additional check can result in an `Element` (`Component`) updating with its `PropsContainer`, but receiving its old `StateContainer` props


## [0.4.4] - 2024-10-03

### Fixed

- Bugfix for `Component.UpdateComponent()` running twice with the same `props` and `oldprops` if state in the `Props` was changed while the `Component.InitializeComponent()` was running. That is, if `InitializeComponent()` were to `dispatch` an action that changed the component's `Props`, the `UpdateComponent()` method would receive two calls with the same `props` and `oldprops`. 

## [0.4.3] - 2024-07-23

### Added

- Improved the `Component` update algorithm to only create copies of props if the relevant state did indeed update. Previously we were copying all `props` into `oldProps` before checking for updates.

### Fixed

- Bugfix for prop generation that wouldn't reflect changes between `oldProps` and `props`.

## [0.4.2] - 2024-07-23

### Fixed

- `StateViewer` reflection reference to `Store._state` corrected to `Store.state`

## [0.4.1] - 2024-07-17

### Fixed

- State and Props code generation will properly write files for `Props` and `StateContainers` that do not appear in a namespace.

## [0.4.0] - 2024-07-15

### Added

- Added code generation to create direct references for Props assignments and State creation. Usage requires `Props` only reference their `StateContainers` or `PropsContainers`, and code generation is run after saving. Similarly, anytime a new `StateContainer` is saved, code generation should be run to create the combined `State` object.

## [0.3.12] - 2024-06-25

### Fixed

- Fixed a potential null-reference in the Store when OnStoreUpdate is null
- Fixed a potential null-reference in the Store in ActionQueue

## [0.3.11] - 2024-04-23

### Fixed

- Correctly identified the `yohash.react.editor.asmdef` as an Editor-only Assembly Definition File, so the editor tools will not try to compile into builds

## [0.3.10] - 2024-04-08

### Changed

- Force the `Component<T>` generic type `T` to also implement `new()`, allowing us to store and instantiate the props in a `private T _props` variable. The props are instantiated with `new T()`. This removes some small amount of required boilerplate from component implementations.
- Removed local `Props` generation and storage in all associated demo scenes.

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