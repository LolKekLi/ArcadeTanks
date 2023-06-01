using UnityEngine;
using UnityEngine.UI;

namespace Project.UI
{
    public class TwoGunReloader : RealoaderBase
    {
        private bool _fireIsStop = false;
        
        private float _time;
        private float _endFireTime;

        private TankFireSettings.TwoGunAttackPreset _twoGunFirePreset;

        public TwoGunReloader(Image reloadImage) : base(reloadImage)
        {
        }

        public void Setup(TankFireSettings.TwoGunAttackPreset twoGunFirePreset)
        {
            _twoGunFirePreset = twoGunFirePreset;
        }

        public override void OnFire()
        {
            if (_time == 0)
            {
                _endFireTime = Time.time + _twoGunFirePreset.OverclockingTime + _twoGunFirePreset.OverHeatTime;
            }
            
            _time = Time.time;
            
            FillReloadImage(0.2f, _time / _endFireTime, UniTaskUtil.RefreshToken(ref _fillToken));
        }
        
        public override void OnStopFire(bool isOverhead)
        {
            if (_fireIsStop)
            {
                return;
            }
            
            base.OnStopFire(isOverhead);

            _time = 0;
            
            _fireIsStop = true;
            
            FillReloadImage(isOverhead ? _twoGunFirePreset.ReloadTime : 0.2f, 0,
                UniTaskUtil.RefreshToken(ref _fillToken), () =>
                {
                    _fireIsStop = false;
                });
        }
    }
}