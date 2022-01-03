using System;
using SWEN1.MTCG.Game.Interfaces;
using IUser = SWEN1.MTCG.Game.Interfaces.IUser;

namespace SWEN1.MTCG.Game
{
    public class Match
    {
        public ILogging Logger { get; set; }
        private int MaxRound = 100;

        public IUser Player1 { get; }
        public IUser Player2 { get; private set; }
        public int PlayerCount { get; private set; } = 1;
        
        private int Player1RoundWon { get; set; }
        private int Player2RoundWon { get; set; }
        
        public bool Running { get; private set; }


        public Match(IUser player1)
        {
            Player1 = player1;
        }

        public void AddUser(IUser player2)
        {
            Player2 = player2;
            PlayerCount++;
        }

        public void BattleAction(ILogging logger)
        {
            Logger = logger;
            
            Running = true;
            int round = 1;
            
            while (round <= MaxRound && Player1.Deck.Count > 0 && Player2.Deck.Count > 0)
            {
                var rd = new Random();

                var myCards = Player1.Deck;
                var enemyCards = Player2.Deck;

                var rd1 = rd.Next(myCards.Count);
                var rd2 = rd.Next(enemyCards.Count);

                var myCard = myCards[rd1];
                var enemyCard = enemyCards[rd2];

                Logger.AppendLogWithLine($"{Player1.Username}'s DeckList:");
                foreach (var player1Card in myCards)
                    Logger.AppendLogWithLine($"- {player1Card.Name} ({player1Card.Damage} Damage)");
                

                Logger.AppendLogWithLine($"{Environment.NewLine}{Player2.Username}'s DeckList:");
                foreach (var player2Card in enemyCards)
                    Logger.AppendLogWithLine($"- {player2Card.Name} ({player2Card.Damage} Damage)");
                

                Logger.AppendLogWithLine($"{Environment.NewLine}Round {round}");
                Logger.AppendLogWithLine($"{Player1.Username}: {myCard.Name} ({myCard.Damage} Damage) " +
                                         $"VS {Player2.Username}: {enemyCard.Name} ({enemyCard.Damage} Damage)");

                double myDamageAdj = 0;
                double enemyDamageAdj = 0;

                string myMonsterStatus = myCard.CheckEffect(enemyCard);
                string enemyMonsterStatus = enemyCard.CheckEffect(myCard);

                if (string.IsNullOrEmpty(myMonsterStatus) && string.IsNullOrEmpty(enemyMonsterStatus))
                {
                    if (myCard.Type != Type.Spell && enemyCard.Type != Type.Spell)
                    {
                        myDamageAdj = myCard.Damage;
                        enemyDamageAdj = enemyCard.Damage;
                    }
                
                    else
                    {
                        myDamageAdj = myCard.CompareElement(enemyCard.Element);
                        enemyDamageAdj = enemyCard.CompareElement(myCard.Element);
                        Logger.AppendLogWithLine($"=> {myCard.Damage} VS {enemyCard.Damage} -> {myDamageAdj} VS {enemyDamageAdj}{Environment.NewLine}");
                    }
                }

                else
                {
                    Logger.AppendLogWithLine(myMonsterStatus);
                    Logger.AppendLogWithLine(enemyMonsterStatus);
                }

                if (myDamageAdj > enemyDamageAdj || !string.IsNullOrEmpty(enemyMonsterStatus))
                {
                    Logger.AppendLogWithLine($"=> {myCard.Name} wins.{Environment.NewLine}");
                    myCards.Add(enemyCard);
                    enemyCards.Remove(enemyCard);
                    Player1RoundWon++;
                }
                
                else if (myDamageAdj < enemyDamageAdj || !string.IsNullOrEmpty(myMonsterStatus))
                {
                    Logger.AppendLogWithLine($"=> {enemyCard.Name} wins.{Environment.NewLine}");
                    enemyCards.Add(myCard);
                    myCards.Remove(myCard);
                    Player2RoundWon++;
                }

                else
                    Logger.AppendLogWithLine($"Draw (no action){Environment.NewLine}");

                round++;
            }
        
            if (Player1.Deck.Count <= 0)
                Logger.AppendLogWithLine($"{Player2.Username} won the game with {Player2RoundWon} of {round} rounds!");
            
            else if (Player2.Deck.Count <= 0)
                Logger.AppendLogWithLine($"{Player1.Username} won the game with {Player1RoundWon} of {round} rounds!");
            
            else
                Logger.AppendLogWithLine($"Over {MaxRound} Rounds were played, let's decide it to a draw!");

            Running = false;
        }
    }
}