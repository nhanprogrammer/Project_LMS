// using Project_LMS.Interfaces.Services;
// using Project_LMS.Interfaces.Repositories;
// using Project_LMS.DTOs.Request;
// using Project_LMS.DTOs.Response;
// using Project_LMS.Models;
// using Project_LMS.Exceptions;

// namespace Project_LMS.Services
// {
//     public class QuestionService : IQuestionsService
//     {
//         private readonly IQuestionRepository _questionRepository;

//         public QuestionService(IQuestionRepository questionRepository)
//         {
//             _questionRepository = questionRepository;
//         }

//         public async Task<IEnumerable<QuestionResponse>> GetAllAsync()
//         {
//             var questions = await _questionRepository.GetAllAsync();
//             return questions.Select(question => new QuestionResponse
//             {
//                 Id = question.Id,
//                 TestExamId = question.TestExamId,
//                 Question1 = question.Question1,
//                 Mark = question.Mark,
//                 IsDelete = question.IsDelete,
//                 CreateAt = question.CreateAt,
//                 UpdateAt = question.UpdateAt
//             });
//         }

//         public async Task<QuestionResponse> GetByIdAsync(int id)
//         {
//             var question = await _questionRepository.GetByIdAsync(id);
//             if (question == null)
//             {
//                 return null;
//             }
//             return new QuestionResponse
//             {
//                 Id = question.Id,
//                 TestExamId = question.TestExamId,
//                 Question1 = question.Question1,
//                 Mark = question.Mark,
//                 IsDelete = question.IsDelete,
//                 CreateAt = question.CreateAt,
//                 UpdateAt = question.UpdateAt
//             };
//         }

//         public async Task<QuestionResponse> CreateAsync(QuestionRequest request)
//         {

//             if (request.TestExamId == null || request.Mark == null)
//             {
//                 throw new ArgumentNullException("TestExamId and Mark cannot be null.");
//             }

//             var question = new Question
//             {
//                 TestExamId = request.TestExamId.Value,
//                 Question1 = request.Question1,
//                 Mark = request.Mark.Value,
//                 IsDelete = false,
//             };

//             await _questionRepository.AddAsync(question);

//             return new QuestionResponse
//             {
//                 Id = question.Id,
//                 TestExamId = question.TestExamId,
//                 Question1 = question.Question1,
//                 Mark = question.Mark,
//                 IsDelete = question.IsDelete,
//                 CreateAt = question.CreateAt,
//                 UpdateAt = question.UpdateAt
//             };
//         }

//         public async Task<QuestionResponse> UpdateAsync(int id, QuestionRequest request)
//         {

//             var question = await _questionRepository.GetByIdAsync(id);
//             if (question == null)
//             {
//                 throw new NotFoundException("Bản ghi không tồn tại.");
//             }


//             if (request.TestExamId == null || request.Mark == null)
//             {
//                 throw new ArgumentNullException("TestExamId và Mark không được phép là null.");
//             }


//             question.TestExamId = request.TestExamId.Value;
//             question.Question1 = request.Question1;
//             question.Mark = request.Mark.Value;


//             await _questionRepository.UpdateAsync(question);


//             return new QuestionResponse
//             {
//                 Id = question.Id,
//                 TestExamId = question.TestExamId,
//                 Question1 = question.Question1,
//                 Mark = question.Mark,
//                 IsDelete = question.IsDelete,
//                 CreateAt = question.CreateAt,
//                 UpdateAt = question.UpdateAt
//             };
//         }

//         public async Task<QuestionResponse> DeleteAsync(int id)
//         {
//             var question = await _questionRepository.GetByIdAsync(id);
//             if (question == null)
//             {
//                 return null;
//             }
//             question.IsDelete = true;

//             await _questionRepository.UpdateAsync(question);
//             return new QuestionResponse
//             {
//                 Id = question.Id,
//                 TestExamId = question.TestExamId,
//                 Question1 = question.Question1,
//                 Mark = question.Mark,
//                 IsDelete = question.IsDelete,
//                 CreateAt = question.CreateAt,
//                 UpdateAt = question.UpdateAt
//             };
//         }
//     }
// }