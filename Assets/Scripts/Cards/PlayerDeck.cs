using System;
using System.Collections.Generic;
using System.Linq;

namespace CaravanRoguelite.Cards
{
    public sealed class PlayerDeck
    {
        private readonly List<CardDefinition> _drawPile = new();
        private readonly List<CardDefinition> _discardPile = new();
        private readonly List<CardDefinition> _exhaustedHand = new();
        private readonly Random _random;

        public PlayerDeck(IEnumerable<CardDefinition> startCards, int seed = 0)
        {
            _random = seed == 0 ? new Random() : new Random(seed);
            _drawPile.AddRange(startCards);
            Shuffle(_drawPile);
        }

        public int Count => _drawPile.Count + _discardPile.Count + _exhaustedHand.Count;
        public IReadOnlyList<CardDefinition> DrawPile => _drawPile;
        public IReadOnlyList<CardDefinition> DiscardPile => _discardPile;

        public IReadOnlyList<CardDefinition> Draw(int count)
        {
            var result = new List<CardDefinition>();
            while (result.Count < count)
            {
                if (_drawPile.Count == 0)
                {
                    ReshuffleDiscardPile();
                }

                if (_drawPile.Count == 0)
                {
                    break;
                }

                var card = _drawPile[0];
                _drawPile.RemoveAt(0);
                _exhaustedHand.Add(card);
                result.Add(card);
            }

            return result;
        }

        public void Discard(IEnumerable<CardDefinition> cards)
        {
            foreach (var card in cards)
            {
                _exhaustedHand.Remove(card);
                _discardPile.Add(card);
            }
        }

        public void DiscardHandRemainder()
        {
            _discardPile.AddRange(_exhaustedHand);
            _exhaustedHand.Clear();
        }

        public void Add(CardDefinition card)
        {
            _discardPile.Add(card);
        }

        public bool RemoveWeakest()
        {
            var all = _drawPile.Concat(_discardPile).Concat(_exhaustedHand).OrderBy(card => card.BasePower).ToList();
            if (all.Count <= 5)
            {
                return false;
            }

            var target = all[0];
            return _drawPile.Remove(target) || _discardPile.Remove(target) || _exhaustedHand.Remove(target);
        }

        public bool UpgradeRandom()
        {
            var all = _drawPile.Concat(_discardPile).Concat(_exhaustedHand).ToList();
            if (all.Count == 0)
            {
                return false;
            }

            var target = all[_random.Next(all.Count)];
            var upgraded = target.Upgrade();
            return Replace(_drawPile, target, upgraded) || Replace(_discardPile, target, upgraded) || Replace(_exhaustedHand, target, upgraded);
        }

        private void ReshuffleDiscardPile()
        {
            _drawPile.AddRange(_discardPile);
            _discardPile.Clear();
            Shuffle(_drawPile);
        }

        private void Shuffle(List<CardDefinition> cards)
        {
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (cards[i], cards[j]) = (cards[j], cards[i]);
            }
        }

        private static bool Replace(List<CardDefinition> cards, CardDefinition target, CardDefinition replacement)
        {
            int index = cards.IndexOf(target);
            if (index < 0)
            {
                return false;
            }

            cards[index] = replacement;
            return true;
        }
    }
}
