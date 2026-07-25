using UnityEngine;

namespace BigHappyBurger.Interaction
{
    public interface IItemSpawner
    {
        public Item ItemToSpawn { get; }
        public int MaxCount { get; }

        public void AddCount(int count);
    }
}
