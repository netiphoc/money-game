using DG.Tweening;
using UnityEngine;

namespace AI.BoxerFigth
{
    public class BoxerFightController : MonoBehaviour
    {
        [SerializeField] private BoxerFightAnimation boxerFightAnimation;

        private bool _isPlayer;
        private Vector3 _spawnPosition;
        private Quaternion _spawnRotation;

        private void Awake()
        {
            _spawnPosition = transform.position;
            _spawnRotation = transform.rotation;
        }

        public void SetFightData(FightData fightData, bool isPlayer)
        {
            _isPlayer = isPlayer;
            fightData.OnAction += OnFightActionType;
            boxerFightAnimation.SetAnimation(BoxerFightAnimationType.Corner);
        }

        public Vector3 GetSpawnPosition() => _spawnPosition;
        public Quaternion GetSpawnRotation() => _spawnRotation;

        private void OnFightActionType(FightData fightData, FightActionType fightActionType)
        {
            switch (fightActionType)
            {
                case FightActionType.PREPARE:
                    //boxerFightAnimation.SetAnimation(BoxerFightAnimationType.Corner);
                    break;
                case FightActionType.FIGHT_STARTING:
                    boxerFightAnimation.SetAnimation(BoxerFightAnimationType.Walk);

                    transform.DOKill();
                    transform.DOMove(GetSpawnPosition(), FightData.StartDelay).SetEase(Ease.Linear);
                    transform.DORotateQuaternion(GetSpawnRotation(), 0.4f);
                    break;
                
                case FightActionType.FIGHT_STARTED:
                    boxerFightAnimation.SetAnimation(BoxerFightAnimationType.GetReady);
                    break;
                
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