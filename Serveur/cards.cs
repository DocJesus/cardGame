using System;


public class Carte
{
    private Couleur couleur;
    private Type type;
    private int value;
    private bool isTrap;

    public Carte()
    {
        isTrap = false;
    }
    ~Carte()
    { }

    public void setCouleur(int i)
    {
        switch (i)
        {
            case 0:
                couleur = Couleur.Carreau;
                break;
            case 1:
                couleur = Couleur.Trèfle;
                break;
            case 2:
                couleur = Couleur.Pick;
                break;
            case 3:
                couleur = Couleur.Coeur;
                break;
            default:
                break;
        }
    }

    public void setSymb(int i)
    {
        switch (i)
        {
            case 0:
                value = (int)Basic.As;
                type = Type.As;
                break;
            case 1:
                value = (int)Basic.Roi;
                type = Type.Roi;
                break;
            case 2:
                value = (int)Basic.Dame;
                type = Type.Dame;
                break;
            case 3:
                value = (int)Basic.Valet;
                type = Type.Valet;
                break;
            case 4:
                value = (int)Basic.Dix;
                type = Type.Dix;
                break;
            case 5:
                value = (int)Basic.Neuf;
                type = Type.Neuf;
                break;
            case 6:
                value = (int)Basic.Huit;
                type = Type.Huit;
                break;
            case 7:
                value = (int)Basic.sept;
                type = Type.Sept;
                break;
            default:
                break;
        }
    }

    public void isTrapCard()
    {
        this.isTrap = true;
        switch (value)
        {
            case (int)Basic.As:
                value = (int)Atout.As;
                break;
            case (int)Basic.Roi:
                value = (int)Atout.Roi;
                break;
            case (int)Basic.Dame:
                value = (int)Atout.Dame;
                break;
            case (int)Basic.Valet:
                value = (int)Atout.Valet;
                break;
            case (int)Basic.Dix:
                value = (int)Atout.Dix;
                break;
            case (int)Basic.Neuf:
                value = (int)Atout.Neuf;
                break;
            //case (int)Basic.Huit:
            //value = (int)Atout.Huit;
            //break;
            //case (int)Basic.sept:
            //value = (int)Atout.Sept;
            //break;
            default:
                break;
        }
    }

    public bool getTrap()
    {
        return isTrap;
    }

    public Type getType()
    {
        return type;
    }

    public int getValue()
    {
        return value;
    }

    public Couleur getCouleur()
    {
        return couleur;
    }

    public bool isBetter(Carte better)
    {
        if (isTrap)
        {
            if (!better.getTrap())
                return true;
            return value > better.getValue();
        }
        else
        {
            if (better.getCouleur() != couleur)
                return false;
            return value > better.getValue();
        }
    }
};
