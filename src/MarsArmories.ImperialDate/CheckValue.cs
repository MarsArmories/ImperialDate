namespace MarsArmories.ImperialDate
{
    public enum CheckValue
    {
        // Location Based
        //A 0 means that the event occurred on Terra.
        Terra = 0,
        //A 1 means that the event occurred within the Sol system.
        Sol = 1,
        // Contact Based
        //A 2 means that the event occurred while someone present for the event was in direct psychic contact with Terra or the Sol system.
        Secondary = 2,
        //A 3 means that an individual or organization present was in psychic contact with a 2 source while the event occurred.
        Tertiary = 3,
        //A 4 means that the individual or organization was in contact with a 3 OR 2 source.
        Four = 4,
        //A 5 means that the individual or organization was in contact with a 4 source.
        Five = 5,
        //A 6 means that the individual or organization was in contact with a 5 source.
        Six = 6,
        // Estimation Based
        //A 7 means that the event in question occurred within 10 years of the date listed in the rest of the Imperial date.
        Approximated10 = 7,
        //An 8 means that the event occurred within 20 years of the date.
        Approximated20 = 8,
        //A 9 class source is special. A 9-class source is an approximated date, and is usually used when recording a date within Warp travel or while on a planet that does not use the Imperial system.
        Warp = 9
    }
}
