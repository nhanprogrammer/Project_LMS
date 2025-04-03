using System.IO;
using System.Net.Http;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Project_LMS.Data;
using Project_LMS.Models;

namespace Project_LMS.Config
{
    public class FileProcessor
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public FileProcessor(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<Question>> ReadQuestionsFromFileAsync(string fileUrl)
        {
            var questions = new List<Question>();

            using (var client = _httpClientFactory.CreateClient())
            {
                var response = await client.GetAsync(fileUrl);
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    using (WordprocessingDocument doc = WordprocessingDocument.Open(stream, false))
                    {
                        Body body = doc.MainDocumentPart.Document.Body;
                        Console.WriteLine("Đã mở file DOCX thành công.");

                        Question currentQuestion = null;
                        List<string> options = new List<string>();
                        bool isMultipleChoice = false;

                        foreach (Paragraph paragraph in body.Elements<Paragraph>())
                        {
                            string text = paragraph.InnerText.Trim();
                            Console.WriteLine($"Đọc dòng: {text}");

                            text = Regex.Replace(text, @"\s+", " "); // Chuẩn hóa khoảng trắng

                            // Phát hiện câu hỏi mới (câu trắc nghiệm hoặc tự luận)
                            if (text.StartsWith("Câu hỏi") || text.StartsWith("Question"))
                            {
                                // Nếu có câu hỏi trước đó, lưu lại trước khi tạo câu hỏi mới
                                if (currentQuestion != null)
                                {
                                    currentQuestion.QuestionType = isMultipleChoice ? "trắc nghiệm" : "tự luận";

                                    // Nếu là trắc nghiệm, thêm các câu trả lời vào
                                    if (isMultipleChoice)
                                    {
                                        foreach (var option in options)
                                        {
                                            bool isCorrect = option.Contains("(correct)");
                                            currentQuestion.Answers.Add(new Answer
                                            {
                                                Answer1 = option.Replace("(correct)", "").Trim(),
                                                IsCorrect = isCorrect,
                                                CreateAt = DateTime.Now,
                                                UpdateAt = DateTime.Now
                                            });
                                        }
                                    }

                                    questions.Add(currentQuestion); // Lưu câu hỏi vào danh sách
                                }

                                // Tạo câu hỏi mới
                                currentQuestion = new Question
                                {
                                    QuestionText = text,
                                    Answers = new HashSet<Answer>()
                                };

                                // Reset lại trạng thái cho câu hỏi mới
                                options = new List<string>();
                                isMultipleChoice = false;
                            }
                            else if (Regex.IsMatch(text, @"^[A-Da-d]\s*\.\s*")) // Nếu là đáp án của câu hỏi trắc nghiệm
                            {
                                isMultipleChoice = true; // Đánh dấu đây là câu hỏi trắc nghiệm
                                options.Add(text); // Thêm đáp án vào danh sách
                            }
                        }

// Lưu câu hỏi cuối cùng nếu có
                        if (currentQuestion != null)
                        {
                            currentQuestion.QuestionType = isMultipleChoice ? "trắc nghiệm" : "tự luận";

                            // Nếu là trắc nghiệm, thêm các câu trả lời vào
                            if (isMultipleChoice)
                            {
                                foreach (var option in options)
                                {
                                    bool isCorrect = option.Contains("(correct)");
                                    currentQuestion.Answers.Add(new Answer
                                    {
                                        Answer1 = option.Replace("(correct)", "").Trim(),
                                        IsCorrect = isCorrect,
                                        CreateAt = DateTime.Now,
                                        UpdateAt = DateTime.Now
                                    });
                                }
                            }

                            questions.Add(currentQuestion);
                        }

                        Console.WriteLine($"Tổng số câu hỏi đọc được: {questions.Count}");


                        // Lưu câu hỏi cuối cùng
                        if (currentQuestion != null)
                        {
                            currentQuestion.QuestionType = isMultipleChoice ? "trắc nghiệm" : "tự luận";

                            // Chuyển options thành danh sách Answers
                            foreach (var option in options)
                            {
                                bool isCorrect = option.Contains("(correct)");
                                currentQuestion.Answers.Add(new Answer
                                {
                                    Answer1 = option.Replace("(correct)", "").Trim(),
                                    IsCorrect = isCorrect,
                                    CreateAt = DateTime.Now,
                                    UpdateAt = DateTime.Now
                                });
                            }

                            questions.Add(currentQuestion);
                        }
                    }
                }
            }
            
            int totalMultipleChoice = questions.Count(q => q.QuestionType == "trắc nghiệm"); // Đếm số câu trắc nghiệm

            if (totalMultipleChoice > 0)
            {
                double markPerQuestion = 10.0 / totalMultipleChoice; // Chia đều điểm cho câu trắc nghiệm
                foreach (var question in questions.Where(q => q.QuestionType == "trắc nghiệm"))
                {
                    question.Mark = markPerQuestion; // Chỉ cập nhật điểm cho câu trắc nghiệm
                }
            }



            Console.WriteLine($"Tổng số câu hỏi đọc được: {questions.Count}");
            return questions;
        }
    }
}