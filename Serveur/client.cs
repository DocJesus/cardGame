using Players;
using System;
using System.Text;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;

namespace Players
{
    public class Player
    {
        int score;
        int id;
        Couleur trap;
        ConnectWork wifi = new ConnectWork();
        Deck deck = new Deck();
        Contract contra = new Contract();

        public Player()
        {
        }

        public unsafe deck fillDeck(deck tofill)
        {
            for (int i = 0; i < deck.getSize(); i++)
            {
                Carte carte = deck.getElem(i);
                tofill.couleur[i] = Convert.ToChar(carte.getCouleur());
                tofill.value[i] = Convert.ToInt32(carte.getType());
            }
            return tofill;
        }

        public bool isBiggerAtout(Carte carte, Deck pile)
        {
            Carte bigger = pile.getBiggerTrapped();
            if (bigger == null || bigger.getValue() < carte.getValue())
                return true;
            return (deck.getBiggerTrapped().getValue() < bigger.getValue());
        }

        public bool isAtout(Carte carte, Deck pile)
        {
            if (carte.getTrap())
                return isBiggerAtout(carte, pile);
            return !deck.isTrapped();
        }

        public bool doesHeHaveGoodColor(Carte carte, Deck pile)
        {
            if (pile.getElem(0).getTrap()
                || !deck.isColor(pile.getElem(0).getCouleur()))
                return (isAtout(carte, pile));
            return false;
        }

        public bool isGoodColor(Carte carte, Deck pile)
        {
            if (pile.getElem(0).getTrap()
                || pile.getElem(0).getCouleur() != carte.getCouleur())
                return doesHeHaveGoodColor(carte, pile);
            return true;
        }

        public bool isGoodMove(Carte carte, Deck pile)
        {
            if (pile.getSize() == 0)
                return true;
            return isGoodColor(carte, pile);
        }

        public unsafe Carte PlayCard(Deck pile, askcontrat askcard)
        {
            askcard.deck = fillDeck(askcard.deck);
            Header h = new Header();
            h.id_command = 3;
            bool ok = false;
            responce responce = new responce();
            while (!ok)
            {
                write<Header>(h);
                write<askcontrat>(askcard);
                responce = read<responce>();
                Carte carte = deck.getElem(responce.value);
                ok = isGoodMove(carte, pile);
            }
            return deck.PutCard(responce.value);
        }
        
        //recois une carte
        public void recieveCard(Carte card)
        {
            deck.add_carte(card);
        }
   
        public T read<T>() where T : new ()
        {
            T tmp = new T();
            byte[] bytes = new byte[Marshal.SizeOf(tmp)];
            int bytesRead = wifi.getNetworkStream().Read(bytes, 0, bytes.Length);
            tmp = ConnectWork.fromBytes<T>(bytes);
            return tmp;
        }

        //écrit un message et l'envoie
        public void write<T>(T tmp) where T : new ()
        {
            byte[] message = wifi.getBytes<T>(tmp);
            wifi.getNetworkStream().Write(message, 0, message.Length);
        }

        public void makeDeal()
        {
            responce contract = read<responce>();
            contra.setCouleur((Couleur)contract.couleur);
            contra.setValue(contract.value);
            contra.setPlayerId(id);
          
        }

        //fonction pour rendre le deck en atout ou pas
        public void UpdateCard()
        {
            deck.trap_deck(trap); 
        }

        public Deck getDeck()
        {
            return deck;
        }

        public void setScore(int _nb)
        {
            score = _nb;
        }

        public int getScore()
        {
            return score;
        }

        public void setTrap(Couleur _trap)
        {
            trap = _trap;
        }

        public void setId(int _id)
        {
            id = _id;
        }

        public void dump()
        {
            Console.WriteLine("Player " + id + " deck : ");
            deck.aff_deck();
        }

        public int getId()
        {
            return id;
        }

        public Contract getContra()
        {
            return contra;
        }

        public ConnectWork getWifi()
        {
            return wifi;
        }
    }
}
