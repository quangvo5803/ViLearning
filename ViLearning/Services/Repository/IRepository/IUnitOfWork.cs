namespace ViLearning.Services.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ISubjectRepository Subject { get; }
        IApplicationUserRepository ApplicationUser{ get; }
        public ICommentRepository Comment { get;}
        public IContentRepository Content { get;  }
        public ILessonRepository Lesson { get; }
        public ITestDetailRepository TestDetail { get; }
        public IQuestionRepository Question { get; }
        public ICourseRepository Course { get; }
        public ICourseCertificateRepository CourseCertificate { get; }
        public IFeedbackRepository Feedback { get; }
        public IStudentCertificateRepository StudentCertificate { get; }
        public IInvoiceRepository Invoice { get; }
        public IMessageRepository Message { get; }
        public IConversationRepository Conversation { get; }
        void Save();
    }
}
