using System.Net.Sockets;
using System.Text;
using ilikefrogs101.Logging;

namespace ilikefrogs101.MusicPlayer;
public static class IcpHandler {
    private const string SOCKET_PATH = "/tmp/musicplayer.sock";
    private const int PENDING_CONNECTIONS_MAX = 5;
    private const int BUFFER_SIZE = 1024;

    public static Action<string> CommandReceived;

    private static List<Socket> _clients = new();

    public static void ListenForCommands() {
        if(File.Exists(SOCKET_PATH)) {
            File.Delete(SOCKET_PATH);
        }

        UnixDomainSocketEndPoint endPoint = new UnixDomainSocketEndPoint(SOCKET_PATH);

        using Socket server = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
        server.Bind(endPoint);
        server.Listen(PENDING_CONNECTIONS_MAX);

        while(true) {
            using Socket client = server.Accept();
            _clients.Add(client);

            byte[] buffer = new byte[BUFFER_SIZE];
            int bytesRead = client.Receive(buffer);

            string command = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Log.DebugMessage($"Received Command \"{command}\"");
            CommandReceived?.Invoke(command);
        }
    }
    public static void BroadcastResponse(string response) {
        Log.DebugMessage($"Broadcasting: {response}");
        for(int i = 0; i < _clients.Count; ++i) {
            Socket client = _clients[i];

            if(!client.Connected) {
                continue;
            }

            byte[] bytes = Encoding.UTF32.GetBytes(response);
            client.Send(bytes);
        }
    }
}