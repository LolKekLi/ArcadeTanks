using UnityEngine.UI;

namespace Project.UI
{
    public class ClassicReloader : RealoaderBase
    {
        private TankFireSettings.ClassTankFirePreset _firePreset;

        public ClassicReloader(Image reloadImage)
            : base(reloadImage)
        {
        }

        public void Setup(TankFireSettings.ClassTankFirePreset firePreset)
        {
            _firePreset = firePreset;
        }

        public override void OnFire()
        {
            _reloadImage.fillAmount = 0;

            FillReloadImage(_firePreset.ReloadTime, 1, UniTaskUtil.RefreshToken(ref _fillToken));
        }
    }
}