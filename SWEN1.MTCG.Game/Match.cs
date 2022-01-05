using System;
using System.Threading;
using SWEN1.MTCG.Game.Interfaces;
using IUser = SWEN1.MTCG.Game.Interfaces.IUser;

namespace SWEN1.MTCG.Game
{
    public class Match
    {
        public ILogging Logger { get; set; }
        public IUser Player1 { get; }
        public IUser Player2 { get; private set; }
        private int Player1RoundWon { get; set; }
        private int Player2RoundWon { get; set; }
        private bool Running { get; set; }
        private readonly int _maxRound;

        public Match(IUser player1, int maxRound)
        {
            Player1 = player1;
            _maxRound = maxRound;
        }

        public void AddUser(IUser player2)
        {
            Player2 = player2;
        }

        public void BattleAction(ILogging logger)
        {
            Logger = logger;
            
            Running = true;
            int round = 1;
            
            while (round <= _maxRound && Player1.Deck.Count > 0 && Player2.Deck.Count > 0)
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

                double myDamageAdj = myCard.Damage;
                double enemyDamageAdj = enemyCard.Damage;

                string myMonsterStatus = myCard.CheckEffect(enemyCard);
                string enemyMonsterStatus = enemyCard.CheckEffect(myCard);

                if (string.IsNullOrEmpty(myMonsterStatus) && string.IsNullOrEmpty(enemyMonsterStatus))
                {
                    if (myCard.Type == Type.Spell || enemyCard.Type == Type.Spell)
                    {
                        myDamageAdj = myCard.CompareElement(enemyCard.Element);
                        enemyDamageAdj = enemyCard.CompareElement(myCard.Element);
                        Logger.AppendLogWithLine($"=> {myCard.Damage} VS {enemyCard.Damage} -> {myDamageAdj} VS {enemyDamageAdj}{Environment.NewLine}");
                    }
                }
                else if (!string.IsNullOrEmpty(myMonsterStatus))
                {
                    Logger.AppendLogWithLine(myMonsterStatus);
                }
                else
                {
                    Logger.AppendLogWithLine(enemyMonsterStatus);
                }

                // Player 1 crit-chance 1/10
                if (rd.Next(10) == 0 && string.IsNullOrEmpty(myMonsterStatus))
                {
                    myDamageAdj *= 2;
                    Logger.AppendLogWithLine($"{myCard.Name} critical damage!");
                    Logger.AppendLogWithLine($"=> {myDamageAdj} VS {enemyDamageAdj}{Environment.NewLine}");
                }

                // Player 2 crit-chance 1/10
                if (rd.Next(10) == 0 && string.IsNullOrEmpty(enemyMonsterStatus))
                {
                    enemyDamageAdj *= 2;
                    Logger.AppendLogWithLine($"{enemyCard.Name} critical damage!");
                    Logger.AppendLogWithLine($"=> {myDamageAdj} VS {enemyDamageAdj}{Environment.NewLine}");
                }
                
                // Player 1 wins
                if (myDamageAdj > enemyDamageAdj || !string.IsNullOrEmpty(enemyMonsterStatus))
                {
                    Logger.AppendLogWithLine($"=> {myCard.Name} wins.{Environment.NewLine}");
                    myCards.Add(enemyCard);
                    enemyCards.Remove(enemyCard);
                    Player1RoundWon++;
                }
                
                // Player 2 wins
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
                Logger.AppendLogWithLine($"Over {_maxRound} Rounds were played, let's decide it to a draw!");

            Running = false;
        }

        public void ProcessRunningGame()
        {
            Thread.Sleep(1000);
            while (Running)
            {
                Thread.Sleep(1000);
            }
        }
    }
}