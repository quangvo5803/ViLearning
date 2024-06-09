using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ViLearning.Data;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;

namespace ViLearning.Services.Repository
{
    public class QuestionRepository : Repository<Question>, IQuestionRepository
    {
        private ApplicationDBContext _db;
        public QuestionRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public async Task<List<Question>> GetQuestionByLessonId(int lessonId)
        {
            try
            {
                return await _db.Questions.Where(x => x.LessonId == lessonId).OrderBy(x => x.Difficulty).ThenBy(x => x.LessonId).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public void Update(Question question)
        {
            _db.Questions.Update(question);
        }

        



    }
}
