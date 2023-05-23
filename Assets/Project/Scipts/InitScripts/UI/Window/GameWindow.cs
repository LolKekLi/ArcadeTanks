using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Project.Meta;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Project.UI
{
    public class GameWindow : Window
    {
        [SerializeField, Space]
        private Transform _cross;

        [SerializeField]
        private SlicedFilledImage _hpBar;

        [SerializeField]
        private Image _bodyIcon;

        [SerializeField]
        private Image _turretIcon;

        [SerializeField]
        private Image _reloadImage;

        private RealoaderBase _realoader;

        [Inject]
        private IUser _user;
        [Inject]
        private UIIconSettings _uiIconSettings;
        [Inject]
        private TankFireSettings _tankFireSettings;
        [Inject]
        private TankBodySettings _tankBodySettings;

        private float _maxHp;
        private float  _currentHp;

        public override bool IsPopup
        {
            get =>
                false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            TankController.Fired += TankController_Fired;
            TankController.Damaged += TankController_Damaged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            TankController.Fired -= TankController_Fired;
            TankController.Damaged -= TankController_Damaged;

            _realoader?.CanselToken();
        }

        protected override void OnShow()
        {
            base.OnShow();

            var turretTypeValue = _user.TurretType.Value;
            var bodyTypeValue = _user.BodyType.Value;
            
            _hpBar.fillAmount = 1;

            _bodyIcon.sprite = _uiIconSettings.GetBodyIcon(bodyTypeValue);
            
            _turretIcon.sprite = _uiIconSettings.GetTurretIcon(turretTypeValue);

            _realoader = GetUIReloader(turretTypeValue);

            _maxHp = _tankBodySettings.GetPresetByType(bodyTypeValue).HP;
        }

        private RealoaderBase GetUIReloader(TurretType type)
        {
            switch (type)
            {
                case TurretType.Classic:
                    var reloader = new ClassicReloader(_reloadImage);
                    reloader.Setup(_tankFireSettings.ClassicFirePresets);
                    return reloader;
                default:
                    return null;
            }
        }

        private void TankController_Damaged(float tankHp)
        {
            _hpBar.fillAmount = tankHp / (float)_maxHp;
        }

        private void TankController_Fired()
        {
            _realoader.Reload();
        }
    }

    public abstract class RealoaderBase
    {
        protected Image _reloadImage;
        protected CancellationTokenSource _fillToken;

        public RealoaderBase(Image reloadImage)
        {
            _reloadImage = reloadImage;
         
        }

        public abstract void Reload();

        protected void FillReloadImage(float fillTime, int targetValue, CancellationToken cancellationToken)
        {
            try
            {
                var startValue = _reloadImage.fillAmount;
                UniTaskExtensions.Lerp(time =>
                    {
                        _reloadImage.fillAmount = Mathf.Lerp(startValue, targetValue, time);
                    },
                    fillTime, token: cancellationToken);
            }
            catch (OperationCanceledException e)
            {
            }
        }

        public virtual void CanselToken()
        {
            UniTaskUtil.CancelToken(ref _fillToken);
        }
    }

    public class ClassicReloader : RealoaderBase
    {
        private TankFireSettings.ClassTankFirePreset _firePreset;

        public ClassicReloader( Image reloadImage)
            : base(reloadImage)
        {
        }

        public void Setup(TankFireSettings.ClassTankFirePreset firePreset)
        {
            _firePreset = firePreset;
        }

        public override void Reload()
        {
            _reloadImage.fillAmount = 0;

            FillReloadImage(_firePreset.ReloadTime, 1, UniTaskUtil.RefreshToken(ref _fillToken));
        }
    }
    
    public class FireReloader : RealoaderBase
    {
        public FireReloader(Image reloadImage) : base(reloadImage)
        {
        }

        public override void Reload()
        {
            
        }
    }
}