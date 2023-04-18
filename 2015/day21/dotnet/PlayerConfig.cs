namespace day21;

internal class PlayerConfig
{
    public int Weapon { get; }
    public int? Armor { get; }
    public int? LeftRing { get; }
    public int? RightRing { get; }

    public static PlayerConfig GetFirst()
    {
        return new PlayerConfig(0, null, null, null);
    }
    
    private PlayerConfig(int myWeapon, int? myArmor, int? myLeftRing, int? myRightRing)
    {
        Weapon = myWeapon;
        Armor = myArmor;
        LeftRing = myLeftRing;
        RightRing = myRightRing;
    }

    public override string ToString()
    {
        return $"Config: W={Weapon} A={Armor} LR={LeftRing} RR={RightRing}";
    }
    
    public PlayerConfig? Next(ItemDatabase itemDatabase)
    {
        var _numWeapons = itemDatabase.Weapons.Length;
        var _numArmors = itemDatabase.Armors.Length;
        var _numRings = itemDatabase.Rings.Length;
        
        int nextWeapon = Weapon;
        int? nextArmor = Armor;
        int? nextLeftRing = LeftRing;
        int? nextRightRing = RightRing;

        var weaponOverflow = false;
        var armorOverflow = false;
        var leftRingOverflow = false;
        var rightRingOverflow = false;
        
        nextWeapon++;
        if (nextWeapon >= _numWeapons)
        {
            nextWeapon = 0;
            weaponOverflow = true;
        }

        if (weaponOverflow)
        {
            if (nextArmor.HasValue)
            {
                nextArmor++;
                if (nextArmor >= _numArmors)
                {
                    nextArmor = null;
                    armorOverflow = true;
                }
            }
            else
            {
                nextArmor = 0;
            }
        }

        if (armorOverflow)
        {
            if (nextLeftRing.HasValue)
            {
                nextLeftRing++;
                if (nextLeftRing >= _numRings)
                {
                    nextLeftRing = null;
                    leftRingOverflow = true;
                }
            }
            else
            {
                nextLeftRing = 0;
            }
        }

        if (leftRingOverflow)
        {
            if (nextRightRing.HasValue)
            {
                nextRightRing++;
                // xxx
                if (nextRightRing == nextLeftRing)
                    nextRightRing++;
                // xxx
                if (nextRightRing >= _numRings)
                {
                    nextRightRing = null;
                    rightRingOverflow = true;
                }
            }
            else
            {
                nextRightRing = 0;
            }
        }

        if (rightRingOverflow)
        {
            return null;
        }

        return new PlayerConfig(nextWeapon, nextArmor, nextLeftRing, nextRightRing);
    }
}