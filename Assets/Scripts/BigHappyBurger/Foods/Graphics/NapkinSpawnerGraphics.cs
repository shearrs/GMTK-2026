using BigHappyBurger.Interaction;
using Shears;
using UnityEngine;

namespace BigHappyBurger.Foods.Graphics
{
    [RequireComponent(typeof(ItemSpawner))]
    public partial class NapkinSpawnerGraphics : MonoBehaviour
    {
        [SerializeField, Required(targetCollectionSize: 3), Local]
        private GameObject[] napkinStacks;

        [Auto]
        [AutoEvent(nameof(ItemSpawner.CountChanged), nameof(OnCountChanged))]
        private ItemSpawner spawner;

        private void OnCountChanged(int count)
        {
            if (count == 0)
            {
                foreach (var stack in napkinStacks)
                    stack.SetActive(false);

                return;
            }

            float t = (float)count / spawner.MaxCount * napkinStacks.Length;
            int lastIndex = Mathf.RoundToInt(t);

            for (int i = 0; i < napkinStacks.Length; i++)
            {
                var stack = napkinStacks[i];

                if (i <= lastIndex)
                {
                    if (!stack.activeSelf)
                        stack.SetActive(true);
                }
                else if (stack.activeSelf)
                    stack.SetActive(false);
            }
        }
    }
}
