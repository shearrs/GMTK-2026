using Shears;
using UnityEngine;

namespace BigHappyBurger.Customers.Graphics
{
    [RequireComponent(typeof(Animator))]
    public partial class CustomerAnimator : MonoBehaviour
    {
        [Auto]
        private Animator animator;
        private Customer customer;

        private readonly int isAttentiveID = Animator.StringToHash("isAttentive");
        private readonly int disappointedID = Animator.StringToHash("disappointed");
        private readonly int grabID = Animator.StringToHash("grab");

        private void Awake()
        {
            __AutoAwake();
        }

        private void OnDestroy()
        {
            if (customer == null)
                return;

            customer.ReachedWindow -= OnReachedWindow;
            customer.ReceivedRightItem -= OnReceivedRightItem;
            customer.ReceivedWrongItem -= OnReceivedWrongItem;
            customer.BeganExiting -= OnBeganExiting;
            customer.WaitedTooLong -= OnReceivedWrongItem;
        }

        internal void SetCustomer(Customer customer)
        {
            this.customer = customer;

            customer.ReachedWindow += OnReachedWindow;
            customer.ReceivedRightItem += OnReceivedRightItem;
            customer.ReceivedWrongItem += OnReceivedWrongItem;
            customer.BeganExiting += OnBeganExiting;
            customer.WaitedTooLong += OnReceivedWrongItem;
        }

        private void OnReachedWindow()
        {
            animator.SetBool(isAttentiveID, true);
        }

        private void OnReceivedRightItem()
        {
            animator.SetTrigger(grabID);
        }

        private void OnReceivedWrongItem()
        {
            animator.SetTrigger(disappointedID);
        }

        private void OnBeganExiting()
        {
            animator.SetBool(isAttentiveID, false);
        }
    }
}
