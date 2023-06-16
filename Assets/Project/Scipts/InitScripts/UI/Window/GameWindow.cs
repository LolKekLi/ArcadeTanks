using Cysharp.Threading.Tasks;
using Project.Meta;
using TMPro;
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

        [SerializeField, Space]
        private TextMeshProUGUI _scoreText;

        [SerializeField]
        private BaseTweenController _scoreTextTween;

        private float _maxHp;
        private float _currentHp;

        private RealoaderBase _realoader;

        [Inject]
        private IUser _user;

        [Inject]
        private UIIconSettings _uiIconSettings;

        [Inject]
        private TankFireSettings _tankFireSettings;

        [Inject]
        private TankBodySettings _tankBodySettings;

        public override bool IsPopup
        {
            get =>
                false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            TankController.Fired += TankController_Fired;
            TankController.StopFire += TankController_StopFire;
            TankController.HpChanged += TankController_HpChanged;
            ScoreController.Changed += ScoreController_Changed;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            TankController.Fired -= TankController_Fired;
            TankController.StopFire -= TankController_StopFire;
            TankController.HpChanged -= TankController_HpChanged;
            ScoreController.Changed -= ScoreController_Changed;

            _realoader?.CanselToken();
        }

        protected override void OnShow()
        {
            base.OnShow();

            var turretTypeValue = _user.TurretType.Value;
            var bodyTypeValue = _user.BodyType.Value;
            _realoader = GetUIReloader(turretTypeValue);

            _hpBar.fillAmount = 1;

            RefreshScoreText(0);
            
            _bodyIcon.sprite = _uiIconSettings.GetBodyIcon(bodyTypeValue);
            _turretIcon.sprite = _uiIconSettings.GetTurretIcon(turretTypeValue);
            _maxHp = _tankBodySettings.GetPresetByType(bodyTypeValue).HP;
        }

        private RealoaderBase GetUIReloader(TurretType type)
        {
            switch (type)
            {
                case TurretType.Classic:
                    var classicReloader = new ClassicReloader(_reloadImage);
                    classicReloader.Setup(_tankFireSettings.ClassicFirePresets);
                    return classicReloader;

                case TurretType.Fire:
                    var fireReloader = new FireReloader(_reloadImage);
                    fireReloader.Setup(_tankFireSettings.FireTankFirePresets);
                    return fireReloader;

                case TurretType.TwoGuns:
                    var twoGunReloader = new TwoGunReloader(_reloadImage);
                    twoGunReloader.Setup(_tankFireSettings.TwoGunFirePreset);
                    return twoGunReloader;

                default:
                    return null;
            }
        }

        private void RefreshScoreText(int scoreValue)
        {
            _scoreTextTween.Play();
            _scoreText.text = $"X{scoreValue}";
        }

        private void TankController_HpChanged(float tankHp)
        {
            _hpBar.fillAmount = tankHp / (float)_maxHp;
        }

        private void TankController_Fired()
        {
            _realoader.OnFire();
        }

        private void TankController_StopFire(bool isOverhead)
        {
            _realoader.OnStopFire(isOverhead);
        }

        private void ScoreController_Changed(int value)
        {
            RefreshScoreText(value);
        }
    }
}