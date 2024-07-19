namespace ViLearning.Models.ViewModels
{
    public class CertificateVM
    {
        public List<LearningProgress> LearningProgresses {  get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
    }
}
