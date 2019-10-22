namespace RevolutAPI.Models.JWT
{
    internal class JwtHeader
    {
        public string alg => JwtHashAlgorithm.RS256.ToString();
        public string typ => "JWT";
    }
}