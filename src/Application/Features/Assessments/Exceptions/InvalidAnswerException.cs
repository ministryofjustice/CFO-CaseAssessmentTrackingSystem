namespace Cfo.Cats.Application.Features.Assessments.Exceptions;

public class InvalidAnswerException(string question, string answer) 
    : Exception($"The answer {answer} is not valid for the question {question}");
