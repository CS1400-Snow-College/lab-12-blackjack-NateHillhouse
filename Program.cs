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
using System.Collections.Immutable;

Main();

void Main()
{
    Console.Clear();
    Random rand = new Random();
    string file = "blackjack.txt";
    //List<PlayerInfo> players = ReadFile(file);
    //PlayerInfo player = Intro(players);
        PlayerInfo player = new PlayerInfo() {name = "Nate", money = 50};
    Console.WriteLine($"Welcome {player.name}, you have ${player.money} in the bank.");
    //makeBet(player);
    Hands hands = new Hands(){PlayerHand = new List<(int, string)>(), HouseHand = new List<(int, string)>()};
    List<(int, string)> deck = CreateDeck(rand);
    
    //Deal Starting Deck
    hands = DealCards(deck, rand, hands, 2);
    //WriteEach Card in players hand
    ViewPlayerCards(hands, player);

    //WriteFile(players, player, file);
}

void ViewPlayerCards(Hands hands, PlayerInfo player)
{
    Console.Write($"{player.name}'s hand: ");
    hands.PlayerValue = 0;
    hands.HouseValue = 0;
    foreach ((int, string) item in hands.PlayerHand) 
    {
        hands.PlayerValue += item.Item1;
        Console.Write($"[{item.Item1}{item.Item2}] ");
    }
    Console.Write($"Total: {hands.PlayerValue}");

}

Hands DealCards(List<(int, string)> deck, Random rand, Hands hands, int cards)
{
    for (int i = cards; i>0; i--) hands.PlayerHand.Add(GetCard(deck, rand, hands));
    for (int i = cards; i>0; i--) hands.HouseHand.Add(GetCard(deck, rand, hands));
    return hands;
}

(int, string) GetCard(List<(int, string)> deck, Random rand, Hands hands)
{
    (int, string) card = deck[rand.Next(0, deck.Count())];
    deck.Remove(card);
    return card;
}

void makeBet(PlayerInfo player)
{
    int bet = InputToInt("How much would you like to bet? ");
    while (bet > player.money)
    {
        Console.Clear();
        bet = InputToInt($"You do not have enough money to bet this much. Please input a number under ${player.money} ");
    }
    player.money -= bet;
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

static List<(int, string)> CreateDeck(Random rand)
{
    List<(int number, string suite)> cards = new List<(int, string)>();
    List<string> suites = new List<string> {"♦","♥","♠","♣"};
    for (int num = 1; num<=13; num++)
    {
        foreach (string item in suites)
        {
            cards.Add(new (num, item));
        }
    }
    List<(int, string)> sortedCards = new List<(int, string)>();

    for (int i = cards.Count(); i > 0; i--)
    {
        (int, string) card = cards[rand.Next(0, cards.Count())];
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
    public List<(int, string)> PlayerHand {get; set;}
    public int PlayerValue {get; set;}
    public List<(int, string)> HouseHand {get; set;}
    public int HouseValue {get; set;}
}