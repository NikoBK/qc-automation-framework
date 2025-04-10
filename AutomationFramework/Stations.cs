using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace AutomationFramework
{
    /// <summary>
    /// Handles instances of <cref="Vial"> (digital representation) redirected to it by the <cref="MES">.
    /// </summary>
    public class LiquidHandler : Station
    {
        //TODO: this contains all functions relevant for the liquid handler to work.
    }

    public class FestoActuator : Station
    {
        //TODO
    }

    public class Capper : Station
    {
        public void Cap()
        {
            TcpClient cli = new TcpClient(Constants.CapHandlerAddr, Constants.CapHandlerPort);
            NetworkStream str = cli.GetStream();
            string msg = "cap";
            byte[] dat = Encoding.UTF8.GetBytes(msg);
            str.Write(dat, 0, dat.Length);
            cli.Close();
            Thread.Sleep(2000);
        }
    }

    public class Decapper : Station
    {
        //TODO
    }

    public class VialMixer : Station
    {
        //TODO
    }
}
