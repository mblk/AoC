namespace day21;

internal class PlayerFactory
{
    private readonly ItemDatabase _itemDatabase;

    public PlayerFactory(ItemDatabase itemDatabase)
    {
        _itemDatabase = itemDatabase;
    }
    
    public Player GetBoss()
    {
        // TODO parse input file or just hard code it?
        return new Player("boss", 104, 8, 1);
    }

    public (Player player, int cost) GetPlayerAndCost(PlayerConfig config)
    {
        var activeItems = new List<Item>();

        activeItems.Add(_itemDatabase.Weapons[config.Weapon]);

        if (config.Armor.HasValue)
            activeItems.Add(_itemDatabase.Armors[config.Armor.Value]);
        
        if (config.LeftRing.HasValue)
            activeItems.Add(_itemDatabase.Rings[config.LeftRing.Value]);
        
        if (config.RightRing.HasValue)
            activeItems.Add(_itemDatabase.Rings[config.RightRing.Value]);

        int hp = 100;
        int dmg = 0;
        int amr = 0;
        int cost = 0;

        foreach (var item in activeItems)
        {
            dmg += item.Damage;
            amr += item.Armor;
            cost += item.Cost;
        }

        return (new Player("player", hp, dmg, amr), cost);
    }
}