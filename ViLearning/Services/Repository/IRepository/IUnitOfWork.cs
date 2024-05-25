namespace ViLearning.Services.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ISubjectRepository Subject { get; }
        IApplicationUserRepository ApplicationUser{ get; }
        void Save();
    }
}
