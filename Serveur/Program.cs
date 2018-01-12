using Players;
using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;

public unsafe class serveur
{
    private List<Player> players = new List<Player>();
    private GameMaster game = new GameMaster();
    private bool done = false;
    private ConnectWork link = new ConnectWork();

    private List<TcpClient> client_room = new List<TcpClient>();
    private List<NetworkStream> stream_room = new List<NetworkStream>();

    public serveur()
    {
    }

    public void checkDisconnection()
    {
        List<int> discIds = new List<int>();
        foreach (Player player in players)
        {
            if (!player.getWifi().getTcpClient().Connected)
            {
                Console.WriteLine("player " + player.getId() + " s'est deconnecte");
                discIds.Add(player.getId());
            }
        }
        foreach (int id in discIds)
        {
            players.RemoveAt(id - 1);
        }
    }

    public int launchServer()
    {
        link.setUpServeur("127.0.0.1", 13);
        while (!done)
        {
            while (players.Count < 4)
            {
                Console.Write("Waiting for connection...");
                TcpClient tmp = new TcpClient();
                tmp = link.getTcpListener().AcceptTcpClient();
                Console.WriteLine(players.Count + "em player connected");
                NetworkStream _ns;
                _ns = tmp.GetStream();
                Player player = new Player();
                player.getWifi().setTcpClient(tmp);
                player.getWifi().setNetworkStream(_ns);
                players.Add(player);
            }
            try
            {
                game.reset();
                foreach (Player player in players)
                    game.addPlayer(player);
                game.start();
            }
            catch (Exception e)
            {
                checkDisconnection();
            }
        }

        link.closeListener();
        return 0;
    }
}

public class Class1
{
        static void Main(string[] args)
        {
        try
        {
        serveur slave = new serveur();
        Console.WriteLine("Server up");
        slave.launchServer();
        Console.ReadKey();
        }
        catch (Exception e)
        {
            Console.WriteLine("An error as occured");
            Console.WriteLine("Press a key to quit");
            Console.ReadKey();
        }

        }
}
