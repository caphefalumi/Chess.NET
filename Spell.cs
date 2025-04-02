using System;
using System.Collections.Generic;

namespace Chess
{
    public enum SpellType
    {
        Teleport,
        Freeze
    }

    public class Spell
    {
        public SpellType Type { get; private set; }
        public bool IsUsed { get; private set; }

        public Spell(SpellType type)
        {
            Type = type;
            IsUsed = false;
        }

        public void Use()
        {
            IsUsed = true;
        }
    }

    public class SpellManager
    {
        private Dictionary<Player, Dictionary<SpellType, List<Spell>>> playerSpells;

        public SpellManager()
        {
            playerSpells = new Dictionary<Player, Dictionary<SpellType, List<Spell>>>();
        }

        public void InitializeSpells(Player player)
        {
            if (!playerSpells.ContainsKey(player))
            {
                playerSpells[player] = new Dictionary<SpellType, List<Spell>>();
                
                // Add 2 teleport spells
                playerSpells[player][SpellType.Teleport] = new List<Spell>();
                for (int i = 0; i < 2; i++)
                {
                    playerSpells[player][SpellType.Teleport].Add(new Spell(SpellType.Teleport));
                }
                
                // Add 5 freeze spells
                playerSpells[player][SpellType.Freeze] = new List<Spell>();
                for (int i = 0; i < 5; i++)
                {
                    playerSpells[player][SpellType.Freeze].Add(new Spell(SpellType.Freeze));
                }
            }
        }

        public int GetSpellCount(Player player, SpellType type)
        {
            if (!playerSpells.ContainsKey(player) || !playerSpells[player].ContainsKey(type))
                return 0;
                
            return playerSpells[player][type].Count(s => !s.IsUsed);
        }

        public bool HasUnusedSpell(Player player, SpellType type)
        {
            if (!playerSpells.ContainsKey(player) || !playerSpells[player].ContainsKey(type))
                return false;
                
            return playerSpells[player][type].Any(s => !s.IsUsed);
        }

        public void UseSpell(Player player, SpellType type)
        {
            if (playerSpells.ContainsKey(player) && playerSpells[player].ContainsKey(type))
            {
                var spell = playerSpells[player][type].FirstOrDefault(s => !s.IsUsed);
                if (spell != null)
                {
                    spell.Use();
                }
            }
        }
    }
} 