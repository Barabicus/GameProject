public interface IDamageable
{
    bool Damage(int damage);
    int CurrentHp { get; set; }
    int MaxHP { get; set; }
}