using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client;

public class NetworkClient
{
    TcpClient tcp = null!;
    StreamReader reader = null!;
    StreamWriter writer = null!;
   

    public int ClientId { get; private set; }               // given by the server ("ID 3")

    public event Action<string>? TextReceived;


    public void Connect(string host, int tcpPort)
    {
        tcp = new TcpClient(host, tcpPort);
        var stream = tcp.GetStream();
        reader = new StreamReader(stream, Encoding.UTF8);
        writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

        new Thread(TcpLoop) { IsBackground = true }.Start();

    }

    public void SendChat(string text) => SendText("MSG " + text);

    void SendText(string line) => writer.WriteLine(line);

  

    void TcpLoop()
    {
        try
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("ID "))
                {
                    ClientId = int.Parse(line.Substring(3));
                   
                }
                else TextReceived?.Invoke(line);
            }
        }
        catch { }                                           // connection closed
    }

}