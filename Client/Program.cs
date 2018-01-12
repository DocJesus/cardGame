using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Runtime.InteropServices;

public unsafe struct roundVictory
{
    public int type;
    public int color;
    public int playerId;
}

public unsafe struct responce
{
    public int couleur;
    public int value;
}

public unsafe struct info //1 //4 -> info porc
{
    public int id;
}

public unsafe struct deck
{
    public fixed int value[8];
    public fixed int couleur[8];
}

public unsafe struct played
{
    public fixed int value[4];
    public fixed int ids[4];
    public fixed int couleur[4];
}

public unsafe struct askcontrat //2 //3
{
    public deck deck;
    public played contrats_pile;
    public int turn;
}
//return contrat

//int index carte a jouer

public struct victory //5
{
    public int team;
    public int score;
}

public unsafe struct Header
{
    public int id_command;
}

namespace client
{
    public unsafe class TcpTimeClient
    {
        public delegate int FunctionPointer(ConnectWork _elem);
        private List<FunctionPointer> funcs = new List<FunctionPointer>();
        private ConnectWork link = new ConnectWork();
        int _ID;

        public TcpTimeClient()
        {
            //funcs.Add(AskContract);
            funcs.Add(readHeader);
            funcs.Add(RepConnection);
            funcs.Add(AskContract);
            funcs.Add(AskCard);
            funcs.Add(print_victory);
            funcs.Add(round_victory);
        }

        public T getStruct<T>(int size) where T : new()
        {
            byte[] bytes = new byte[size];
            int bytesRead = link.getNetworkStream().Read(bytes, 0, bytes.Length);
            return ConnectWork.fromBytes<T>(bytes);
        }

        public int readHeader(ConnectWork link) //0
        {
            Header str = new Header();

            str = getStruct<Header>(Marshal.SizeOf(str));
         //   Console.WriteLine("id_command = " + str.id_command);

            return str.id_command;
        }

        public int RepConnection(ConnectWork link) //1
        {
            info str = new info();

            str = getStruct<info>(Marshal.SizeOf(str));
            _ID = str.id;
            Console.WriteLine("Vous etes le joueur " + str.id);
            return 0;
        }
 
        //public unsafe struct askcontrat //2 //3
        public int AskContract(ConnectWork link)
        {
            bool turn = true;
            int stronger = 0;
            responce res = new responce();
           // Console.WriteLine("AskContract");
            played tmp;
            deck deck;
            askcontrat str = new askcontrat();
            string saisie;
            int numval = 0;

            str = getStruct<askcontrat>(Marshal.SizeOf(str));
            //Console.WriteLine("turn = " + str.turn);
            tmp = str.contrats_pile;
            deck = str.deck;
            res.couleur = -1;
            res.value = -1;
            for (int i = 0; i < 4; i++)
            {
                if (tmp.ids[i] != -1)
                {
                    if (tmp.value[i] > stronger)
                        stronger = tmp.value[i];
                    Console.WriteLine("le joueur " + tmp.ids[i] + " a fait un contrat de " + tmp.value[i] + " de la couleur " + (Couleur)tmp.couleur[i]);
                }
            }
            if (str.turn == _ID)
            {
                Console.WriteLine("A vous de proposer un contrat");
                Console.WriteLine("Votre deck est composé de :");
                print_deck(deck);
                while (turn == true)
                {
                    Console.WriteLine("veuillez saisir le montant du contrat (il doit valoir 0 ou être suppérieur au plus haut des contrats, ce dernier vaut : " + stronger);
                    saisie = Console.ReadLine();
                    if (saisie != "")
                    {
                        numval = Convert.ToInt32(saisie);
                        if (numval == 0 || numval > stronger)
                            turn = false;
                    }
                }
                res.value = numval;
                turn = true;
                while (turn == true)
                {
                    Console.WriteLine("veuillez la couleur du contrat: 0 -> carreau, 1 -> trèfle, 2 -> Pick, 3 -> Coeur");
                    saisie = Console.ReadLine();
                    if (saisie != "")
                    {
                        numval = Convert.ToInt32(saisie);
                        if (numval <= 3 && numval >= 0)
                            turn = false;
                    }
                }
                res.couleur = numval;
            }
            if (res.couleur != -1 && res.value != -1)
            {
                byte[] message = link.getBytes<responce>(res);
                link.getNetworkStream().Write(message, 0, message.Length);
            }
            return 0;
        }

        public int AskCard(ConnectWork link)
        {
            askcontrat str = new askcontrat();
            deck deck;
            played pile;
            responce res = new responce();
            string saisie;
            bool turn = true;
            int i = 0;
            int numval = 0;

            str = getStruct<askcontrat>(Marshal.SizeOf(str));
            deck = str.deck;
            pile = str.contrats_pile;
            if (str.turn == _ID)
            {
                Console.WriteLine("les cartes déjà jouer sont :");
                for (int j = 0; j < 4; j++)
                {
                    if (pile.value[j] != -1)
                        Console.WriteLine("la " + j + " em carte est un(e) " + (Type)pile.ids[j] + " de " + (Couleur)pile.couleur[j] + " qui vaut " + pile.value[j]);
                }
                while (turn == true)
                {
                    Console.WriteLine("Votre deck est composé de :");
                    i = print_deck(deck);
                    Console.WriteLine("veulliez saisir l'index de la carte que vous voulez jouer");
                    saisie = Console.ReadLine();
                    if (saisie != "")
                    {
                        numval = Convert.ToInt32(saisie);
                        if (numval < i && numval >= 0)
                            turn = false;
                    }
                }
                res.value = numval;
                res.couleur = -1;
                byte[] message = link.getBytes<responce>(res);
                link.getNetworkStream().Write(message, 0, message.Length);
            }
            return 0;
        }

        public int print_deck(deck deck)
        {
            int i = 0;
            for (int k = 0; k < 8; k++)
            {
                if (deck.value[k] != -1)
                {
                    Console.WriteLine(k + "em carte est un(e) " + (Type)deck.value[k] + " de " + (Couleur)deck.couleur[k]);
                    i = i + 1;
                }
            }
            return i;
        }

        public int print_victory(ConnectWork link)
        {
            victory str = new victory();

            str = getStruct<victory>(Marshal.SizeOf(str));
            Console.WriteLine("l'équipe gagnante est l'équipe n° " + str.team);
            Console.WriteLine("elle gagne avec " + str.score + " points");
            return 0;
        }

        public int round_victory(ConnectWork link)
        {
            roundVictory vic = new roundVictory();

            vic = getStruct<roundVictory>(Marshal.SizeOf(vic));
            Console.WriteLine("le gagnant de cette mache est le joueur " + vic.playerId + " avec un(e) " + (Type)vic.type + " de " + (Couleur)vic.color);
            return 0;
        }

        public void connect()
        {
            //
            int i = 0;
            string saisie = "";

            while (saisie == "")
            {
                Console.WriteLine("veuillez rentrer l'adresse IP du serveur");
                saisie = Console.ReadLine();
            }
            link.setUpClient(saisie, 13);
            Console.WriteLine("You're connected to the server");
            while (true)
            {
                var func = funcs[i];
                i = func(link);
            }
            //link.closeClient();
        }
    }

    public class client
    {
        public static int Main(String[] args)
        {
            TcpTimeClient client = new TcpTimeClient();
            try
            {
                client.connect();    
            }
            catch (Exception e)
            {
                Console.WriteLine("Server Error: press a key to quit");
                Console.ReadKey();
            }
            return 0;
        }
    }
}

