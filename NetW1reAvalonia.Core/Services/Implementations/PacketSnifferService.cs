using NetW1reAvalonia.Core.Models;
using NetW1reAvalonia.Core.Services;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetW1reAvalonia.Core.Services.Implementations
{
    public class PacketSnifferService : IPacketSnifferService
    {
        private ICaptureDevice? _captureDevice;
        private bool _isCapturing;        public ObservableCollection<PacketInfo> CapturedPackets { get; } = new();
        public bool IsCapturing => _isCapturing;

        public event EventHandler<PacketInfo>? PacketCaptured;

        public async Task<string[]> GetNetworkDevicesAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var devices = CaptureDeviceList.Instance;
                    return devices.Select(d => d.Description ?? d.Name ?? "Unknown Device").ToArray();
                }
                catch
                {
                    return Array.Empty<string>();
                }
            });
        }

        public async Task StartCaptureAsync(string deviceName)
        {
            if (_isCapturing)
                return;

            await Task.Run(() =>
            {
                try
                {
                    var devices = CaptureDeviceList.Instance;
                    _captureDevice = devices.FirstOrDefault(d => 
                        d.Description == deviceName || d.Name == deviceName);

                    if (_captureDevice == null)
                        return;                    _captureDevice.OnPacketArrival += OnPacketArrival;
                    _captureDevice.Open(DeviceModes.Promiscuous, 1000);
                    _captureDevice.StartCapture();
                    _isCapturing = true;
                }
                catch
                {
                    // Log error - for now just stop capture
                    _isCapturing = false;
                    _captureDevice?.Close();
                }
            });
        }

        public async Task StopCaptureAsync()
        {
            if (!_isCapturing || _captureDevice == null)
                return;

            await Task.Run(() =>
            {
                try
                {
                    _captureDevice.StopCapture();
                    _captureDevice.OnPacketArrival -= OnPacketArrival;
                    _captureDevice.Close();
                    _isCapturing = false;
                }
                catch
                {
                    // Ignore errors during shutdown
                }
                finally
                {
                    _captureDevice = null;
                    _isCapturing = false;
                }
            });
        }        public void ClearPackets()
        {
            CapturedPackets.Clear();
        }        private void OnPacketArrival(object sender, PacketCapture e)
        {
            try
            {
                var packet = Packet.ParsePacket(e.GetPacket().LinkLayerType, e.GetPacket().Data);                // Convert UTC timestamp to local time
                var localTimestamp = e.GetPacket().Timeval.Date.ToLocalTime();
                var packetInfo = ParsePacket(packet, localTimestamp);
                
                // Add to collection on UI thread
                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    CapturedPackets.Add(packetInfo);
                    PacketCaptured?.Invoke(this, packetInfo);
                });
            }
            catch
            {
                // Ignore parsing errors
            }        }

        private PacketInfo ParsePacket(Packet packet, DateTime timestamp)
        {
            var packetInfo = new PacketInfo
            {
                Timestamp = timestamp,
                Length = packet.Bytes?.Length ?? 0,
                RawData = packet.Bytes ?? Array.Empty<byte>()
            };

            // Parse Ethernet frame
            if (packet is EthernetPacket ethernetPacket)
            {
                packetInfo.SourceMAC = ethernetPacket.SourceHardwareAddress.ToString();
                packetInfo.DestinationMAC = ethernetPacket.DestinationHardwareAddress.ToString();
                packetInfo.EthernetType = ethernetPacket.Type.ToString();

                // Parse IP packet
                if (ethernetPacket.PayloadPacket is IPPacket ipPacket)
                {
                    packetInfo.SourceIP = ipPacket.SourceAddress;
                    packetInfo.DestinationIP = ipPacket.DestinationAddress;
                    packetInfo.Protocol = ipPacket.Protocol.ToString();
                      // Enhanced IP parsing
                    packetInfo.IpVersion = (int)ipPacket.Version;
                    packetInfo.IpHeaderLength = ipPacket.HeaderLength;
                    packetInfo.Ttl = ipPacket.TimeToLive;
                    packetInfo.TotalLength = ipPacket.TotalLength;
                    
                    // Check if it's IPv4 for additional fields
                    if (ipPacket is IPv4Packet ipv4Packet)
                    {
                        packetInfo.Identification = ipv4Packet.Id;
                        packetInfo.IpChecksum = $"0x{ipv4Packet.Checksum:X4}";
                        packetInfo.IpFlags = (int)ipv4Packet.FragmentFlags;
                        packetInfo.FragmentOffset = ipv4Packet.FragmentOffset;
                    }
                    
                    // Extract payload data
                    if (ipPacket.PayloadData != null && ipPacket.PayloadData.Length > 0)
                    {
                        packetInfo.PayloadData = ipPacket.PayloadData;
                        packetInfo.PayloadPreview = GetPayloadPreview(ipPacket.PayloadData);
                    }

                    // Parse TCP packet
                    if (ipPacket.PayloadPacket is TcpPacket tcpPacket)
                    {
                        packetInfo.Protocol = "TCP";
                        packetInfo.SourcePort = tcpPacket.SourcePort;
                        packetInfo.DestinationPort = tcpPacket.DestinationPort;
                        packetInfo.Info = $"TCP {tcpPacket.SourcePort} → {tcpPacket.DestinationPort}";
                        
                        // Enhanced TCP parsing
                        packetInfo.TcpSequenceNumber = (int)tcpPacket.SequenceNumber;
                        packetInfo.TcpAcknowledgmentNumber = (int)tcpPacket.AcknowledgmentNumber;
                        packetInfo.TcpHeaderLength = tcpPacket.DataOffset;
                        packetInfo.TcpWindowSize = tcpPacket.WindowSize;
                        packetInfo.TcpChecksum = $"0x{tcpPacket.Checksum:X4}";
                        packetInfo.TcpUrgentPointer = tcpPacket.UrgentPointer;                        // Parse TCP flags - simplified approach
                        var flags = new List<string>();
                        try
                        {
                            // Try to get flags using basic properties that should exist
                            var flagsValue = tcpPacket.ToString();
                            if (flagsValue.Contains("SYN")) flags.Add("SYN");
                            if (flagsValue.Contains("ACK")) flags.Add("ACK");
                            if (flagsValue.Contains("FIN")) flags.Add("FIN");
                            if (flagsValue.Contains("RST")) flags.Add("RST");
                            if (flagsValue.Contains("PSH")) flags.Add("PSH");
                            if (flagsValue.Contains("URG")) flags.Add("URG");
                        }
                        catch
                        {
                            flags.Add("Unknown");
                        }
                        packetInfo.TcpFlags = string.Join(", ", flags);
                          // Get TCP payload
                        if (tcpPacket.PayloadData != null && tcpPacket.PayloadData.Length > 0)
                        {
                            packetInfo.PayloadData = tcpPacket.PayloadData;
                            packetInfo.PayloadPreview = GetPayloadPreview(tcpPacket.PayloadData);
                            packetInfo.PayloadText = GetPayloadText(tcpPacket.PayloadData);
                            
                            // Parse application layer protocols
                            ParseApplicationProtocols(packetInfo, tcpPacket.PayloadData, tcpPacket.SourcePort, tcpPacket.DestinationPort);
                        }
                    }
                    // Parse UDP packet
                    else if (ipPacket.PayloadPacket is UdpPacket udpPacket)
                    {
                        packetInfo.Protocol = "UDP";
                        packetInfo.SourcePort = udpPacket.SourcePort;
                        packetInfo.DestinationPort = udpPacket.DestinationPort;
                        packetInfo.Info = $"UDP {udpPacket.SourcePort} → {udpPacket.DestinationPort}";
                        
                        // Enhanced UDP parsing
                        packetInfo.UdpChecksum = $"0x{udpPacket.Checksum:X4}";
                          // Get UDP payload
                        if (udpPacket.PayloadData != null && udpPacket.PayloadData.Length > 0)
                        {
                            packetInfo.PayloadData = udpPacket.PayloadData;
                            packetInfo.PayloadPreview = GetPayloadPreview(udpPacket.PayloadData);
                            packetInfo.PayloadText = GetPayloadText(udpPacket.PayloadData);
                            
                            // Parse application layer protocols
                            ParseApplicationProtocols(packetInfo, udpPacket.PayloadData, udpPacket.SourcePort, udpPacket.DestinationPort);
                        }
                    }
                    // Parse ICMP packet
                    else if (ipPacket.PayloadPacket is IcmpV4Packet icmpPacket)
                    {
                        packetInfo.Protocol = "ICMP";
                        packetInfo.Info = $"ICMP {icmpPacket.TypeCode}";
                        
                        // Enhanced ICMP parsing
                        packetInfo.IcmpType = icmpPacket.TypeCode.ToString();
                        packetInfo.IcmpCode = icmpPacket.TypeCode.ToString();
                        
                        // Get ICMP payload
                        if (icmpPacket.PayloadData != null && icmpPacket.PayloadData.Length > 0)
                        {
                            packetInfo.PayloadData = icmpPacket.PayloadData;
                            packetInfo.PayloadPreview = GetPayloadPreview(icmpPacket.PayloadData);
                        }
                    }
                    else
                    {
                        packetInfo.Info = $"{ipPacket.Protocol} packet";
                    }
                }
                // Parse ARP packet
                else if (ethernetPacket.PayloadPacket is ArpPacket arpPacket)
                {
                    packetInfo.Protocol = "ARP";
                    packetInfo.Info = $"ARP {arpPacket.Operation}";
                    
                    // Enhanced ARP parsing
                    packetInfo.ArpOperation = arpPacket.Operation.ToString();
                    packetInfo.ArpSenderIp = arpPacket.SenderProtocolAddress?.ToString() ?? "";
                    packetInfo.ArpTargetIp = arpPacket.TargetProtocolAddress?.ToString() ?? "";
                    packetInfo.ArpSenderMac = arpPacket.SenderHardwareAddress?.ToString() ?? "";
                    packetInfo.ArpTargetMac = arpPacket.TargetHardwareAddress?.ToString() ?? "";
                }
                else
                {
                    packetInfo.Protocol = ethernetPacket.Type.ToString();
                    packetInfo.Info = "Ethernet frame";
                }
            }
            else
            {
                packetInfo.Protocol = "Unknown";
                packetInfo.Info = "Unknown packet type";
            }

            return packetInfo;
        }        private string GetPayloadPreview(byte[] payloadData, int maxLength = 200)
        {
            if (payloadData == null || payloadData.Length == 0)
                return string.Empty;

            // Try to show printable ASCII characters
            var preview = new StringBuilder();
            int bytesToShow = Math.Min(payloadData.Length, maxLength);
            
            for (int i = 0; i < bytesToShow; i++)
            {
                byte b = payloadData[i];
                if (b >= 32 && b <= 126) // Printable ASCII
                {
                    preview.Append((char)b);
                }
                else if (b == 10) // Line feed
                {
                    preview.Append("\\n");
                }
                else if (b == 13) // Carriage return
                {
                    preview.Append("\\r");
                }
                else if (b == 9) // Tab
                {
                    preview.Append("\\t");
                }
                else
                {
                    preview.Append($"\\x{b:X2}");
                }
            }
            
            if (payloadData.Length > maxLength)
            {
                preview.Append("...");
            }
            
            return preview.ToString();
        }

        private string GetPayloadText(byte[] payloadData)
        {
            if (payloadData == null || payloadData.Length == 0)
                return string.Empty;

            try
            {
                // Try to decode as UTF-8 first
                var text = Encoding.UTF8.GetString(payloadData);
                
                // Check if it contains mostly printable characters
                int printableCount = text.Count(c => char.IsControl(c) == false || char.IsWhiteSpace(c));
                if (printableCount > text.Length * 0.7) // If 70% or more are printable
                {
                    return text;
                }
                
                // Fall back to ASCII
                return Encoding.ASCII.GetString(payloadData.Where(b => b >= 32 && b <= 126).ToArray());
            }
            catch
            {
                return string.Empty;
            }
        }

        private void ParseApplicationProtocols(PacketInfo packetInfo, byte[] payload, int sourcePort, int destinationPort)
        {
            if (payload == null || payload.Length == 0)
                return;

            var payloadText = GetPayloadText(payload);
            
            // Check for HTTP
            if (IsHttpPort(sourcePort) || IsHttpPort(destinationPort) || IsHttpData(payloadText))
            {
                ParseHttp(packetInfo, payloadText);
            }
            
            // Check for DNS
            else if (sourcePort == 53 || destinationPort == 53)
            {
                ParseDns(packetInfo, payload);
            }
            
            // Check for TLS/SSL
            else if (IsTlsPort(sourcePort) || IsTlsPort(destinationPort) || IsTlsData(payload))
            {
                ParseTls(packetInfo, payload);
                packetInfo.IsEncrypted = true;
            }
            
            // Store application data
            if (!string.IsNullOrEmpty(payloadText))
            {
                packetInfo.ApplicationData = payloadText.Length > 1000 ? 
                    payloadText.Substring(0, 1000) + "..." : payloadText;
            }
            
            // Generate flow key
            var smaller = Math.Min(sourcePort, destinationPort);
            var larger = Math.Max(sourcePort, destinationPort);
            packetInfo.FlowKey = $"{packetInfo.SourceIP}:{smaller}-{packetInfo.DestinationIP}:{larger}";
        }

        private bool IsHttpPort(int port) => port == 80 || port == 8080 || port == 8000 || port == 3000;
        private bool IsTlsPort(int port) => port == 443 || port == 8443 || port == 993 || port == 995 || port == 465;

        private bool IsHttpData(string data) =>
            data.StartsWith("GET ") || data.StartsWith("POST ") || data.StartsWith("PUT ") ||
            data.StartsWith("DELETE ") || data.StartsWith("HEAD ") || data.StartsWith("OPTIONS ") ||
            data.StartsWith("HTTP/");

        private bool IsTlsData(byte[] data) =>
            data.Length >= 3 && data[0] == 0x16 && data[1] == 0x03;

        private void ParseHttp(PacketInfo packetInfo, string data)
        {
            var lines = data.Split('\n');
            if (lines.Length == 0) return;

            var firstLine = lines[0].Trim();
            
            // Parse request line
            if (firstLine.StartsWith("GET ") || firstLine.StartsWith("POST ") || 
                firstLine.StartsWith("PUT ") || firstLine.StartsWith("DELETE ") ||
                firstLine.StartsWith("HEAD ") || firstLine.StartsWith("OPTIONS "))
            {
                var parts = firstLine.Split(' ');
                if (parts.Length >= 2)
                {
                    packetInfo.HttpMethod = parts[0];
                }
            }

            // Parse headers
            foreach (var line in lines.Skip(1))
            {
                if (line.StartsWith("Host: ", StringComparison.OrdinalIgnoreCase))
                {
                    packetInfo.HttpHost = line.Substring(6).Trim();
                }
                else if (line.StartsWith("User-Agent: ", StringComparison.OrdinalIgnoreCase))
                {
                    packetInfo.HttpUserAgent = line.Substring(12).Trim();
                }
                else if (line.StartsWith("Content-Type: ", StringComparison.OrdinalIgnoreCase))
                {
                    packetInfo.HttpContentType = line.Substring(14).Trim();
                }
            }
        }

        private void ParseDns(PacketInfo packetInfo, byte[] data)
        {
            try
            {
                // Very basic DNS parsing - just extract domain names from the query section
                if (data.Length < 12) return; // DNS header is 12 bytes

                // Skip DNS header (12 bytes)
                int offset = 12;
                
                // Try to extract the query name
                var queryName = ExtractDnsName(data, ref offset);
                if (!string.IsNullOrEmpty(queryName))
                {
                    packetInfo.DnsQuery = queryName;
                }
            }
            catch
            {
                // DNS parsing failed, ignore
            }
        }

        private string ExtractDnsName(byte[] data, ref int offset)
        {
            var name = new StringBuilder();
            
            while (offset < data.Length)
            {
                int length = data[offset++];
                if (length == 0) break; // End of name
                
                if (length > 63) // Pointer or invalid
                    break;
                
                if (name.Length > 0)
                    name.Append('.');
                
                for (int i = 0; i < length && offset < data.Length; i++)
                {
                    name.Append((char)data[offset++]);
                }
            }
            
            return name.ToString();
        }

        private void ParseTls(PacketInfo packetInfo, byte[] data)
        {
            try
            {
                if (data.Length < 5) return;

                // TLS record header: type(1) + version(2) + length(2)
                if (data[0] == 0x16) // Handshake
                {
                    packetInfo.TlsVersion = $"TLS {data[1]}.{data[2]}";
                    
                    // Try to extract SNI from Client Hello
                    if (data.Length > 43) // Minimum size for SNI parsing
                    {
                        var sni = ExtractSni(data);
                        if (!string.IsNullOrEmpty(sni))
                        {
                            packetInfo.TlsSni = sni;
                        }
                    }
                }
            }
            catch
            {
                // TLS parsing failed, ignore
            }
        }

        private string ExtractSni(byte[] data)
        {
            try
            {
                // This is a very simplified SNI extraction
                // In a real implementation, you'd want proper TLS parsing
                
                var dataStr = Encoding.ASCII.GetString(data);
                
                // Look for common patterns that might indicate SNI
                var commonDomains = new[] { ".com", ".org", ".net", ".io", ".dev", ".co" };
                
                foreach (var domain in commonDomains)
                {
                    int index = dataStr.IndexOf(domain);
                    if (index > 0)
                    {
                        // Try to extract the domain name
                        int start = index;
                        while (start > 0 && (char.IsLetterOrDigit(dataStr[start - 1]) || dataStr[start - 1] == '.' || dataStr[start - 1] == '-'))
                        {
                            start--;
                        }
                        
                        int end = index + domain.Length;
                        var possibleDomain = dataStr.Substring(start, end - start);
                        
                        if (possibleDomain.Contains('.') && possibleDomain.Length > 3)
                        {
                            return possibleDomain;
                        }
                    }
                }
                
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public void Dispose()
        {
            StopCaptureAsync().Wait();
        }
    }
}
