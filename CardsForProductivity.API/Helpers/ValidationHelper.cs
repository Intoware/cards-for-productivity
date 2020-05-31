using MongoDB.Bson;

namespace CardsForProductivity.API.Helpers
{
    public static class ValidationHelper
    {
        public static bool ValidateObjectId(string input)
        {
            return ObjectId.TryParse(input, out _);
        }
    }
}