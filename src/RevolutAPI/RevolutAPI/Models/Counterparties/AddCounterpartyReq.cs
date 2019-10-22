namespace RevolutAPI.Models.Counterparties
{
    public class AddCounterpartyReq
    {
        public string ProfileType { get; set; }

        public string Name { get; set; }

        /// <summary>
        ///     Phone number
        /// </summary>
        /// <remarks>That number must be unique otherwise an exception is thrown when you create a new counterparty.</remarks>
        public string Phone { get; set; }

        public string Email { get; set; }
    }
}