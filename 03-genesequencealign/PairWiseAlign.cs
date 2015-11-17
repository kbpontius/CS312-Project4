using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticsLab
{
    class PairWiseAlign
    {
        
        /// <summary>
        /// Align only 5000 characters in each sequence.
        /// </summary>
        private int MaxCharactersToAlign = 5000;

        /// <summary>
        /// This is the function you implement.
        /// </summary>
        /// <param name="sequenceA">the first sequence</param>
        /// <param name="sequenceB">the second sequence, may have length not equal to the length of the first seq.</param>
        /// <param name="resultTableSoFar">the table of alignment results that has been generated so far using pair-wise alignment</param>
        /// <param name="rowInTable">this particular alignment problem will occupy a cell in this row the result table.</param>
        /// <param name="columnInTable">this particular alignment will occupy a cell in this column of the result table.</param>
        /// <returns>the alignment score for sequenceA and sequenceB.  The calling function places the result in entry rowInTable,columnInTable
        /// of the ResultTable</returns>
        public int Align(GeneSequence sequenceA, GeneSequence sequenceB, ResultTable resultTableSoFar, int rowInTable, int columnInTable)
        {
            if((rowInTable - columnInTable) >= 0)
            {
                return 0;
            }

            string trimmedA = TrimString(sequenceA.Sequence, MaxCharactersToAlign);
            string trimmedB = TrimString(sequenceB.Sequence, MaxCharactersToAlign);

            Grid grid = new Grid(trimmedA, trimmedB, true);
            return grid.CalculateScoreSolution();
        }

        // HELPER METHOD
        private string TrimString(string stringToTrim, int limit)
        {
            if(stringToTrim.Length <= limit)
            {
                return stringToTrim;
            }

            return stringToTrim.Substring(0, limit);
        }
    }
}
