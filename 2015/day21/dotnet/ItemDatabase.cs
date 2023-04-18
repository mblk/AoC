namespace day21;

internal class ItemDatabase
{
    public Item[] Weapons { get; }
    public Item[] Armors { get; }
    public Item[] Rings { get; }

    public ItemDatabase()
    {
        Weapons = GetWeapons().ToArray();
        Armors = GetArmors().ToArray();
        Rings = GetRings().ToArray();
    }
    
    private IEnumerable<Item> GetWeapons()
    {
        yield return new Item("Dagger", 8, 4, 0);
        yield return new Item("Shortsword", 10, 5, 0);
        yield return new Item("Warhammer", 25, 6, 0);
        yield return new Item("Longsword", 40, 7, 0);
        yield return new Item("Greataxe", 74, 8, 0);
    }

    private IEnumerable<Item> GetArmors()
    {
        yield return new Item("Leather", 13, 0, 1);
        yield return new Item("Chainmail", 31, 0, 2);
        yield return new Item("Splintmail", 53, 0, 3);
        yield return new Item("Bandedmail", 75, 0, 4);
        yield return new Item("Platemail", 102, 0, 5);
    }
    
    private IEnumerable<Item> GetRings()
    {
        yield return new Item("Damage +1", 25, 1, 0);
        yield return new Item("Damage +2", 50, 2, 0);
        yield return new Item("Damage +3", 100, 3, 0);
        yield return new Item("Defense +1", 20, 0, 1);
        yield return new Item("Defense +2", 40, 0, 2);
        yield return new Item("Defense +3", 80, 0, 3);
    }
}