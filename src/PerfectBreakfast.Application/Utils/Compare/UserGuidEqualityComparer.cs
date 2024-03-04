using PerfectBreakfast.Domain.Entities;

namespace PerfectBreakfast.Application.Utils.Compare;

public class UserGuidEqualityComparer : IEqualityComparer<User>
{
    public bool Equals(User x, User y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            return false;

        return x.Id.Equals(y.Id);
    }

    public int GetHashCode(User obj)
    {
        if (ReferenceEquals(obj, null)) return 0;
        return obj.Id.GetHashCode();
    }
}