namespace Client;

public class Form1 : Form
{
    NetworkClient net = new();
   

    TextBox txtHost   = new() { Text = "127.0.0.1", Width = 150 };
    Button btnConnect = new() { Text = "Connect", AutoSize = true };
    

    ListBox lstLog    = new() { Dock = DockStyle.Fill };
    TextBox txtMsg    = new() { Width = 300 };
    Button btnSend    = new() { Text = "Send", AutoSize = true };
   

    public Form1()
    {
        Text = "Conference Client";
        Width = 1100; Height = 600;

        var row1 = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true };
        row1.Controls.AddRange(new Control[] { txtHost, btnConnect });
       
        var bottom = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true };
        bottom.Controls.AddRange(new Control[] { txtMsg, btnSend });

        
        Controls.Add(lstLog);
       
        Controls.Add(row1);
        Controls.Add(bottom);

        net.TextReceived += line => Log(line);
       

        btnConnect.Click += (s, e) =>
        {
            try { net.Connect(txtHost.Text, 9000); Log("Connected"); }
            catch (Exception ex) { Log("Connect failed: " + ex.Message); }
        };
        
        btnSend.Click += (s, e) => { net.SendChat(txtMsg.Text); txtMsg.Clear(); };
    }

  
    void Log(string s) => BeginInvoke(new Action(() => lstLog.Items.Add(s)));

}