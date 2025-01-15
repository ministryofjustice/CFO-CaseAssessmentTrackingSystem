using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cfo.Cats.Server.UI.Pages.Payments.Components;

public static class Extensions
{
   public static decimal CalculateCappedPercentage(this int score, int target)
   {
        // Prevent division by zero
        if (target == 0)
        {
            return 0;
        }

        // Calculate the percentage
        decimal percentage = ((decimal)score / target) * 100;

        // Cap the result at 100%
        return Math.Min(Math.Round(percentage), 100);
   } 
}