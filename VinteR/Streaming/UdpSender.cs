using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Google.Protobuf;
using VinteR.Configuration;
using VinteR.Model;
using VinteR.Serialization;

namespace VinteR.Streaming
{
    public class UdpSender : IStreamingServer
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public IList<UdpReceiver> UdpReceivers { get; set; }

        public int Port { get; }

        private ConcurrentQueue<IPEndPoint> _endPoints;

        private UdpClient _udpServer;

        private readonly ISerializer _serializer;

        public UdpSender(IConfigurationService configurationService, ISerializer serializer)
        {
            UdpReceivers = configurationService.GetConfiguration().UdpReceivers;
            Port = configurationService.GetConfiguration().UdpServerPort;
            _serializer = serializer;
        }

        public void Send(MocapFrame mocapFrame)
        {
            _serializer.ToProtoBuf(mocapFrame, out var frame);
            var data = frame.ToByteArray();
            foreach (var ipEndPoint in _endPoints)
            {
                Task.Factory.StartNew(() =>
                {
                    Logger.Debug("Sending to {0}:{1}", ipEndPoint.Address, ipEndPoint.Port);
                    try
                    {
                        _udpServer.Send(data, data.Length, ipEndPoint);
                    }
                    catch (Exception)
                    {
                        Logger.Warn("Could not send frame {0,8} to {1}", mocapFrame.ElapsedMillis, ipEndPoint.Address);
                    }
                });
            }
        }

        public void AddReceiver(IPEndPoint receiverEndPoint)
        {
            if (!_endPoints.Contains(receiverEndPoint))
                _endPoints.Enqueue(receiverEndPoint);
        }

        public void Start()
        {
            _udpServer = new UdpClient(Port);
            Logger.Info("Udp server running on port {0}", Port);
            _endPoints = new ConcurrentQueue<IPEndPoint>();
            foreach (var udpReceiver in UdpReceivers)
            {
                var ip = udpReceiver.Ip;
                var port = udpReceiver.Port;
                try
                {
                    _endPoints.Enqueue(new IPEndPoint(IPAddress.Parse(ip), port));
                }
                catch (Exception e)
                {
                    Logger.Warn("Could not add endpoint {0}:{1}, cause = {2}", ip, port, e.Message);
                }
            }
        }

        public void Stop()
        {
            _udpServer?.Close();
        }
    }
}