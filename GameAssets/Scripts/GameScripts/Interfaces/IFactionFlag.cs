public interface IFactionFlag
{
    FactionFlags FactionFlags { get; set; }
}

[System.Flags]
public enum FactionFlags
{
    None = 0x0,
    one,
    two,
    three,
    four
}