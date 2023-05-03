﻿namespace Digichlist.Bot.Enums
{
    /// <summary>
    /// The status that represents a defect's state.
    /// </summary>
    public enum DefectStatus
    {
        /// <summary>
        /// Technical status. Covers the cases when status is missing.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// The defect is published.
        /// </summary>
        Opened = 1,

        /// <summary>
        /// The defect is being fixed.
        /// </summary>
        Fixing = 2,

        /// <summary>
        /// The defect is eliminated and no longer exists.
        /// </summary>
        Eliminated = 3,
    }
}
