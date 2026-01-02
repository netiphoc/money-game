using Data;
using UnityEngine;

namespace Core
{
    public class BoxerFightManager : MonoBehaviour
    {
        public static BoxerFightManager Instance;
        
        [SerializeField] private FightStage[] fightStages;
        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        public void OnFightDataReceived(FightData fightData)
        {
            if(!TryGetStage(fightData, out FightStage fightStageData)) return;
            
            fightStageData.BoxerFightPlayer.SetFightData(fightData, true);
            fightStageData.BoxerFightEnemy.SetFightData(fightData, false);
        }

        public bool TryGetStage(FightData fightData, out FightStage fightStage)
        {
            foreach (var stage in fightStages)
            {
                if(!stage.FightDataSo.Equals(fightData.FightDataSO)) continue;
                fightStage = stage;
                return true;
            }
            
            fightStage = null;
            return false;
        }
    }
}