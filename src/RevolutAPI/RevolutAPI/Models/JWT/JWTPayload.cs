using System;

namespace RevolutAPI.Models.JWT
{
    internal class JwtPayload
    {
        public string iss { get; set; }

        public string sub { get; set; }

        /// <summary>
        ///     Always "https://revolut.com" see documentation
        /// </summary>
        public string aud => "https://revolut.com";

        public long iat => new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero).ToUnixTimeSeconds();

        public long exp
        {
            get
            {
                var dto = new DateTimeOffset(DateTime.UtcNow, TimeSpan.Zero);
                return dto.ToUnixTimeSeconds() + 60 * 60;
            }
        }
    }
}