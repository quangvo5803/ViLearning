using ViLearning.Data;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;
using ViLearning.Utility;

namespace ViLearning.Services.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public ISubjectRepository Subject { get; private set; }
        public IApplicationUserRepository ApplicationUser{ get; private set; }
        public ICommentRepository Comment { get; private set; }
        public IContentRepository Content { get; private set; }
        public ILessonRepository Lesson { get; private set; }
        public ITestDetailRepository TestDetail { get; private set; }
        public IQuestionRepository Question { get; private set; }
        public ICourseRepository Course { get; private set; }
        public IFeedbackRepository Feedback { get; private set; }
        public ILearningProgressRepository LearningProgress { get; private set; }
        public IInvoiceRepository Invoice { get; private set; }
        public IConversationRepository Conversation { get; private set; }
        public IMessageRepository Message { get; private set; }

        public IWithdrawRequestRepositoy WithdrawRequest {  get; private set; }


        private ApplicationDBContext _db;

        public UnitOfWork(ApplicationDBContext db)
        {
            _db = db;
            Subject = new SubjectRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            Comment = new CommentRepository(_db);
            Content = new ContentRepository(_db);
            Lesson = new LessonRepository(_db);
            TestDetail = new TestDetailRepository(_db);
            Course = new CourseRepository(_db);
            Feedback = new FeedbackRepository(_db);
            Question = new QuestionRepository(_db);
            Invoice = new InvoiceRepository(_db);
            Conversation = new ConversationRepository(_db);
            Message = new MessageRepository(_db);
            LearningProgress = new LearningProgressRepository(_db);
            WithdrawRequest = new WithdrawRequestRepositoy(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
