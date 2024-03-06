using MongoDB.Bson;

namespace PerfectBreakfast.Application.Utils;

public static class RandomCode
{
    public static long GenerateOrderCode()  
    {
        var objectId = ObjectId.GenerateNewId();
        return Math.Abs(objectId.GetHashCode()); 
    }
}