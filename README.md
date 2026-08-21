<p align="center">
  <img src="assets/icon.png" alt="Honkai Impact 3rd DiscordRPC" width="200" />
</p>

<h1 align="center">Honkai Impact 3rd DiscordRPC</h1>

<p align="center">
  Discord <a href="https://discord.com/developers/docs/topics/rich-presence">Rich Presence</a> for <a href="https://honkai.hoyoverse.com/">Honkai Impact 3rd</a>.
</p>

---

## Preview

![preview](https://github.com/KiraiEEE/Honkai-RPC/assets/54278089/03fe83f6-d2b0-4a38-9bd4-db67278ef83a)

## Features

- Shows your current Honkai Impact 3rd session on Discord
- System tray icon with AutoStart toggle
- Single instance only

## Usage

1. Download the latest release from [Releases](https://github.com/KiraiEEE/Honkai-Impact-3rd-RPC/releases)
2. Run `HonkaiImpactRpc.exe`
3. The app will sit in your system tray and automatically detect the game

## AutoStart

Right-click the tray icon to toggle AutoStart on or off. When enabled, the app launches automatically on login.

## Building

Requires Visual Studio 2022 with .NET desktop development workload, or MSBuild with .NET Framework 4.8 targeting pack.

```bash
msbuild Honkai-Impact-3rd-RPC.sln /p:Configuration=Release
```

Output will be in `src\bin\Release\`.

## License

This project is licensed under the [MIT License](LICENSE.md).

## Acknowledgements

Forked from [StarRailRpc](https://github.com/Kxnrl/StarRailRpc) by Kxnrl.
