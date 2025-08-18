using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cfo.Cats.Application.Features.Bios.Exceptions;
public class InvalidAnswerException(string question, string answer)
    : Exception($"The answer {answer} is not valid for the question {question}");

