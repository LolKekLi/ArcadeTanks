namespace Project
{
    public interface IEffectTarget
    {
        void EnableArmor();
        void DisableArmor();
        void ChangeSpeed(float presetValue);
        void AddHP(float presetValue);
    }
}