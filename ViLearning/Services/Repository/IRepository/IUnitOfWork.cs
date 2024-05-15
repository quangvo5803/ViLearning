namespace ViLearning.Services.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ISubjectRepository Subject { get; }
        void Save();
    }
}
