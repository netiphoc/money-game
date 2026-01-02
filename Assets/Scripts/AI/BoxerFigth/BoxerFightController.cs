using UnityEngine;

namespace AI.BoxerFigth
{
    public class BoxerFightController : MonoBehaviour
    {
        [SerializeField] private BoxerFightAnimation boxerFightAnimation;

        private bool _isPlayer;
        
        public void SetFightData(FightData fightData, bool isPlayer)
        {
            _isPlayer = isPlayer;
            fightData.OnAction += OnFightActionType;
        }

        private void OnFightActionType(FightData arg1, FightActionType fightActionType)
        {
            switch (fightActionType)
            {
                case FightActionType.PLAYER_HITS:
                    boxerFightAnimation.SetAnimation(_isPlayer
                        ? BoxerFightAnimationType.Attack
                        : BoxerFightAnimationType.Hit);
                    break;
                case FightActionType.PLAYER_MISS:
                    boxerFightAnimation.SetAnimation(_isPlayer
                        ? BoxerFightAnimationType.Attack
                        : BoxerFightAnimationType.Dodge);
                    break;
                case FightActionType.ENEMY_HITS:
                    boxerFightAnimation.SetAnimation(_isPlayer
                        ? BoxerFightAnimationType.Hit
                        : BoxerFightAnimationType.Attack);
                    break;
                case FightActionType.ENEMY_MISS:
                    boxerFightAnimation.SetAnimation(_isPlayer
                        ? BoxerFightAnimationType.Dodge
                        : BoxerFightAnimationType.Attack);
                    break;
                case FightActionType.GAME_RESULT_WIN:
                    boxerFightAnimation.SetAnimation(_isPlayer
                        ? BoxerFightAnimationType.Win
                        : BoxerFightAnimationType.Lose);
                    break;
                case FightActionType.GAME_RESULT_LOSE:
                    boxerFightAnimation.SetAnimation(_isPlayer
                        ? BoxerFightAnimationType.Lose
                        : BoxerFightAnimationType.Win);
                    break;
                case FightActionType.GAME_RESULT_DRAW:
                    boxerFightAnimation.SetAnimation(BoxerFightAnimationType.Draw);
                    break;
            }
        }
    }
}