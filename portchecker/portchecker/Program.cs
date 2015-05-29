using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace portchecker
{
    class Program
    {
        static void Main(string[] args)
        {
            List<PortInfo> pi = GetOpenPort();

            Form1 f = new Form1(pi);

            f.ShowDialog();
        }

        private static List<PortInfo> GetOpenPort()
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpEndPoints = properties.GetActiveTcpListeners();
            TcpConnectionInformation[] tcpConnections = properties.GetActiveTcpConnections();

            var k= tcpConnections.Select(p =>
            {
                return new PortInfo(
                    i: p.LocalEndPoint.Port,
                    local: String.Format("{0}:{1}", p.LocalEndPoint.Address, p.LocalEndPoint.Port),
                    remote: String.Format("{0}:{1}", p.RemoteEndPoint.Address, p.RemoteEndPoint.Port),
                    state: p.State.ToString(),
                    _info:p);
            }).ToList();

            var kk = k.Where(a => a.info.LocalEndPoint.Port == 11000).ToList();

            return k;
        }
    }
    public class PortInfo
    {
        public int PortNumber { get; set; }
        public string Local { get; set; }
        public string Remote { get; set; }
        public string State { get; set; }

        internal TcpConnectionInformation info;

        public PortInfo(int i, string local, string remote, string state,TcpConnectionInformation _info)
        {
            PortNumber = i;
            Local = local;
            Remote = remote;
            State = state;
            info = _info;
        }
    }
}
