namespace Digichlist.Bot.Models
{
    /// <summary>
    /// The model that contains info when a user selects a defect to change its status.
    /// </summary>
    public class DefectStatusCallback
    {
        /// <summary>
        /// The command this callback originates from.
        /// </summary>
        public string Command { get; private set; }

        /// <summary>
        /// The realted defect identifier.
        /// </summary>
        public int DefectId { get; set; }

        ///// <summary>
        ///// The desired status for the specified defect.
        ///// Null means that a user hasn't yet a chance to select the desired status.
        ///// </summary>
        public DefectStatus? Status { get; set; }

        ///// <summary>
        ///// Fills up all callback's info.
        ///// </summary>
        public static DefectStatusCallback ToModel(int defectId, string command)
        {
            return new DefectStatusCallback
            {
                Command = command,
                DefectId = defectId,
            };
        }
    }
}
