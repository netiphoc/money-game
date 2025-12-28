using Utilities;

namespace Core
{
    public class GameController : BaseMonoBehaviour
    {
        public void StartTheDay()
        {
            GameManager.Instance.GameTimeManager.StartClock();
        }

        public void EndTheDay()
        {
        }
    }
}