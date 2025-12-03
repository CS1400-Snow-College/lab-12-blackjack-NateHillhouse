/*You should keep a file record of players that has their name, and their current amount they have to bet at the blackjack table
You need to simulate a deck of cards in a random order. 
Your deck should have 52 unique card (13 faces x 4 suits).  
When drawing cards from this deck they should be removed from the deck. 
When a player stops playing, their new bank total should be stored back to file. 
There are many additional rules to blackjack. 
You must implement the following menu options for your game play:  (H)it, (S)tand, (D)ouble.  
The game should adhere to all the rules of blackjack. Please refer to the discussion if you are not familiar with how to play the game.
The example game play shown is only how I implemented the game. 
You may take license to create the game in a way pleasing to you.  Just remember it must work, no inventing your own rules. */

Main();

void Main()
{
    Console.Clear();
    Random rand = new Random();
    string file = "blackjack.txt";
    List<PlayerInfo> players = ReadFile(file);
    PlayerInfo player = Intro(players);
        
        //Test Info
        //PlayerInfo player = new PlayerInfo() {name = "Nate", money = 50};
        //int bet = 5;
    
    Console.WriteLine($"Welcome {player.name}, you have ${player.money} in the bank.");
    int bet = makeBet(player);
    Hands hands = new Hands(){PlayerHand = new List<(int value, string suite, string number)>(), HouseHand = new List<(int value, string suite, string number)>()};
    List<(int value, string suite, string number)> deck = CreateDeck(rand);
    
    //Deal Starting Deck
    hands = DealCards(deck, rand, hands, 2);
    bet = GameLoop(hands, player, rand, deck, bet);

    string outcome;
    if ((hands.PlayerValue > hands.HouseValue && hands.PlayerValue <=21 && hands.HouseValue <= 21) || (hands.PlayerValue <=21 && hands.HouseValue >21)) 
    {
        outcome = $"You won ${bet}";
        
        Console.WriteLine(outcome);
        player.money += bet;
    }
    else if (hands.HouseValue >21 && hands.PlayerValue > 21) outcome = "Tie";
    else {
        outcome = $"You lost ${bet}";
        Console.WriteLine(outcome);
        player.money -= bet;
    }
    
        
    //WriteFile(players, player, file);
}


int GameLoop(Hands hands, PlayerInfo player, Random rand, List<(int, string, string)> deck, int bet, bool continuePlayer = true, bool continueHouse = true)
{
    int turn = 1;
    while (continuePlayer || continueHouse)
    {
        //WriteEach Card in players hand
        //Console.WriteLine($"{continuePlayer}, {continueHouse}"); //For debugging

        ViewPlayerCards(hands, player);
        turn = ViewHouseCards(hands, player, turn);

        Console.WriteLine();
        Console.WriteLine();

        if (continuePlayer && hands.PlayerValue < 21) {
            string action = GetPlayerAction(turn);
            Console.WriteLine(hands.PlayerValue);
            Console.WriteLine();
            continuePlayer = Action(action, hands, rand, deck, ref bet);
        }
        if (hands.PlayerValue >= 21 /*&& /*!hasAce*/) continuePlayer = false; //change value of ace to 1 if over 21
        if (continueHouse) continueHouse = HouseAction(hands, deck, rand, turn);
        if (hands.HouseValue >= 17) continueHouse = false;
        
    }
    
    return bet;

    bool HouseAction(Hands hands, List<(int, string, string)> deck, Random rand, int turn)
    {
        if (hands.HouseValue <= 16 && turn >2)
        {
            hands.HouseHand.Add(GetCard(deck, rand, hands));
            return true;
        } 
        else if (hands.HouseValue > 16) return false;
        else return true;
        
    }

    bool Action(string action, Hands hands, Random rand, List<(int value, string suite, string number)> deck, ref int bet)
    {
        switch (action)
        {
            case "Hit":
            (int value, string suite, string number) card = GetCard(deck, rand, hands);
            hands.PlayerHand.Add(card);
            Console.WriteLine($"Your draw: [{card.suite}{card.number}]");
            break;

            case "Stand":
            return false;
            
            case "Double":
            hands.PlayerHand.Add(GetCard(deck, rand, hands));
            bet *= 2;
            return false;
        }
        return true;
    }

    string GetPlayerAction(int turn)
    {
        Dictionary<string, List<string>> actions = new Dictionary<string, List<string>>() 
        {
            {"Hit", ["Hit", "hit", "H", "h"]},
            {"Stand", ["Stand", "stand", "S", "s"]},
            {"Double", ["Double", "double", "D", "d"]}
        }; 
        string? key = "";
        
        while (!actions["Hit"].Contains(key) && !actions["Stand"].Contains(key) && !actions["Double"].Contains(key))
        {
            Console.Write("(H)it, (S)tand");
            if (turn == 2) Console.Write(", (D)ouble? ");
            else Console.Write("? ");
            key = Console.ReadLine();  
        } 
        if (actions["Hit"].Contains(key)) return "Hit";
        else if (actions["Stand"].Contains(key)) return "Stand";
        else if (turn == 2) 
        {
            
            return "Double";
        }
        else return GetPlayerAction(turn);

    }
}
int ViewHouseCards(Hands hands, PlayerInfo player, int turn)
{
    Console.WriteLine($"Dealer Shows: ");
    hands.HouseValue = 0;
    Console.SetCursorPosition(15, Console.GetCursorPosition().Top);
    if (turn >= 2)
    {
        for (int item = 0; item < hands.HouseHand.Count(); item++) 
        {
            hands.HouseValue += hands.HouseHand[item].value;
            Console.Write($"[{hands.HouseHand[item].suite}{hands.HouseHand[item].number}] ");
        }
        Console.Write($"Total: {hands.HouseValue}");
    }
    else if (turn == 1)
    {
        Console.Write($"[{hands.HouseHand[0].suite}{hands.HouseHand[0].number}] [?] ");
    }

    return turn+1;
}

