using AI.BoxerFigth;
using UnityEngine;

namespace Data
{
    public class FightStage : MonoBehaviour
    {
        [field: SerializeField] public FightDataSO FightDataSo { get; private set; }
        
        [Header("Boxer Setup")]
        [field: SerializeField] public BoxerFightController BoxerFightPlayer { get; private set; }
        [field: SerializeField] public BoxerFightController BoxerFightEnemy { get; private set; }
        
        [Header("Stage Setup")]
        [SerializeField] private Transform startCornerPlayer;
        [SerializeField] private Transform startCornerEnemy;
        
        [SerializeField] public Camera liveCam;

        public RenderTexture GetLiveCamRenderTexture()
        {
            return liveCam.targetTexture;
        }

        public void ResetCorner()
        {
            // Player
            BoxerFightPlayer.transform.position = startCornerPlayer.position;
            BoxerFightPlayer.transform.rotation = startCornerPlayer.rotation;
            
            // Enemy
            BoxerFightEnemy.transform.position = startCornerEnemy.position;
            BoxerFightEnemy.transform.rotation = startCornerEnemy.rotation;
        }
    }
}