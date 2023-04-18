namespace day21;

internal class Player
{
    public string Name { get; }
    public int InitialHitpoints { get; }
    public int Damage { get; }
    public int Armor { get; }

    public int CurrentHitpoints { get; set; }
    
    public Player(string name, int hitpoints, int damage, int armor)
    {
        Name = name;
        InitialHitpoints = hitpoints;
        CurrentHitpoints = hitpoints;
        Damage = damage;
        Armor = armor;
    }

    public override string ToString()
    {
        return $"Player: HP={CurrentHitpoints}/{InitialHitpoints} DMG={Damage} AMR={Armor}";
    }
}