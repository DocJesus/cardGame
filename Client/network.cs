using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;

public class ConnectWork
{
    private TcpListener listener;
    private TcpClient client;
    private NetworkStream ns;
    private IPAddress Ip;
    private IAsyncResult result;

    public ConnectWork()
    {
	}

    public void setUpClient(string hostName, int portnum)
    {
        client = new TcpClient();
        result = client.BeginConnect(hostName, portnum, null, null);
        var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1), false);
        if (!success)
        {
            throw new Exception("failed to connect");
        }
        ns = client.GetStream();
        Console.WriteLine("vous êtes connecté");
    }

    public void setUpServeur(string hostname, int portnum)
    {
        Ip = IPAddress.Parse(hostname);
        listener = new TcpListener(Ip, portnum);
        listener.Start();
    }

    public static T fromBytes<T>(byte[] arr) where T : new()
    {
        T str = new T();

        int size = Marshal.SizeOf(str);
        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(arr, 0, ptr, size);
        str = (T)Marshal.PtrToStructure(ptr, str.GetType());
        Marshal.FreeHGlobal(ptr);

        return str;
    }

    public byte[] getBytes<T>(T lol)
    {
        int size = Marshal.SizeOf(lol);
        byte[] arr = new byte[size];

        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(lol, ptr, false);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);
        return arr;
    }

    public void setTcpListener(TcpListener tmp)
    {
        listener = tmp;
    }

    public TcpListener getTcpListener()
    {
        return listener;
    }

    public void setTcpClient(TcpClient tmp)
    {
        client = tmp;
    }

    public TcpClient getTcpClient()
    {
        return client;
    }

    public void setNetworkStream(NetworkStream tmp)
    {
        ns = tmp;
    }

    public NetworkStream getNetworkStream()
    {
        return ns;
    }

    public IPAddress GetIPAdress()
    {
        return Ip;
    }

    public void closeClient()
    {
        client.Close();
    }

    public void closeListener()
    {
        listener.Stop();
    }
}
