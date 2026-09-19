# Changelog

All notable changes to this project are documented in this file.

## [Unreleased]

## [2.0.0] - 2026-09-19

### Changed

- **Breaking:** Zero year fractions and year components are now accepted; the minimum value is all zeros. Conversion at millennium boundaries follows the documented formula, and millennia above 999,999 are now rejected. Packed values previously valid are encoded the same way; persisted values outside the newly enforced range must be corrected before loading.
- Corrected Gregorian-boundary conversion and made tick encoding reversible across all supported values.

### Added

- The NuGet package now includes the changelog and license alongside the README and XML documentation.
- Reproducible builds with embedded PDBs (DotNet.ReproducibleBuilds).
