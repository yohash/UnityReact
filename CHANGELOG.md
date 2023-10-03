# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.2.1] - 2023-10-03

### Changed

- Swapped all sample-scene action types from `class` to `struct`

## [0.2.0] - 2023-10-02

### Changed

- `public abstract class Action` changed to `public interface IAction`
  - The base type is primarily used for pattern matching.
  - This type allows the freedom of passing structs.
- Upgrade Unity version to 2021.3.28f1