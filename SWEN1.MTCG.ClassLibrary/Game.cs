using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace SWEN1.MTCG.ClassLibrary
{
    public class Game
    {
        public int Round { get; private set; }
        
        private User _player1;
        private User _player2;

        public Game(User player1, User player2)
        {
            _player1 = player1;
            _player2 = player2;
            Round = 1;
        }

        public static double CompareElement(Card playerCard, Card enemyCard)
        {
            double damageAdj;

            if ((playerCard.Element == Element.Water && enemyCard.Element == Element.Fire) ||
                (playerCard.Element == Element.Fire && enemyCard.Element == Element.Normal) ||
                (playerCard.Element == Element.Normal && enemyCard.Element == Element.Water))
            {
                damageAdj = playerCard.Damage * 2;
            } 
            else if ((playerCard.Element == Element.Fire && enemyCard.Element == Element.Water) ||
                     (playerCard.Element == Element.Water && enemyCard.Element == Element.Normal) ||
                     (playerCard.Element == Element.Normal && enemyCard.Element == Element.Fire))
            {
                damageAdj = playerCard.Damage * 0.5;
            }
            else
            {
                damageAdj = playerCard.Damage * 0;
            }

            return damageAdj;
        }
        public static bool CompareMonsterEffects(Card playerCard, Card enemyCard)
        {
            if (playerCard is MonsterCard monsterPlayerCard && enemyCard is MonsterCard monsterEnemyCard)
            {
                if(monsterPlayerCard.MonsterType == Monster.Goblin && monsterEnemyCard.MonsterType == Monster.Dragon) 
                {
                    Console.WriteLine($"{monsterPlayerCard.Name} is afraid of {monsterEnemyCard.Name}!");
                    return true;
                }
                if (monsterPlayerCard.MonsterType == Monster.Ork && monsterEnemyCard.MonsterType == Monster.Wizard)
                {
                    Console.WriteLine($"{monsterEnemyCard.Name} is putting {monsterPlayerCard.Name} under control!");
                    return true;
                }
                if (monsterPlayerCard.MonsterType == Monster.Dragon && monsterEnemyCard.Element == Element.Fire &&
                         monsterEnemyCard.MonsterType == Monster.Elf) 
                {
                    Console.WriteLine($"{monsterEnemyCard.Name} is able to evade {monsterPlayerCard.Name}'s attack!");
                    return true;
                }
            }
            
            else if (playerCard is MonsterCard monsterPlayerCard2 && enemyCard is SpellCard spellEnemyCard)
            {
                if (monsterPlayerCard2.MonsterType == Monster.Knight && spellEnemyCard.Element == Element.Water)
                {
                    Console.WriteLine($"{monsterPlayerCard2.Name} drowned in the {spellEnemyCard.Name}!");
                    return true;
                }
            }
            else if (playerCard is SpellCard spellPlayerCard2 && enemyCard is MonsterCard monsterEnemyCard2)
            {
                if (monsterEnemyCard2.MonsterType == Monster.Kraken)
                {
                    Console.WriteLine($"{monsterEnemyCard2.Name} is immune against spells!");
                    return true;
                }
            }
            
            return false;
        }
        public void BattleAction()
        {
            Random rd = new Random();
            List<Card> player1Cards = _player1.DeckCollection;
            List<Card> player2Cards = _player2.DeckCollection;
            
            int rdPlayer1 = rd.Next(player1Cards.Count);
            int rdPlayer2 = rd.Next(player2Cards.Count);

            var player1ChosenCard = player1Cards[rdPlayer1];
            var player2ChosenCard = player2Cards[rdPlayer2];

            Console.WriteLine($"{_player1.Username}'s DeckList:");
            for (int i = 0; i < player1Cards.Count; i++)
            {
                Console.WriteLine($"{i+1}: {player1Cards[i].Name} ({player1Cards[i].Damage} Damage)");
            }
            
            Console.WriteLine($"\n{_player2.Username}'s DeckList:");
            for (int i = 0; i < player2Cards.Count; i++)
            {
                Console.WriteLine($"{i+1}: {player2Cards[i].Name} ({player2Cards[i].Damage} Damage)");
            }
            
            Console.WriteLine($"\nRound {Round}");
            Console.WriteLine($"{_player1.Username}: {player1ChosenCard.Name} ({player1ChosenCard.Damage} Damage) " +
                              $"VS {_player2.Username}: {player2ChosenCard.Name} ({player2ChosenCard.Damage} Damage)\n");

            double damageAdj1 = 0;
            double damageAdj2 = 0;

            bool monsterStatus1 = CompareMonsterEffects(player1ChosenCard, player2ChosenCard);
            bool monsterStatus2 = CompareMonsterEffects(player2ChosenCard, player1ChosenCard);

            if (monsterStatus1 == false && monsterStatus2 == false)
            {
                if (player1ChosenCard is MonsterCard && player2ChosenCard is MonsterCard)
                {
                    damageAdj1 = player1ChosenCard.Damage;
                    damageAdj2 = player2ChosenCard.Damage;
                }
            
                else
                {
                    damageAdj1 = CompareElement(player1ChosenCard, player2ChosenCard);
                    damageAdj2 = CompareElement(player2ChosenCard, player1ChosenCard);
                    Console.WriteLine($"=> {player1ChosenCard.Damage} VS {player2ChosenCard.Damage} -> {damageAdj1} VS {damageAdj2}\n");
                }
            }

            if (damageAdj1 > damageAdj2 || monsterStatus2)
            {
                Console.WriteLine($"=> {player1ChosenCard.Name} wins.\n");
                player1Cards.Add(player2ChosenCard);
                player2Cards.Remove(player2ChosenCard);
            }
            else if (damageAdj1 < damageAdj2 || monsterStatus1)
            {
                Console.WriteLine($"=> {player2ChosenCard.Name} wins.\n");
                player2Cards.Add(player1ChosenCard);
                player1Cards.Remove(player1ChosenCard);
            }
            
            else
            {
                Console.WriteLine("Draw (no action)\n");
            }

            Round++;
        }
    }
}