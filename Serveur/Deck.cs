using System;
using System.Collections.Generic;

public class Deck
{
    private List<Carte> pile = new List<Carte>();

    public Deck()
	{
	}

    //initialiser le deck en game master
    public void deck_master()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 8; j++)
            {

                Carte carte = new Carte();
                carte.setCouleur(i);
                carte.setSymb(j);
                pile.Add(carte);
            }
        }
    }

    public int getSize()
    {
        return pile.Count;
    }

    public void reset()
    {
        pile.Clear();
    }

    public Carte getBiggerTrapped()
    {
        Carte better = null;
        foreach (Carte carte in pile)
        {
            if (carte.getTrap() 
                && (better == null || better.getValue() < carte.getValue()))
                better = carte;
        }
        return better;
    }

    public bool isTrapped()
    {
        foreach (Carte carte in pile)
        {
            if (carte.getTrap())
                return true;
        }
        return false;
    }

    public Carte getStrongest(Couleur couleur)
    {
        if (!isColor(couleur))
            return null;
        Carte tmp;
        tmp = pile[0];
        foreach (Carte card in pile)
        {
            if (card.getCouleur() == couleur && (tmp.getCouleur() != couleur || card.getValue() > tmp.getValue()))
                tmp = card;
        }
        return tmp;
    }
    
    //a-t-il dans son deck cette couleur ?
    public bool isColor(Couleur color)
    {
        foreach(Carte card in pile)
        {
            if (card.getCouleur() == color)
                return true;
        }
        return false;
    }

    //a-t-il dans son deck ce type ?
    public bool isType(Type _type)
    {
        foreach (Carte card in pile)
        {
            if (card.getType() == _type)
                return true;
        }
        return false;
    }

    //ajouter une carte
    public void add_carte(Carte _carte)
    {
        pile.Add(_carte);
    }

    //prend une carte et la retourne
    public Carte PutCard(int index)
    {
        Carte card = new Carte();

        card = pile[index];
        pile.Remove(card);
        return card;
    }

    //affiche le deck
    public void aff_deck()
    {
        int i = 0;
        foreach (Carte card in pile)
        {
            Console.Write(i + ": la carte est un " + card.getType() + " de "+ card.getCouleur() + " et vaut " + card.getValue());
            if (card.getTrap() == true)
                Console.Write(" et c'est un atout");
            Console.WriteLine();
            i = i + 1;
        }
        Console.WriteLine();
    }

    //compter la value des cartes dans le deck
    public int total_value()
    {
        int res = 0;
        foreach (Carte card in pile)
        {
            res += card.getValue();
        }
        Console.WriteLine("la totalité des valeurs de vautre deck est de " + res);
        return res;
    }

    public Carte getElem(int i)
    {
        return pile[i];
    }

    //transforme le deck en atout ou pas
    public void trap_deck(Couleur atout)
    {
        foreach(Carte card in pile)
        {
            if (card.getCouleur() == atout)
                card.isTrapCard();
        }
    }

    public List<Carte> getList()
    {
        return pile;
    }
}
