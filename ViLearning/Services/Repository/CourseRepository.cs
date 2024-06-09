﻿using ViLearning.Data;
using ViLearning.Models;
using ViLearning.Services.Repository.IRepository;

namespace ViLearning.Services.Repository
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        private ApplicationDBContext _db;
        public CourseRepository(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Course course)
        {
            _db.Courses.Update(course);
        }
        public void LoadCourse(Course course)
        {
            _db.Courses.Entry(course)
                    .Collection(c => c.Lesson)
                    .Load();
            _db.Update(course);
        }
    }
}
