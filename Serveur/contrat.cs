using System;

public class Contract
{
        public int playerId;
        public int value;
        public Couleur couleur;


    public void reset()
    {
        playerId = 0;
        value = 0;
        couleur = Couleur.neutre;
    }

    public Contract()
    {
    }

    public int getPlayerId()
    {
        return playerId;
    }

    public int getValue()
    {
        return value;
    }

    public Couleur getCouleur()
    {
        return couleur;
    }

    public void setPlayerId(int _id)
    {
        playerId = _id;
    }

    public void setValue(int _value)
    {
        value = _value;
    }

    public void setCouleur(Couleur _color)
    {
        couleur = _color;
    }

    public void dump()
    {
        Console.WriteLine("J" + playerId + ":"
            + value + " on " + couleur);
    }
}
