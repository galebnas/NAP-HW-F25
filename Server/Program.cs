using System.Net;
using System.Net.Sockets;
using System.Text;

const int TcpPort = 9000;

int nextId = 0;

var listener = new TcpListener(IPAddress.Any, TcpPort);
listener.Start();
Console.WriteLine($"TCP listening on {TcpPort}");

while (true)
{
    TcpClient tcp = listener.AcceptTcpClient();           // waits for the next client
    new Thread(() => HandleClient(tcp)) { IsBackground = true }.Start();
}

void HandleClient(TcpClient tcp)
{
    var stream = tcp.GetStream();
    var reader = new StreamReader(stream, Encoding.UTF8);
    var me = new ClientInfo
    {
        Id = Interlocked.Increment(ref nextId),            
        Writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true }
    };
    Console.WriteLine($"Client {me.Id} connected");
    me.Send("ID " + me.Id);

    try
    {
        string? line;
        while ((line = reader.ReadLine()) != null)         // null = client closed the connection
        {
            Console.WriteLine($"[{me.Id}] {line}");
            me.Send("INFO Server got: " + line);           
        }
    }
    catch (IOException) { }                                 // client crashed / network error

    
    Console.WriteLine($"Client {me.Id} disconnected");
    tcp.Close();
}


class ClientInfo
{
    public int Id;
    public string Name = "";
  
    public StreamWriter Writer = null!;

    public void Send(string line)
    {
        try { lock (Writer) Writer.WriteLine(line); }
        catch { }                                           // client is gone; its own thread cleans up
    }
}