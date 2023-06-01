using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public class FireReloader : RealoaderBase
    {
        private TankFireSettings.FireTankFirePreset _firePreset;

        private float _time;
        private float _endFlameTime;

        public FireReloader(Image reloadImage) : base(reloadImage)
        {
        }

        public void Setup(TankFireSettings.FireTankFirePreset firePreset)
        {
            _firePreset = firePreset;
            _reloadImage.fillAmount = 0;
        }

        public override void OnFire()
        {
            if (_time == 0)
            {
                _endFlameTime = Time.time + _firePreset.FlameAttackTime;
            }
            
            _time = Time.time;
            
            FillReloadImage(0.2f, _time / _endFlameTime, UniTaskUtil.RefreshToken(ref _fillToken));
        }

        public override void OnStopFire(bool isOverhead)
        {
            base.OnStopFire(isOverhead);

            _time = 0;
            FillReloadImage(isOverhead ? _firePreset.ReloadTime : 0.2f, 0,
                UniTaskUtil.RefreshToken(ref _fillToken));
        }
    }
}