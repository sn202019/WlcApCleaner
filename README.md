# WLC AP Cleaner

WLC AP Cleaner is a C# WPF application designed to help network administrators identify and remove inactive Cisco access points (APs) from a Cisco 9800 Wireless LAN Controller (WLC). By comparing the configured APs in the WLC’s running configuration with the currently active APs shown in the AP summary, the tool generates the necessary CLI commands to remove stale AP entries.

- **Inactive AP Detection**: Compares APs in the running configuration (`show run`) with those active in the AP summary (`show ap summary`) and identifies inactive APs.
- **CLI Command Generation**: Generates CLI commands using both `no username <mac>` and `no ap <dot.mac>` formats for easy removal.
- **User-Friendly Interface**: Displays inactive APs in a selectable list with search and bulk select/deselect features.
- **Description Display**: Shows the AP description along with its MAC address.
- **Export Functionality**: Generated CLI commands can be reviewed and copied directly.
- **Installation via MSI**: The application is packaged with an installer for easy deployment.

---

## Getting Started

### Prerequisites

- Windows 10 or later
- [.NET Desktop Runtime 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) or later

---

## Installation

1. Download the latest `.msi` installer from the [GitHub Releases](../../releases) section.
2. Run the installer to install the application on your system.
3. Launch **WLC AP Cleaner** from the Start Menu or desktop shortcut.

---

## Usage

1. Load the **Show Run Config** file exported from the WLC.
2. Load the **AP Summary** file (e.g., output of `show ap summary`).
3. Click **Compare** to identify inactive APs.
4. Select APs to be deleted using checkboxes, or use **Select All/Deselect All** buttons.
5. Click **Generate Delete CLI** to generate the commands.
6. Copy the commands and paste into your WLC CLI.

---

## Output Example

The CLI command output includes lines such as:

```
no username 00a1.b2c3.d4b6
no ap 00a1.b2.c3b6
```

This ensures complete cleanup of both the AP and associated username from the WLC.

---

## Release

The current version **v1.0.0** is available via the [GitHub Releases](../../releases) page.

---

## License

This project is licensed under the [MIT License](LICENSE).
