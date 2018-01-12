using System;
using System.Collections.Generic;
using Players;


public class GameMaster
{
    private List<Player> _players = new List<Player>();
    private int _fp = 0;
    private Contract _contract = null;
    private Couleur _atout = Couleur.neutre;
    private Deck _pile = new Deck();
    private Deck _team1 = new Deck();
    private Deck _team0 = new Deck();

    public GameMaster()
	{
	}

    public void reset()
    {
        _fp = 0;
        _contract = null;
        _atout = Couleur.neutre;
        _pile.reset();
        _team0.reset();
        _team1.reset();
        _players.Clear();
    }

    public int countPlayers()
    {
        return _players.Count;
    }

    public void addPlayer(Player player)
    {
        _players.Add(player);
        Header header = new Header();
        header.id_command = 1;
        info info = new info();
        info.id = _players.Count;
        player.setId(info.id);
        player.write<Header>(header);
        player.write<info>(info);
    }

    public void start()
    {
        startContracts();
        startGame();
    }

    public unsafe played fillContracts(played contract, List<Contract> contracts)
    {
        for (int i = 0; i < contracts.Count; i++)
        {
            contract.value[i] = contracts[i].getValue();
            contract.couleur[i] = Convert.ToInt32(contracts[i].getCouleur());
            contract.ids[i] = contracts[i].getPlayerId();
        }
        return contract;
    }

    public unsafe askcontrat newaskcontrat()
    {
        askcontrat contract = new askcontrat();
        contract.contrats_pile = new played();
        contract.deck = new deck();
        for (int i = 0; i < 8; i++)
        {
            if (i < 4)
            {
                contract.contrats_pile.couleur[i] = -1;
                contract.contrats_pile.ids[i] = -1;
                contract.contrats_pile.value[i] = -1;
            }
            contract.deck.couleur[i] = -1;
            contract.deck.value[i] = -1;
        }
        return contract;
    }

    public unsafe void askContract(int playerId, List<Contract> contracts)
    {
        Header h = new Header();
        h.id_command = 2;
        askcontrat contract = newaskcontrat();
        contract.turn = playerId;
        contract.contrats_pile = fillContracts(contract.contrats_pile, contracts);

        foreach (Player player in _players)
        {
            contract.deck = player.fillDeck(contract.deck);
            player.write<Header>(h);
            player.write<askcontrat>(contract);
        }
    }

    public void DoContracts(List<Contract> contracts)
    {
        for (int i = 0; i < 4; i++)
        {
            Player player = _players[(i + _fp) % 4];
            askContract(player.getId(), contracts);
            player.makeDeal();
            contracts.Add(player.getContra());
        }
    }

    public Contract getBetterContract(List<Contract> contracts)
    {
        Contract better = contracts[0];
        foreach (Contract contract in contracts)
        {
            if (contract.getValue() > better.getValue())
                better = contract;
        }
        return better;
    }

    public void distribute()
    {
        foreach (Player player in _players)
            player.getDeck().reset();
        Deck deck = new Deck();
        deck.deck_master();
        int i = 0;
        Random random = new Random();
        while (deck.getSize() > 0)
        {
            _players[i].recieveCard(deck.PutCard(random.Next(deck.getSize())));
            i = (i + 1) % 4;
        }
    }

    public void startContracts()
    {
        List<Contract> contracts = new List<Contract>();
        while (_contract == null)
        {
            distribute();
            contracts.Clear();
            DoContracts(contracts);
            Contract contract = getBetterContract(contracts);
            if (contract.getValue() != 0)
                _contract = contract;
            _fp = (_fp + 1) % 4;
        }
        _atout = _contract.getCouleur();
        Console.WriteLine("atout is: " +_atout);
        foreach (Player player in _players)
        {
            player.setTrap(_atout);
            player.UpdateCard();
        }
        _fp = _contract.getPlayerId() - 1;
    }

    public unsafe played fillPile(played pile)
    {
        for(int i = 0; i < _pile.getSize(); i++)
        {
            Carte card = _pile.getElem(i);
            pile.value[i] = card.getValue();
            pile.ids[i] = Convert.ToInt32(card.getType());
            pile.couleur[i] = Convert.ToInt32(card.getCouleur());
        }
        return pile;
    }

    public askcontrat askCard(int playerId)
    {
        askcontrat askcard = newaskcontrat();
        Header h = new Header();
        h.id_command = 3;
        askcard.turn = playerId;
        askcard.contrats_pile = fillPile(askcard.contrats_pile);
        foreach (Player player in _players)
        {
            askcard.deck = player.fillDeck(askcard.deck);
            if (player.getId() != playerId)
            {
                askcard.deck = player.fillDeck(askcard.deck);
                player.write<Header>(h);
                player.write<askcontrat>(askcard);
            }
        }
        return askcard;
    }

    public void playRound()
    {
        _pile.reset();
        int i = 0;
        while (_pile.getSize() < 4)
        {
            int cp = (i++ + _fp) % 4;
            askcontrat askcard = askCard(_players[cp].getId());   
            _pile.add_carte(_players[cp].PlayCard(_pile, askcard));
        }
    }

    public void givePoints()
    {
        Deck win = _team1;
        if ((_fp % 2) == 0)
            win = _team0;
        for (int i = 0; i < _pile.getSize(); i++)
            win.add_carte(_pile.PutCard(i));
    }

    public void announceRoundWinner(Carte better)
    {
        Header h = new Header();

        h.id_command = 5;
        roundVictory info = new roundVictory();
        info.type = Convert.ToInt32(better.getType());
        info.color = Convert.ToInt32(better.getCouleur());
        info.playerId = _fp + 1;
        foreach (Player player in _players)
        {
            player.write<Header>(h);
            player.write<roundVictory>(info);
        }
    }

    public void determineRoundWinner()
    {
        Carte better = _pile.getElem(0);
        int win = _fp;
        for (int i = 1; i < 4; i++)
        {
            Carte carte = _pile.getElem(i);
            if (carte.isBetter(better))
            {
                better = carte;
                win = (_fp + i) % 4;
            }
        }
        _fp = win;
        givePoints();
        announceRoundWinner(better);
    }

    public void announceVictory(int win)
    {
        Header h = new Header();
        h.id_command = 4;
        victory vic = new victory();
        vic.team = win;
        if (win % 2 == 0)
            vic.score = _team0.total_value();
        else
            vic.score = _team1.total_value();
        foreach (Player player in _players)
        {
            player.write<Header>(h);
            player.write<victory>(vic);
        }
    }

    public void determineGameWinner()
    {
        int team = (_contract.getPlayerId() - 1) % 4;
        int win = 0;
        Deck deck = _team1;
        if (team == 0)
            deck = _team0;
        if (deck.total_value() >= _contract.getValue())
            win = team;
        else
            win = team + 1 % 2;
        announceVictory(win);
    }

    public void startGame()
    {
        for (int i = 0; i < 8; i++)
        {
            playRound();
            _pile.aff_deck();
            determineRoundWinner();
        }
        determineGameWinner();
    }
}
