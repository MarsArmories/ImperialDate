namespace MarsArmories.ImperialDate;

/// <summary>Classifies the source or uncertainty of an Imperial date.</summary>
/// <remarks>The numeric value is the leading digit in Imperial notation and participates in equality and ordering.</remarks>
public enum CheckValue
{
    /// <summary>The event occurred on Terra.</summary>
    Terra = 0,
    /// <summary>The event occurred within the Sol system.</summary>
    Sol = 1,
    /// <summary>A witness was in direct psychic contact with Terra or the Sol system.</summary>
    Secondary = 2,
    /// <summary>A source present was in psychic contact with a secondary source.</summary>
    Tertiary = 3,
    /// <summary>A source present was in contact with a tertiary or secondary source.</summary>
    Four = 4,
    /// <summary>A source present was in contact with a class-four source.</summary>
    Five = 5,
    /// <summary>A source present was in contact with a class-five source.</summary>
    Six = 6,
    /// <summary>The event is estimated to be within ten years of the recorded date.</summary>
    Approximated10 = 7,
    /// <summary>The event is estimated to be within twenty years of the recorded date.</summary>
    Approximated20 = 8,
    /// <summary>An approximate date, typically during Warp travel or on a world using another calendar.</summary>
    Warp = 9
}
