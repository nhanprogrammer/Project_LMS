using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class AnswersService : IAnswersService
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly IMapper _mapper;

        public AnswersService(IAnswerRepository answerRepository, IMapper mapper)
        {
            _answerRepository = answerRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AnswerResponse>> GetAllAnswers()
        {
            var answers = await _answerRepository.GetAllAsync();
            return _mapper.Map<List<AnswerResponse>>(answers);
        }

        public async Task<AnswerResponse?> GetAnswerById(int id)
        {
            var answer = await _answerRepository.GetByIdAsync(id);
            return _mapper.Map<AnswerResponse>(answer);
        }

        public async Task AddAnswer(CreateAnswerRequest request)
        {
            var answer = _mapper.Map<Answer>(request);
            await _answerRepository.AddAsync(answer);
        }

        public async Task UpdateAnswer(int id, UpdateAnswerRequest request)
        {
            var existingAnswer = await _answerRepository.GetByIdAsync(id);
            if (existingAnswer == null)
            {
                throw new Exception("Answer not found");
            }

            _mapper.Map(request, existingAnswer);
            await _answerRepository.UpdateAsync(existingAnswer);
        }

        public async Task<bool> DeleteAnswer(int id)
        {
            var existingAnswer = await _answerRepository.GetByIdAsync(id);
            if (existingAnswer == null)
            {
                return false;
            }

            await _answerRepository.DeleteAsync(id);
            return true;
        }
    }
}