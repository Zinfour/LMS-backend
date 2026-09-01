using Microsoft.EntityFrameworkCore;

namespace LMS.API.Data
{
    public class LmsContext(DbContextOptions<LmsContext> options) : DbContext(options)
    {

    }
}
