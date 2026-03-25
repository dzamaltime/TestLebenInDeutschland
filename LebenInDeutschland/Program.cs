using Microsoft.Data.Sqlite;
namespace LebenInDeutschland
{
    internal class Program
    {
        static SqliteConnection connection;

        static int maxId;//global variable
        static void Main(string[] args)
        {

            connection = new SqliteConnection("Data Source=test.db");
            connection.Open();

            maxId = GetMaxId();

            Start();

        }


        //READING FROM DATABASE
        static void ReadQuestion(int id)
        {

            var command = connection.CreateCommand();
            command.CommandText = "SELECT text FROM questions WHERE id = " + id;

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"{id}: {reader["text"]}");
            }
            reader.Close();
        }

        static void ReadAnswers(int id)
        {

            var command = connection.CreateCommand();
            command.CommandText = "SELECT text FROM answers WHERE questionid = " + id;

            var reader = command.ExecuteReader();
            int num = 1;
            while (reader.Read())
            {
                Console.WriteLine($"{num} - {reader["text"]}");
                num++;
            }
            reader.Close();

        }

        static int ReadCorrectAnswerId(int id)
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT correct_answer_id FROM questions WHERE id = " + id;

            var reader = command.ExecuteReader();

            int correctAnswerId = 0;

            while (reader.Read())
            {
                correctAnswerId = Convert.ToInt32(reader["correct_answer_id"]);
            }
            reader.Close();
            return correctAnswerId;
        }


        //GAME LOGIC
        static int GetInput(int a)
        {
            string input = Console.ReadLine();

            try
            {
                int value = Convert.ToInt32(input);

                if (value < 1 || value > a)
                    throw new Exception();
                return value;
            }
            catch (FormatException)
            {
                Console.WriteLine("error: wrong format, only numbers are allowed");
                return GetInput(a);
            }
            catch (OverflowException)
            {
                Console.WriteLine("error: wrong number");
                return GetInput(a);
            }
            catch
            {
                Console.WriteLine("error");
                return GetInput(a);
            }

        }

        static int AskQuestion(int globalId)
        {
            ReadQuestion(globalId);

            ReadAnswers(globalId);

            if (GetInput(4) == ReadCorrectAnswerId(globalId))
            {
                Console.WriteLine("true");
                return 1;
            }
            else
            {
                Console.WriteLine("false");
                return 0;
            }
        }





        static void Start()
        {
            Console.WriteLine("Choose mode");
            Console.WriteLine("1 - single question");
            Console.WriteLine("2 - random question");
            Console.WriteLine("3 - Test");
            int mode = GetInput(3);

            switch (mode)
            {
                case 1: SingleQuestionMode(); break;
                case 2: RandomQuestionMode(); break;
                case 3: TestMode(); break;
            }

            Console.WriteLine("Play again?");
            Console.WriteLine("1 - YES");
            Console.WriteLine("2 - NO");
            if (GetInput(2) == 1)
                Start();
        }


        //GAME MODES
        static void SingleQuestionMode()
        {
            Console.WriteLine("Choose question, from 1 to " + maxId);
            AskQuestion(GetInput(maxId));
        }

        static void RandomQuestionMode()
        {
            Random rnd = new Random();
            AskQuestion(rnd.Next(1, maxId + 1));
        }

        static void TestMode()
        {
            Console.WriteLine("TEST");
            Random rnd = new Random();
            List<int> questionsList = new List<int>();
            while (questionsList.Count < 5)
            {
                int q = rnd.Next(1, maxId + 1);
                if (!questionsList.Contains(q))
                    questionsList.Add(q);
            }

            int points = 0;
            for (int i = 0; i < 5; i++)
            {
                points = points + AskQuestion(questionsList[i]);

            }
            Console.WriteLine(points + "/5");
        }




        //GLOBAL VARIABLE
        static int GetMaxId()
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT MAX(id) FROM questions";

            var reader = command.ExecuteReader();

            int maxId = 0;
            while (reader.Read())
            {
                maxId = Convert.ToInt32(reader["MAX(id)"]);
            }
            reader.Close();

            return maxId;
        }
    }
}

