using UnityEngine;

namespace AI.BoxerFigth
{
    public enum BoxerFightAnimationType
    {
        Corner,
        Walk,
        FriendlyPunch,
        Hit,
        Attack,
        Dodge,
        Win,
        Lose,
        Draw,
    }
    
    public class BoxerFightAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        public void SetAnimation(BoxerFightAnimationType boxerFightAnimationType)
        {
            string text = string.Empty;
            Color color = Color.white;
            
            switch (boxerFightAnimationType)
            {
                case BoxerFightAnimationType.Corner:
                    animator.SetTrigger("Corner");
                    break;
                case BoxerFightAnimationType.Walk:
                    animator.SetTrigger("Walk");
                    break;
                case BoxerFightAnimationType.FriendlyPunch:
                    animator.SetTrigger("FriendlyPunch");
                    break;
                case BoxerFightAnimationType.Hit:
                    animator.SetTrigger("Hit");
                    text = "*HIT*";
                    break;
                case BoxerFightAnimationType.Attack:
                    animator.SetTrigger("Attack");
                    int attackIndex = Random.Range(0, 3);
                    animator.SetInteger("AttackIndex", attackIndex);
                    break;
                case BoxerFightAnimationType.Dodge:
                    animator.SetTrigger("Dodge");
                    text = "*DODGE*";
                    break;
                case BoxerFightAnimationType.Win:
                    animator.SetTrigger("Win");
                    text = "Yayy!";
                    break;
                case BoxerFightAnimationType.Lose:
                    animator.SetTrigger("Lose");
                    text = ":(";
                    break;
                case BoxerFightAnimationType.Draw:
                    animator.SetTrigger("Draw");
                    text = ":|";
                    break;
            }
            
            if (!string.IsNullOrEmpty(text))
            {
                FloatingTextManager.Instance.ShowWorldText(transform.position, text, color, 0.8f);
            }
        }
    }
}