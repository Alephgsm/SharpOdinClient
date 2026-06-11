<p align="center">
  <img src="docs/banner.svg" alt="SharpOdinClient — C# Samsung Odin Protocol Library" width="100%"/>
</p>

# SharpOdinClient

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![.NET Framework](https://img.shields.io/badge/.NET_Framework-4.5.1+-512BD4)](https://dotnet.microsoft.com/)
[![Platform](https://img.shields.io/badge/Platform-Windows-0078D4)](https://www.microsoft.com/windows)

**SharpOdinClient** is a .NET library that lets C# applications talk to **Samsung Android devices in download mode (ODIN / LOKE)**. Use it to flash firmware (`image`, `tar`, `tar.md5`, `lz4`), read device info, read/write PIT, and build your own Odin-style tools — without wrapping the official closed `SS_DL.dll`.

USB communication uses the **Windows serial COM port** (Samsung CDC driver). No UsbDk dependency.

---

## SharpOdinClient Pro — commercial upgrade (Odin 3.14.4)

We now offer a **commercial, production-grade** successor built on the **Odin3 v3.14.4** protocol:

**[SharpOdinClient Pro — C# Odin Source Code (product page)](https://alephgsm.com/2026/06/11/sharpodinclient-pro/)**

| | **SharpOdinClient** (this repo) | **SharpOdinClient Pro** |
|---|---|---|
| **License** | GPL-3.0 (open source) | Commercial — ship inside closed-source products |
| **Protocol** | Earlier Odin generation | Reverse-engineered from **Odin3 v3.14.4** |
| **Framework** | .NET Framework 4.5.1 | .NET Framework 4.7.2 |
| **Android 15 / 16 `super.img`** | Known failures ([#18](https://github.com/Alephgsm/SharpOdinClient/issues/18)) | **Fixed** |
| **Secure-download auth (0x69)** | Not included | Full implementation |
| **Mass D/L, Home Binary, `.ock`** | Not included | Included |
| **GUI** | Library only | WinForms Odin3 clone + CLI |
| **Support** | Community | Private licensing & support |

**Choose this open-source library** if you need basic flash / info / PIT on older targets and you are fine with GPL-3.0.

**Choose [SharpOdinClient Pro](https://alephgsm.com/2026/06/11/sharpodinclient-pro/)** if you flash modern devices (Android 15/16), need secure-download authentication, Mass D/L, One Click / Home Binary workflows, or a **commercial license** for your own tool.

Contact for Pro licensing: [Telegram @GsmCoder](https://t.me/GsmCoder)

---

## Table of contents

- [Requirements](#requirements)
- [Features](#features)
- [Quick start](#quick-start)
- [Find devices in download mode](#find-devices-in-download-mode)
- [Read device info](#read-device-info)
- [Read PIT from device](#read-pit-from-device)
- [Write PIT to device](#write-pit-to-device)
- [Flash firmware (tar / tar.md5)](#flash-firmware-tar--tarmd5)
- [Flash a single file](#flash-a-single-file)
- [Events (Log & Progress)](#events-log--progress)
- [How it works](#how-it-works)
- [Known limitations](#known-limitations)
- [Related projects](#related-projects)
- [License](#license)
- [Disclaimer](#disclaimer)

---

## Requirements

1. **Windows** with the official **Samsung USB driver** installed.
2. **.NET Framework 4.5.1** or newer.
3. Device connected in **Download mode** (Odin mode).

---

## Features

- Auto-detect Samsung devices in download mode (VID/PID / COM port).
- Read device information (`DVIF` — model, CSC, firmware version, unique id, etc.).
- Read PIT from the device.
- Write PIT (from `.pit` or from a `tar` / `tar.md5` that contains a PIT member).
- Flash `tar`, `tar.md5` packages (including `lz4`, `img`, `bin` members inside TAR).
- Flash a single file to a named partition (e.g. `boot.img`, `sboot.bin`).
- Async API with `Log` and `ProgressChanged` events.
- Pure managed C# — no dependency on `SS_DL.dll`.

---

## Quick start

### Namespaces

```csharp
using SharpOdinClient.structs;
using SharpOdinClient.util;
```

### Create an instance and subscribe to events

```csharp
private readonly Odin _odin = new Odin();

public MainWindow()
{
    InitializeComponent();
    _odin.Log += OnOdinLog;
    _odin.ProgressChanged += OnOdinProgress;
}

private void OnOdinProgress(string filename, long max, long value, long writtenSize)
{
    // filename: partition/file being flashed
    // max: total size
    // value / writtenSize: bytes written
}

private void OnOdinLog(string text, utils.MsgType color)
{
    // text: log line
    // color: message type for UI coloring
}
```

---

## Find devices in download mode

```csharp
var device = await _odin.FindDownloadModePort();

Console.WriteLine(device.Name);  // device name
Console.WriteLine(device.COM);    // COM port number
Console.WriteLine(device.VID);    // USB VID
Console.WriteLine(device.PID);    // USB PID
```

To open the session on the found port:

```csharp
if (await _odin.FindAndSetDownloadMode())
{
  // ready for LOKE_Initialize / flash operations
}
```

---

## Read device info

After `FindAndSetDownloadMode()`:

```csharp
var info = await _odin.DVIF();
await _odin.PrintInfo();
```

Common keys in the `info` dictionary:

| Key | Meaning |
|-----|---------|
| `capa` | Capa number |
| `product` | Product id |
| `model` | Model number |
| `fwver` | Firmware version |
| `vendor` | Vendor |
| `sales` | Sales code |
| `ver` | Build number |
| `did` | DID number |
| `un` | Unique id |
| `tmu_temp` | TMU number |
| `prov` | Provision |

---

## Read PIT from device

Most download-mode operations require `IsOdin()` and `LOKE_Initialize` first. Use `totalfilesize = 0` when you are not flashing.

```csharp
if (await _odin.FindAndSetDownloadMode())
{
    await _odin.PrintInfo();

    if (await _odin.IsOdin())
    {
        if (await _odin.LOKE_Initialize(0))
        {
            var pit = await _odin.Read_Pit();
            if (pit.Result)
            {
                byte[] pitBytes = pit.data;           // raw PIT file bytes
                var entries = pit.Pit;                // partition table entries
            }
        }
    }
}
```

---

## Write PIT to device

Parameter `pit` can be:

- A standalone `.pit` file path, or
- A `tar` / `tar.md5` path that contains a PIT member (e.g. CSC package).

```csharp
if (await _odin.FindAndSetDownloadMode())
{
    await _odin.PrintInfo();

    if (await _odin.IsOdin())
    {
        if (await _odin.LOKE_Initialize(0))
        {
            bool ok = (await _odin.Write_Pit("path\\to\\device.pit")).status;
        }
    }
}
```

---

## Flash firmware (tar / tar.md5)

Build a `List<FileFlash>` from your BL / AP / CP / CSC (and optional) packages, then flash:

```csharp
public async Task<bool> FlashFirmware(List<string> tarPaths)
{
    var flashFiles = new List<FileFlash>();

    foreach (var tarPath in tarPaths)
    {
        var members = _odin.tar.TarInformation(tarPath);
        foreach (var member in members)
        {
            if (AlreadyAdded(member, flashFiles)) continue;

            var ext = Path.GetExtension(member.Filename);
            var file = new FileFlash
            {
                Enable   = true,
                FileName = member.Filename,
                FilePath = tarPath
            };

            if (ext == ".pit")
            {
                // PIT inside package — handle repartition separately if needed
            }
            else if (ext == ".lz4")
            {
                file.RawSize = _odin.CalculateLz4SizeFromTar(tarPath, member.Filename);
            }
            else
            {
                file.RawSize = member.Filesize;
            }

            flashFiles.Add(file);
        }
    }

    if (flashFiles.Count == 0) return false;

    long totalSize = flashFiles.Sum(f => f.RawSize);

    if (!await _odin.FindAndSetDownloadMode()) return false;
    await _odin.PrintInfo();

    if (!await _odin.IsOdin()) return false;
    if (!await _odin.LOKE_Initialize(totalSize)) return false;

    // Optional: repartition if PIT is inside the package
    var pitInTar = flashFiles.Find(x => x.FileName.EndsWith(".pit", StringComparison.OrdinalIgnoreCase));
    if (pitInTar != null)
    {
        // await _odin.Write_Pit(pitInTar.FilePath);
    }

    var readPit = await _odin.Read_Pit();
    if (!readPit.Result) return false;

    const int efsClear = 0;
    const int bootUpdate = 1;

    if (!await _odin.FlashFirmware(flashFiles, readPit.Pit, efsClear, bootUpdate, autoReboot: true))
        return false;

    return await _odin.PDAToNormal();
}
```

Set `FileFlash.Enable = false` to skip a member without removing it from the list.

---

## Flash a single file

```csharp
public async Task<bool> FlashSingleFile(string filePath, string partitionFileName)
{
    var flashFile = new FileFlash
    {
        Enable   = true,
        FileName = partitionFileName,   // e.g. "boot.img"
        FilePath = filePath,
        RawSize  = new FileInfo(filePath).Length
    };

    if (!await _odin.FindAndSetDownloadMode()) return false;
    await _odin.PrintInfo();

    if (!await _odin.IsOdin()) return false;
    if (!await _odin.LOKE_Initialize(flashFile.RawSize)) return false;

    var readPit = await _odin.Read_Pit();
    if (!readPit.Result) return false;

    const int efsClear = 0;
    const int bootUpdate = 0;

    if (!await _odin.FlashSingleFile(flashFile, readPit.Pit, efsClear, bootUpdate, autoReboot: true))
        return false;

    return await _odin.PDAToNormal();
}
```

---

## Events (Log & Progress)

| Event | Purpose |
|-------|---------|
| `Log` | Text log lines with `MsgType` for UI coloring |
| `ProgressChanged` | `filename`, `max`, `value`, `writtenSize` during flash |

---

## How it works

1. Install the Samsung USB driver.
2. Boot the phone into **Download mode** and connect via USB.
3. SharpOdinClient finds the COM port (`FindDownloadModePort` / `FindAndSetDownloadMode`).
4. Handshake with the bootloader (`IsOdin`, `LOKE_Initialize`).
5. Read PIT, then stream firmware over the serial protocol.
6. Optionally reboot to normal mode (`PDAToNormal`).

Communication is **SerialPort-based** — the same transport class used by the official Odin ecosystem on Windows.

---

## Known limitations

This open-source library targets an **earlier Odin protocol generation**. On newer Samsung devices you may hit:

- **`super.img` flash failures on Android 15 / 16** — see [issue #18](https://github.com/Alephgsm/SharpOdinClient/issues/18) (e.g. Galaxy A55, Galaxy S25).
- No **secure-download authentication (command 0x69)** for protocol v4+ devices.
- No **Mass D/L**, **Home Binary / `download-list.txt`**, or **One Click (`.ock`)** workflows.
- No built-in GUI — you integrate the library into your own app.

For production use on modern devices, see **[SharpOdinClient Pro](https://alephgsm.com/2026/06/11/sharpodinclient-pro/)**.

---

## Related projects

| Project | Description |
|---------|-------------|
| **[SharpOdinClient Pro](https://alephgsm.com/2026/06/11/sharpodinclient-pro/)** | Commercial Odin 3.14.4 C# source code (this library’s successor) |
| [Freya](https://alephgsm.com/2022/11/14/sharpodinclient-samsung-devices-flash-library-in-c/) | .NET Samsung flash tool built on SharpOdinClient |
| [GSM Alphabet](https://alephgsm.com/) | Source code & protocol research |

---

## License

**GNU General Public License v3.0** — see [License](License) in this repository.

If you integrate SharpOdinClient into your project, GPL-3.0 obligations apply (including share-alike for distributed derivatives). For a **commercial, closed-source license**, use [SharpOdinClient Pro](https://alephgsm.com/2026/06/11/sharpodinclient-pro/) instead.

---

## Contact

- **SharpOdinClient Pro / licensing:** [Telegram @GsmCoder](https://t.me/GsmCoder)
- **Product page:** [alephgsm.com/2026/06/11/sharpodinclient-pro/](https://alephgsm.com/2026/06/11/sharpodinclient-pro/)
- **Website:** [alephgsm.com](https://alephgsm.com/)

---

## Disclaimer

SharpOdinClient is intended for **lawful development, research, maintenance and authorized service** only. Flashing firmware can brick devices. Behavior depends on chipset, bootloader, firmware package, and security state. Use only on devices you own or are authorized to service, at your own risk.

---

**Author:** Alephgsm / GSM Alphabet
