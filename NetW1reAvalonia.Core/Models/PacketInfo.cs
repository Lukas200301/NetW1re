using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;

namespace NetW1reAvalonia.Core.Models
{
    public class PacketInfo
    {
        public DateTime Timestamp { get; set; }
        public string Protocol { get; set; } = string.Empty;
        public IPAddress? SourceIP { get; set; }
        public IPAddress? DestinationIP { get; set; }
        public int? SourcePort { get; set; }
        public int? DestinationPort { get; set; }
        public int Length { get; set; }
        public string Info { get; set; } = string.Empty;
        public byte[] RawData { get; set; } = Array.Empty<byte>();
        public string SourceMAC { get; set; } = string.Empty;
        public string DestinationMAC { get; set; } = string.Empty;
        public string SourceHostname { get; set; } = string.Empty;
        public string DestinationHostname { get; set; } = string.Empty;
        public bool HostnamesResolved { get; set; } = false;
        public string EthernetType { get; set; } = string.Empty;
        public int? IpVersion { get; set; }
        public int? IpHeaderLength { get; set; }
        public int? Ttl { get; set; }
        public int? IpFlags { get; set; }
        public int? FragmentOffset { get; set; }
        public string? IpChecksum { get; set; }
        public int? TotalLength { get; set; }
        public int? Identification { get; set; }
        public int? TcpSequenceNumber { get; set; }
        public int? TcpAcknowledgmentNumber { get; set; }
        public int? TcpHeaderLength { get; set; }
        public string TcpFlags { get; set; } = string.Empty;
        public int? TcpWindowSize { get; set; }
        public string? TcpChecksum { get; set; }
        public int? TcpUrgentPointer { get; set; }
        public string? UdpChecksum { get; set; }
        public int? UdpLength { get; set; }
        public string IcmpType { get; set; } = string.Empty;
        public string IcmpCode { get; set; } = string.Empty;
        public string ArpOperation { get; set; } = string.Empty;
        public string ArpSenderIp { get; set; } = string.Empty;
        public string ArpTargetIp { get; set; } = string.Empty;
        public string ArpSenderMac { get; set; } = string.Empty;
        public string ArpTargetMac { get; set; } = string.Empty;
        public string PayloadPreview { get; set; } = string.Empty;
        public bool HasPayload => PayloadData.Length > 0;
        public byte[] PayloadData { get; set; } = Array.Empty<byte>();
        public string PayloadText { get; set; } = string.Empty;
        public string ApplicationData { get; set; } = string.Empty;
        public string DnsQuery { get; set; } = string.Empty;
        public string DnsQueryType { get; set; } = string.Empty;
        public string DnsResponse { get; set; } = string.Empty;
        public string HttpMethod { get; set; } = string.Empty;
        public string HttpHost { get; set; } = string.Empty;
        public string HttpUserAgent { get; set; } = string.Empty;
        public string HttpContentType { get; set; } = string.Empty;
        public string TlsSni { get; set; } = string.Empty;
        public string TlsVersion { get; set; } = string.Empty;
        public string FlowKey { get; set; } = string.Empty;
        public int PacketInFlow { get; set; } = 0;
        public long FlowStartTime { get; set; } = 0;
        public string Direction { get; set; } = string.Empty;
        public bool IsSuspicious { get; set; } = false;
        public string SuspiciousReason { get; set; } = string.Empty;
        public bool IsEncrypted { get; set; } = false;
        public Dictionary<string, object> AdditionalInfo { get; set; } = new();
        public string Source => SourceIP?.ToString() ?? (string.IsNullOrEmpty(SourceMAC) ? "-" : SourceMAC);
        public string Destination => DestinationIP?.ToString() ?? (string.IsNullOrEmpty(DestinationMAC) ? "-" : DestinationMAC);
        public string Ports => SourcePort.HasValue && DestinationPort.HasValue ? $"{SourcePort}→{DestinationPort}" : "-";
        public string PacketSummary => GetPacketSummary();
        public string HexDump => GetHexDump();
        
        public string GetHexDump()
        {
            if (RawData.Length == 0) return string.Empty;
            
            var lines = new List<string>();
            for (int i = 0; i < RawData.Length; i += 16)
            {
                var offset = i.ToString("X8");
                var hexBytes = string.Join(" ", RawData.Skip(i).Take(16).Select(b => b.ToString("X2")));
                var asciiChars = string.Concat(RawData.Skip(i).Take(16).Select(b => b >= 32 && b <= 126 ? (char)b : '.'));
                lines.Add($"{offset}  {hexBytes.PadRight(47)} |{asciiChars}|");
            }
            return string.Join(Environment.NewLine, lines);
        }
        
        public string GetPacketSummary()
        {
            var summary = new List<string>();
            
            summary.Add($"Timestamp: {Timestamp:yyyy-MM-dd HH:mm:ss.fff}");
            summary.Add($"Protocol: {Protocol}");
            summary.Add($"Length: {Length} bytes");
            
            if (!string.IsNullOrEmpty(SourceMAC) && !string.IsNullOrEmpty(DestinationMAC))
            {
                summary.Add($"Ethernet: {SourceMAC} → {DestinationMAC}");
                if (!string.IsNullOrEmpty(EthernetType))
                    summary.Add($"Ethernet Type: {EthernetType}");
            }
            
            if (SourceIP != null && DestinationIP != null)
            {
                summary.Add($"IP: {SourceIP} → {DestinationIP}");
                if (IpVersion.HasValue) summary.Add($"IP Version: {IpVersion}");
                if (Ttl.HasValue) summary.Add($"TTL: {Ttl}");
                if (TotalLength.HasValue) summary.Add($"Total Length: {TotalLength}");
            }
            
            if (SourcePort.HasValue && DestinationPort.HasValue)
            {
                summary.Add($"Ports: {SourcePort} → {DestinationPort}");
            }
            
            if (!string.IsNullOrEmpty(HttpMethod))
                summary.Add($"HTTP: {HttpMethod} {HttpHost}");
            
            if (!string.IsNullOrEmpty(DnsQuery))
                summary.Add($"DNS Query: {DnsQuery}");
            
            if (!string.IsNullOrEmpty(TlsSni))
                summary.Add($"TLS SNI: {TlsSni}");
            
            if (IsEncrypted)
                summary.Add("🔒 Encrypted Traffic");
            
            if (IsSuspicious)
                summary.Add($"⚠️ Suspicious: {SuspiciousReason}");
            
            if (HasPayload && !string.IsNullOrEmpty(PayloadText))
                summary.Add($"Data Preview: {PayloadText.Substring(0, Math.Min(100, PayloadText.Length))}...");
            
            return string.Join(Environment.NewLine, summary);
        }
    }
}
