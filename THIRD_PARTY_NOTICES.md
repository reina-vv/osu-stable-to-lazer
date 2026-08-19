# Third-Party Notices

## osu!lazer code packages

This project references the following NuGet packages from the osu! ecosystem:

- `ppy.osu.Game` 2026.730.0
- `ppy.osu.Game.Rulesets.Osu` 2026.730.0
- `ppy.osu.Game.Rulesets.Catch` 2026.730.0
- `ppy.osu.Game.Rulesets.Taiko` 2026.730.0
- `ppy.osu.Game.Rulesets.Mania` 2026.730.0

The source code for these packages is provided by ppy Pty Ltd under the MIT
License. Source and licence text are available at
https://github.com/ppy/osu and https://github.com/ppy/osu/blob/master/LICENCE.

## osu!lazer game resources

`ppy.osu.Game` restores `ppy.osu.Game.Resources` as a transitive runtime
dependency. This repository does not contain or distribute that package. It is
retrieved by NuGet only when a user restores and builds locally.

The majority of osu! game resources are licensed under CC-BY-NC 4.0, and some
fonts have separate licences. Any use or redistribution of restored resources
or compiled output must comply with those terms. See
https://github.com/ppy/osu-resources for the current resource licence details.

## Other dependencies and trademarks

The dependency graph includes additional NuGet packages. Their licence terms
apply when compiling, using, or distributing the resulting software. Anyone
creating a binary distribution is responsible for reviewing those terms and
including the required notices and licence texts.

`osu!`, `osu`, `lazer`, and ppy-related branding are trademarks or brand assets
of ppy Pty Ltd. This project does not claim ownership of, or permission to use,
those marks beyond necessary descriptive reference.
