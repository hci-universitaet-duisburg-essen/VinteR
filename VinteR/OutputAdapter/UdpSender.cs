using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using VinteR.Configuration;
using VinteR.Model;

namespace VinteR.OutputAdapter
{
    public class UdpSender : IOutputAdapter
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public IList<UdpReceiver> UdpReceivers { get; set; }

        public int Port { get; }

        private IList<IPEndPoint> _endPoints;

        private UdpClient _udpServer;

        public UdpSender(IConfigurationService configurationService)
        {
            UdpReceivers = configurationService.GetConfiguration().UdpReceivers;
            Port = configurationService.GetConfiguration().Port;
        }

        public void OnDataReceived(MocapFrame mocapFrame)
        {
            var data = mocapFrame.ToBytes();
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

        public void SetHomeDir(string homeDir)
        {
        }

        public void Start()
        {
            _udpServer = new UdpClient(Port);
            Logger.Info("Udp server running on port {0}", Port);
            _endPoints = new List<IPEndPoint>();
            foreach (var udpReceiver in UdpReceivers)
            {
                var ip = udpReceiver.Ip;
                var port = udpReceiver.Port;
                try
                {
                    _endPoints.Add(new IPEndPoint(IPAddress.Parse(ip), port));
                }
                catch (Exception e)
                {
                    Logger.Warn("Could not add endpoint {0}:{1}, cause = {2}", ip, port, e.Message);
                }
            }
        }
    }
}