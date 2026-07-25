using BigHappyBurger.Interaction;
using Shears;
using Shears.Tweens;
using UnityEngine;

namespace BigHappyBurger.Foods
{
    public class BagExpander : MonoBehaviour
    {
        [SerializeField, Required, Local]
        private Item item;

        [SerializeField, Required, Local]
        private Transform scaleTarget;

        [SerializeField, Local]
        private Vector3 expandSize = Vector3.one;

        [SerializeField, Local]
        private TweenData scaleTween = new(0.5f, easingFunction: TweenEase.OutBack);

        private void Start()
        {
            item.LockRotation();
            var tween = scaleTarget.GetScaleLocalTween(expandSize, scaleTween).PlayAfter(0.15f);

            tween.Completed += () => item.UnlockRotation();
        }
    }
}
