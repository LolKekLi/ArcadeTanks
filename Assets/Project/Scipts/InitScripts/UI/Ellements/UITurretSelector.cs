using System.Linq;
using ModestTree;

namespace Project.UI
{
    public class UITurretSelector : UISelector<TurretType>
    {
        protected override void Prepare()
        {
            _currentType.Value = _user.TurretType.Value;
            _selectorPresets = _uiIconSettings.TurretIconPresets;
        }

        protected override void Start()
        {
            var preset = _selectorPresets.FirstOrDefault(x => x.Type == CurrentType.Value);

            if (preset != null)
            {
                _index.Value = _selectorPresets.IndexOf(preset);
            }
            
            base.Start();
        }
    }
}