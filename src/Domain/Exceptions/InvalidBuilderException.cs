using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cfo.Cats.Domain.Exceptions;

internal class InvalidBuilderException(string missingField) : ApplicationException($"{missingField} has not been set")
{
}