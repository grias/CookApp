using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookApp.Domain.UtilityClasses;

public class Pagination
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }

    public Pagination(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public static Pagination All()
    {
        return new Pagination(1, int.MaxValue);
    }
}
