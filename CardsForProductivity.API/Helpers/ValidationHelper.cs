using MongoDB.Bson;

namespace CardsForProductivity.API.Helpers
{
    /// <summary>
    /// Validation helper.
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Validates an ObjectId.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>True if valid, else false.</returns>
        public static bool ValidateObjectId(string input)
        {
            return ObjectId.TryParse(input, out _);
        }
    }
}