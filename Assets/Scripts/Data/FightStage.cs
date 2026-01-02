using AI.BoxerFigth;
using UnityEngine;

namespace Data
{
    public class FightStage : MonoBehaviour
    {
        [field: SerializeField] public FightDataSO FightDataSo { get; private set; }
        [field: SerializeField] public BoxerFightController BoxerFightPlayer { get; private set; }
        [field: SerializeField] public BoxerFightController BoxerFightEnemy { get; private set; }
        
        [SerializeField] public Camera liveCam;

        public RenderTexture GetLiveCamRenderTexture()
        {
            return liveCam.targetTexture;
        }
    }
}