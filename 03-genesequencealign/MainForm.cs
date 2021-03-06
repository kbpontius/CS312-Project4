using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace GeneticsLab
{
    public partial class MainForm : Form
    {
        ResultTable m_resultTable;
        GeneSequence[] m_sequences;
        const int NUMBER_OF_SEQUENCES = 10;
        const string GENOME_FILE = "genomes.txt";

        public MainForm()
        {
            InitializeComponent();

            statusMessage.Text = "Loading Database...";

            // load database here

            try
            {
                m_sequences = loadFile("../../" + GENOME_FILE);
            }
            catch (FileNotFoundException e)
            {
                try // Failed, try one level down...
                {
                    m_sequences = loadFile("../" + GENOME_FILE);
                }
                catch (FileNotFoundException e2)
                {
                    // Failed, try same folder
                    m_sequences = loadFile(GENOME_FILE);
                }
            }

            m_resultTable = new ResultTable(this.dataGridViewResults, NUMBER_OF_SEQUENCES);

            statusMessage.Text = "Loaded Database.";

        }

        private GeneSequence[] loadFile(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            string input = "";

            try
            {
                input = reader.ReadToEnd();
            }
            catch
            {
                Console.WriteLine("Error Parsing File...");
                return null;
            }
            finally
            {
                reader.Close();
            }

            GeneSequence[] temp = new GeneSequence[NUMBER_OF_SEQUENCES];
            string[] inputLines = input.Split('\r');

            for (int i = 0; i < NUMBER_OF_SEQUENCES; i++)
            {
                string[] line = inputLines[i].Replace("\n","").Split('#');
                temp[i] = new GeneSequence(line[0], line[1]);
            }
            return temp;
        }

        private void fillMatrix()
        {
            PairWiseAlign processor = new PairWiseAlign();
            for (int x = 0; x < NUMBER_OF_SEQUENCES; ++x)
            {
                for (int y = 0; y < NUMBER_OF_SEQUENCES; ++y)
                {
                    m_resultTable.SetCell(x, y, processor.Align(m_sequences[x], m_sequences[y], m_resultTable, x, y));
                }
            }
        }

        private void processButton_Click(object sender, EventArgs e)
        {
            statusMessage.Text = "Processing...";
            Stopwatch timer = new Stopwatch();
            timer.Start();
            fillMatrix();
            timer.Stop();
            statusMessage.Text = "Done.  Time taken: " + timer.Elapsed;
        }

        private void dataGridViewResults_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            statusMessage.Text = "Calculating Alignment...";
            txtAlignment.Text = "";
            this.Refresh();

            Grid extractionGrid = new Grid(m_sequences[e.RowIndex].Sequence, m_sequences[e.ColumnIndex].Sequence, false, 5000);
            int score = extractionGrid.CalculateExtractionSolution();
            Tuple<string, string> sequences = extractionGrid.ExtractPath();

            string trimmedNewTopSequence = TrimAlignment(sequences.Item1);
            string trimmedNewLeftSequence = TrimAlignment(sequences.Item2);

            StringBuilder builder = new StringBuilder();
            builder.Append(FormatSequence(trimmedNewTopSequence));
            builder.Append(FormatSequence(trimmedNewLeftSequence));
            txtAlignment.Text = builder.ToString();

            statusMessage.Text = "Done.";
        }

        private string FormatSequence(string stringToFormat)
        {
            StringBuilder formattedStrings = new StringBuilder();

            foreach(char letter in stringToFormat)
            {
                formattedStrings.Append(Char.ToUpper(letter));
            }

            formattedStrings.Append("\r\n");
            return formattedStrings.ToString();
        }

        private string TrimAlignment(string stringToTrim)
        {
            if(stringToTrim.Length <= 100)
            {
                return stringToTrim;
            }

            return stringToTrim.Substring(0, 100);
        }
    }
}