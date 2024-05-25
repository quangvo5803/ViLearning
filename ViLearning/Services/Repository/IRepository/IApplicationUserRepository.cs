using System.Linq.Expressions;
using ViLearning.Models;

namespace ViLearning.Services.Repository.IRepository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        void Update(ApplicationUser applicationUser);
    }
}
