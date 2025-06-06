# NetW1re

<div align="center">
  <img src="netw1re.png" alt="NetW1re Logo" width="200px" />
  <br>
  <h3>Advanced Network Monitoring and Control Tool</h3>
  
  ![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)
  ![Avalonia UI](https://img.shields.io/badge/Avalonia_UI-11.0-2389FD)
  ![License](https://img.shields.io/badge/License-MIT-green.svg)
  ![Platform](https://img.shields.io/badge/Platform-Windows%20|%20Linux-lightgrey)
</div>

## 🌟 Overview

NetW1re is a modern network management tool that allows you to monitor, analyze, and control the traffic of devices on your local network. Built with Avalonia UI and following the MVVM pattern, NetW1re provides a sleek, cross-platform experience for network administrators and power users.

## ✨ Key Features

- **Real-time Device Discovery**: Automatically detect and monitor all devices on your network
- **Traffic Control**: Block, redirect, or limit bandwidth for specific devices
- **Packet Analysis**: Monitor and analyze network traffic in real-time
- **Rule Builder**: Create custom rules for automated device management
- **Friendly UI**: Modern interface with customizable themes and notifications
- **Cross-platform**: Works on Windows and Linux (macOS support coming soon)

## 📥 Installation

### Requirements

- **Npcap Driver**: [Download from nmap.org](https://nmap.org/download.html) (Windows only)
- **.NET 9.0 Runtime**: [Download from Microsoft](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Administrator/Root access**: Required for network traffic manipulation

### Download

See [Releases](https://github.com/lukas200301/NetW1re/releases) for the latest binaries.

### Building from Source

```powershell
# Clone the repository
git clone https://github.com/lukas200301/NetW1re.git
cd NetW1re

# Build the project
dotnet build

# Run the application
# Windows
cd NetW1reAvalonia.Windows/bin/Debug/net9.0
./NetW1reAvalonia.Windows.exe

# Linux
cd NetW1reAvalonia.Linux/bin/Debug/net9.0
./NetW1reAvalonia.Linux
```

## 📊 Usage

### Device Management

1. **Scan Network**: Click the "Scan" button to discover devices
2. **Device Control**: Right-click on any device for options:
   - Block/Unblock internet access
   - Redirect traffic
   - Limit bandwidth
   - Set friendly name

### Traffic Monitoring

1. Navigate to the "Sniffer" tab
2. Select a network interface
3. Start capturing packets
4. Filter and analyze traffic

### Rule Builder

1. Navigate to the "Rule Builder" tab
2. Create rules based on:
   - MAC address
   - IP address
   - Hostname
   - Time of day
3. Set actions (block, redirect, limit)
4. Enable/disable rules as needed

## 📋 Status Indicators

- 🔴 **Red**: Device is blocked from accessing the internet
- 🟡 **Yellow**: Device traffic is being redirected
- 🔵 **Blue**: Device has bandwidth limitations

## 🔧 Troubleshooting

### Device Not Appearing
- Ensure the device is connected to the same network
- Wait a few minutes for automatic discovery
- Use the 'Refresh' button to force a new scan
- Check if the device is in sleep/power-saving mode

### Actions Not Working
- Verify you have administrator privileges
- Check network adapter compatibility
- Ensure target device is still online
- Restart NetW1re if issues persist

### Performance Issues
- Reduce scan frequency if CPU usage is high
- Close packet monitoring when not needed
- Limit the number of active rules
- Check available system memory

## 🔄 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- [SharpPcap](https://github.com/dotpcap/sharppcap) for packet capture capabilities
- [Avalonia UI](https://avaloniaui.net/) for the cross-platform UI framework
- [ReactiveUI](https://reactiveui.net/) for the reactive programming model
- [Npcap](https://nmap.org/npcap/) for packet capture driver

---

<div align="center">
  <sub>Built with ❤️ by Lukas</sub>
</div>
