namespace aoc.common.Maps;

[AttributeUsage(AttributeTargets.Field)]
public class EnumMapCharacterAttribute : Attribute
{
    public char Character { get; }

    public EnumMapCharacterAttribute(char character)
    {
        Character = character;
    }
}