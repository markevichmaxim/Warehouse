namespace Core.Models.DTO
{
    /// <summary>
    /// Class for passing data from the backend to the interface
    /// </summary>
    /// <typeparam name="TStatus"></typeparam>
    public class EventManager<TStatus>
    {
        public delegate void BoxEvent((double side, double height) key, TStatus status, int existingQty, int remainingQty);
        public event BoxEvent OnBoxEvent = delegate { };

        public delegate void ExpiredBoxesEvent((double side, double height) key, bool status, int qty, DateTime expiration);
        public event ExpiredBoxesEvent OnRemoveExpiredBoxesEvent = delegate { };

        public void InvokeBoxEvent((double, double) key, TStatus status, int existingQty, int remainingQty)
        {
            OnBoxEvent?.Invoke(key, status, existingQty, remainingQty);
        }

        public void InvokeExpiredBoxesEvent((double, double) key, bool status, int qty, DateTime expiration)
        {
            OnRemoveExpiredBoxesEvent?.Invoke(key, status, qty, expiration);
        }
    }
}
