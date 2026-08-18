using Newtonsoft.Json;

namespace Asana.Library.Util
{
    /// <summary>
    /// JSON helpers for API responses.
    /// </summary>
    public static class Json
    {
        /// <summary>
        /// Deserialize a response body that may be null.
        ///
        /// <see cref="WebRequestHandler"/> returns null when the API could not
        /// be reached or answered with an error status. Passing that straight
        /// to Newtonsoft throws ArgumentNullException, which surfaced as an
        /// unhandled crash instead of a handled "API is down" path.
        /// </summary>
        public static T? FromResponse<T>(string? json) =>
            string.IsNullOrWhiteSpace(json)
                ? default
                : JsonConvert.DeserializeObject<T>(json);
    }
}
