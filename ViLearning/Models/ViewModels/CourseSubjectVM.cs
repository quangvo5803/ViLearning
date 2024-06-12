namespace ViLearning.Models.ViewModels
{
	public class CourseSubjectVM
	{
		public IEnumerable<Course> Course { get; set; }
		public IEnumerable<Subject> Subject {  get; set; }
		public string SelectedSubjectName { get; set; }
		public double? MinPrice { get; set; }
		public double? MaxPrice { get; set; }
		public string SortOrder { get; set; }
		public int? Grade { get; set; }
	}
}