void ViewPlayerCards(Hands hands, PlayerInfo player)
{
    Console.WriteLine($"{player.name}'s hand: ");
    hands.PlayerValue = 0;
    Console.SetCursorPosition(15, Console.GetCursorPosition().Top);
    foreach ((int value, string suite, string number) item in hands.PlayerHand) 
    {
        hands.PlayerValue += item.value;
    }
    for (int i = 0; i < hands.PlayerHand.Count; i++){ if (hands.PlayerHand[i].number == "A" && hands.PlayerValue > 21) hands.PlayerHand[i] = (1, hands.PlayerHand[i].suite, hands.PlayerHand[i].number);
    }
    hands.PlayerValue = 0;
    foreach ((int value, string suite, string number) item in hands.PlayerHand) 
    {
        hands.PlayerValue += item.value;
        Console.Write($"[{item.suite}{item.number}] ");
    }
    Console.WriteLine($"Total: {hands.PlayerValue}");

}

Hands DealCards(List<(int value, string suite, string number)> deck, Random rand, Hands hands, int cards)
{
    for (int i = cards; i>0; i--) hands.PlayerHand.Add(GetCard(deck, rand, hands));
    for (int i = cards; i>0; i--) hands.HouseHand.Add(GetCard(deck, rand, hands));
    return hands;
}

(int value, string suite, string number) GetCard(List<(int value, string suite, string number)> deck, Random rand, Hands hands)
{
    (int value, string suite, string number) card = deck[rand.Next(0, deck.Count())];
    deck.Remove(card);
    return card;
}

int makeBet(PlayerInfo player)
{
    int bet = InputToInt("How much would you like to bet? ");
    while (bet > player.money)
    {
        Console.Clear();
        bet = InputToInt($"You do not have enough money to bet this much. Please input a number under ${player.money} ");
    }
    return bet;
}

static PlayerInfo Intro(List<PlayerInfo> players)
{
    string? playername = null;

    while (playername == null) {
        Console.Clear();   
        Console.Write("What is your name? ");
        playername = Console.ReadLine();
    }

    foreach (PlayerInfo item in players)
    {
        if (item.name == playername) return item;
    }
    return new PlayerInfo ()
    {
        name = playername,
        money = 50
    }; 

}

static int InputToInt(string message)
{
    
    bool success = false;
    int input = 0;
    while (!success) 
    {
        Console.Write(message);
        success = Int32.TryParse(Console.ReadLine(), out input);
    }
    return input;
}

static List<PlayerInfo> ReadFile(string file)
{
    List<PlayerInfo> info = new List<PlayerInfo>();

    string[] fileinfo = File.ReadAllLines(file);
    foreach (string line in fileinfo)
    {
        string[] part = line.Split(',');

        info.Add(new PlayerInfo()
        {
            name = part[0],
            money = Int32.Parse(part[1]),
        }
        );
    }
    return info;
}

static void WriteFile(List<PlayerInfo> players, PlayerInfo player, string path)
{
    bool added = false;
    for (int person = 0; person < players.Count; person++)
    {
        if (players[person].name == player.name) 
        {
            players[person] = player;
            added = true;
        }
    }
    if (!added) players.Add(player);

    List<string> info = new List<string>();
    foreach (PlayerInfo line in players)
    {
        info.Add(line.name + ',' + line.money.ToString());
    }
    
    File.WriteAllLines(path, info);
}

static List<(int value, string suite, string number)> CreateDeck(Random rand)
{
    List<(int value, string suite, string number)> cards = new List<(int value, string suite, string number)>();
    List<string> suites = new List<string> {"♦","♥","♠","♣"};
    for (int num = 1; num<=13; num++)
    {
        foreach (string item in suites)
        {
            string card = "";
            int value = num;
            switch (num)
            {
                case 1:
                    card = "A";
                    num = 1;
                    value = 11;
                    break;
                case 11:
                    card = "J";
                    break;
                case 12:
                    card = "Q";
                    break;
                case 13:
                    card = "K";
                    break;
                default:
                    card = num.ToString();
                    break;
            }
            if (num > 10) cards.Add(new (10, item, card));
            else cards.Add(new (value, item, card));
        }
    }
    List<(int value, string suite, string number)> sortedCards = new List<(int value, string suite, string number)>();

    for (int i = cards.Count(); i > 0; i--)
    {
        (int value, string suite, string number) card = cards[rand.Next(0, cards.Count())];
        cards.Remove(card);
        sortedCards.Add(card);
        
    }
    return sortedCards;
}

class PlayerInfo
{
    public string? name {get; set;}
    public int money {get; set;}
    public List<(int, string)> hand {get; set;}
}

class Hands
{
    public List<(int value, string suite, string number)> PlayerHand {get; set;}
    public int PlayerValue {get; set;}
    public List<(int value, string suite, string number)> HouseHand {get; set;}
    public int HouseValue {get; set;}
}