using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class DeviceInfo
{
    public string MacAddress { get; set; } = string.Empty;
    public string Identity { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
}


public class MNDPDiscovery
{
    private const int MNDP_PORT = 5678;

    public static void DiscoverDevices(Action<DeviceInfo> onDeviceDiscovered)
    {
        UdpClient udpClient = new UdpClient();
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, MNDP_PORT);

        byte[] discoveryPacket = new byte[1];
        udpClient.Send(discoveryPacket, discoveryPacket.Length, endPoint);

        udpClient.BeginReceive(new AsyncCallback((ar) => ReceiveCallback(ar, onDeviceDiscovered)), udpClient);
    }

    private static void ReceiveCallback(IAsyncResult ar, Action<DeviceInfo> onDeviceDiscovered)
    {
        UdpClient? udpClient = ar.AsyncState as UdpClient;
        if (udpClient == null)
        {
            return;
        }

        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, MNDP_PORT);
        byte[]? receiveBytes = udpClient.EndReceive(ar, ref endPoint);

        if (receiveBytes != null)
        {
            DeviceInfo? deviceInfo = ParseMNDPResponse(receiveBytes);
            if (deviceInfo != null)
            {
                onDeviceDiscovered(deviceInfo);
            }
        }

        udpClient.BeginReceive(new AsyncCallback((ar) => ReceiveCallback(ar, onDeviceDiscovered)), udpClient);
    }



    private static DeviceInfo? ParseMNDPResponse(byte[] data)
    {
        DeviceInfo deviceInfo = new DeviceInfo();

        int index = 0;
        while (index < data.Length)
        {
            byte type = data[index++];
            byte length = data[index++];

            switch (type)
            {
                case 1:
                    deviceInfo.MacAddress = BitConverter.ToString(data, index, length);
                    break;
                case 5:
                    deviceInfo.Identity = Encoding.ASCII.GetString(data, index, length);
                    break;
                case 7:
                    deviceInfo.Version = Encoding.ASCII.GetString(data, index, length);
                    break;
                case 8:
                    deviceInfo.Platform = Encoding.ASCII.GetString(data, index, length);
                    break;
            }

            index += length;
        }

        return deviceInfo;
    }

}
