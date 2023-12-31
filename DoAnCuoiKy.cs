using System;
using System.Diagnostics;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Media;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace DOANCSLT
{
    class Game
    {
        static char[,] maze = new char[10, 10] //khởi tạo một mảng hai chiều maze với kích thước là 10x10. Mỗi phần tử của mảng đại diện cho một ô trong mê cung và chứa một ký tự.
        {
            { '╔', '═', '═', '═', '═', '═', '═', '═', '═', '╗' },
            { '║', '.', '.', '!', '.', '#', '.', '.', '.', '║' },
            { '║', '.', '!', '.', '.', '!', '.', '.', '#', '║' },
            { '║', '!', '.', '#', '#', '.', '.', '!', '.', '║' },
            { '║', '.', '.', '!', '.', '!', '#', '.', '#', '║' },
            { '║', '!', '#', '#', '#', '.', '.', '!', '.', '║' },
            { '║', '.', '!', '.', '.', '!', '#', '.', '!', '║' },
            { '║', '#', '.', '#', '!', '.', '.', '!', '.', '║' },
            { '║', '.', '.', '.', '#', '.', '!', '.', '.', '║' },
            { '╚', '═', '═', '═', '═', '═', '═', '═', '═', '╝' }
        };

        //Khai báo vị trí người chơi
        static int playerX = 2;
        static int playerY = 2;
        static int exitX = 8;
        static int exitY = 8;

        static Stopwatch stopwatch = new Stopwatch();//tạo đối tượng Stopwatch
        static Random random = new Random(); //tạo một đối tượng Random để sinh số ngẫu nhiên và gán cho biến random.

        static void DrawMaze()
        {
            for (int i = 0; i < 10; i++) //bắt đầu một vòng lặp for để duyệt qua các hàng trong mê cung.
            {
                for (int j = 0; j < 10; j++)//bắt đầu một vòng lặp for lồng bên trong để duyệt qua các cột trong mê cung.
                    if (i == playerX && j == playerY)
                    {
                        Console.Write('☺'); // Vị trí bắt đầu
                    }
                    else if (i == exitX && j == exitY)
                    {
                        Console.Write('E'); // Vị trí kết thúc
                    }
                    else
                    {
                        Console.Write(maze[i, j]); //in ký tự tại vị trí [i, j] của mê cung lên màn hình mà không xuống dòng.
                    }
                Console.WriteLine();
            }
        }

        static void MovePlayer(ConsoleKey key) //phương thức để di chuyển người chơi trong mê cung dựa trên phím mũi tên được nhấn
        {
            //lưu trữ vị trí mới của người chơi
            int newX = playerX;
            int newY = playerY;

            switch (key) // di chuyển trên bàn phím
            {
                case ConsoleKey.UpArrow:
                    newX = playerX - 1;
                    break;
                case ConsoleKey.DownArrow:
                    newX = playerX + 1;
                    break;
                case ConsoleKey.LeftArrow:
                    newY = playerY - 1;
                    break;
                case ConsoleKey.RightArrow:
                    newY = playerY + 1;
                    break;
            }

            if ((maze[newX, newY] != '#') && (maze[newX, newY] != '║') && (maze[newX, newY] != '═')) //nếu điều kiện đúng thì tiếp tục xử lý
            {
                if (maze[newX, newY] == '!')
                {
                    if (AskQuestion()) //hàm AskQuestion() được gọi để hiển thị câu hỏi cho người chơi và trả về kết quả (true nếu người chơi trả lời đúng và false nếu người chơi trả lời sai).
                    {
                        maze[playerX, playerY] = '.';//ô hiện tại được đánh dấu.
                                                     //cập nhập tọa độ mới
                        playerX = newX;
                        playerY = newY;
                        maze[playerX, playerY] = '☺';//ô mới thành mặt cười để đánh dấu vị trí người chơi
                    }
                    else
                    {
                        maze[newX, newY] = '#'; // Trả lời sai, ký tự '!' trở thành '#'
                    }
                }
                else
                {
                    //ô mới không phải ! thì di chuyển như bình thường
                    maze[playerX, playerY] = '.';
                    playerX = newX;
                    playerY = newY;
                    maze[playerX, playerY] = '☺';
                }
            }
        }
        static List<int> usedQuestions = new List<int>(); // Danh sách các câu hỏi đã sử dụng
        static bool AskQuestion()
        {
            Console.OutputEncoding = Encoding.UTF8;
            string[,] questions = new string[,]
            {
        { "Bỏ ngoài nướng trong, ăn ngoài bỏ trong là gì?", "nướng ngô", "nướng trứng", "nướng khoai" },
        { "Con gì mang được miếng gỗ lớn nhưng không mang được hòn sỏi?", "con sông", "con đường", "con đèo" },
        { "Cái gì ở giữa bầu trời và trái đất", "và", "mây", "gió" },
        { "Thủ đô của Anh là?", "London", "Paris", "Berlin" },
        { "Mồm bò không phải mồm bò mà lại là mồm bò là gì?", "ốc sên", "con bò", "con ngựa" },
        { "Thủ đô của Việt Nam là?", "Hà Nội","Hồ Chí Minh",  "Đà Nẵng" },
        { "Tỉnh thành nào nằm ở cực Bắc của Việt Nam?", "Hà Giang", "Sơn La", "Lạng Sơn" },
        { "Tỉnh thành nào là điểm đất liền xa nhất về phía Đông của Việt Nam?", "Khánh Hòa", "Phú Yên", "Bình Định" },
        { "Đâu là tỉnh thành không giáp biển?", "Lào Cai", "Quảng Bình", "Ninh Bình" },
        { "Ai là người đầu tiên đặt chân lên Mặt Trăng?", "Neil Armstrong", "Buzz Aldrin", "Yuri Gagarin" },
        { "Đâu là tên một loại chợ?","Cóc","Ếch", "Nhái"},
        { "Đâu là tên một bãi biển ở Quảng Bình?","Đá Nhảy","Đá Lăn","Đá Chạy"},
        { "Tháng âm lịch nào còn được gọi là Tháng cô hồn ?","Tháng bảy","Tháng tám","Tháng chín"},
        { "Bảo tàng Hồ Chí Minh ở thủ đô Hà Nội được thiết kế theo hình dáng loại hoa nào?","Hoa sen","Hoa hồng","Hoa đào"},
        { "Cái gì có thể chạy mà không di chuyển?","Đồng hồ","Máy bay","Giày"},
        { "Tỉnh thành nào nổi tiếng với vịnh Hạ Long?", "Quảng Ninh", "Bắc Ninh", "Hải Phòng" },
        { "Bức tranh nổi tiếng 'Mona Lisa' được vẽ bởi họa sĩ nào?", "Leonardo da Vinci", "Vincent van Gogh", "Pablo Picasso"},
        { "Tổng thống nước Mỹ thứ 45 là ai?", "Joe Biden", "Barack Obama", "Donald Trump"},
        { "Tên của công ty công nghệ lớn nhất ở Hàn Quốc là gì?", "Samsung Electronics", "LG Electronics", "SK Hynix"},
        { "Thủ đô của Hà Lan là gì?", "Amsterdam", "Moscow", "Rome"},
        { "Tác giả của quyển sách 'The Alchemist' (Nhà giả kim) là ai?", "Paulo Coelho", "Mario Puzo", "Mark Twain"},
        { "Nhà văn Victor Hugo là người nước nào?", "Pháp", "Ý", "Tây Ban Nha"},
        { "James Naismith đã phát minh ra trò chơi thể thao nào vào năm 1891?", "Bóng rổ", "Golf", "Quần vợt"},
        { "Nền văn hóa phục hưng bắt nguồn từ nước nào?", "Ý", "Thụy Sỹ", "Anh" },
        { "Chu kì của vật dao động điều hòa là?", "Thời gian để vật thực hiện được một dao động toàn phần.", "Thời gian ngắn nhất để vật đi từ biên này đến biên kia.", "Thời gian ngắn nhất để vật đi từ vị trí cân bằng ra biên." }
            };

            //Nếu số câu hỏi đã sử dụng bằng với số hàng của mảng questions, tức là đã sử dụng hết các câu hỏi trong danh sách, thì danh sách usedQuestions được xóa bỏ để chuẩn bị sử dụng lại các câu hỏi.
            if (usedQuestions.Count == questions.GetLength(0)) //nếu câu hỏi đã sử dụng bằng chiều dài mảng câu hỏi
            {
                usedQuestions.Clear(); // Xóa danh sách các câu hỏi đã sử dụng
            }

            int index;
            // Bắt đầu một vòng lặp "do-while". Trong vòng lặp này, một số ngẫu nhiên được tạo ra và gán cho biến index. Vòng lặp này sẽ tiếp tục chạy cho đến khi giá trị index không nằm trong danh sách usedQuestions.
            do
            {
                index = random.Next(questions.GetLength(0));
            } while (usedQuestions.Contains(index));
            usedQuestions.Add(index); // Thêm câu hỏi vào danh sách đã sử dụng

            //theo thứ tự câu hỏi và câu trả lời
            string question = questions[index, 0]; // Lấy câu hỏi từ mảng hai chiều questions tại hàng index và cột đầu tiên (0)
            string correctAnswer = questions[index, 1];
            string wrongAnswer1 = questions[index, 2];
            string wrongAnswer2 = questions[index, 3];

            // Hoán đổi vị trí các câu trả lời ngẫu nhiên
            string[] answerOptions = new string[] { correctAnswer, wrongAnswer1, wrongAnswer2 };//Mảng answerOptions chứa tất cả các câu trả lời (bao gồm cả câu trả lời đúng và sai)
            ShuffleArray(answerOptions);// hoán đổi ngẫu nhiên các phần tử trong mảng answerOptions

            //in ra câu hỏi và các phương án trả lời đã được hoán đổi vị trí
            Console.WriteLine(question);
            Console.WriteLine("A. " + answerOptions[0]);
            Console.WriteLine("B. " + answerOptions[1]);
            Console.WriteLine("C. " + answerOptions[2]);

            Console.Write("Hãy nhập đáp án (A, B, hoặc C): ");
            string userInput = Console.ReadLine().ToUpper();

            string userAnswer;
            if (userInput == "A")
            {
                userAnswer = answerOptions[0];
            }
            else if (userInput == "B")
            {
                userAnswer = answerOptions[1];
            }
            else if (userInput == "C")
            {
                userAnswer = answerOptions[2];
            }
            else
            {
                Console.WriteLine("Câu trả lời không hợp lệ!");
                Thread.Sleep(500);
                return false;
            }

            if (userAnswer == correctAnswer)
            {
                Console.WriteLine("Chính xác!");
                Thread.Sleep(500);
                return true;
            }
            else
            {
                Console.WriteLine("Sai rồi!");
                Thread.Sleep(500);
                return false;
            }
        }

        //sắp xếp một mảng ngẫu nhiên bằng cách hoán đổi ngẫu nhiên vị trí của các phần tử trong mảng.
        static void ShuffleArray<T>(T[] array)//phần tử cần được sắp xếp ngẫu nhiên.
        {
            int n = array.Length;
            for (int i = 0; i < n; i++)
            {
                int randomIndex = i + random.Next(n - i); //mỗi lần lặp, một số ngẫu nhiên randomIndex được tạo ra bằng cách sử dụng hàm random.Next(n - i).
                /*Giá trị của phần tử hiện tại array[i] được tráo đổi với phần tử có chỉ số randomIndex.
                Quá trình tráo đổi này giúp đảm bảo mỗi phần tử trong mảng được đặt vào một vị trí ngẫu nhiên trong suốt quá trình lặp.*/
                T temp = array[i];
                array[i] = array[randomIndex];
                array[randomIndex] = temp;
            }
        }
        static bool CheckWin()
        {
            return (playerX == exitX && playerY == exitY);
        }

        static void Main(string[] args)
        {
            while (true)
            {
                string filePath = @"C:\NhacGameSuperMarioFull-VariousArt_3eqzv (1).wav";
                try
                {
                    SoundPlayer soundPlayer = new SoundPlayer(filePath);
                    soundPlayer.Play();
                }
                catch (FileNotFoundException)
                {
                    Console.OutputEncoding = Encoding.UTF8;
                    Console.WriteLine("Tệp âm thanh không tồn tại.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Đã xảy ra lỗi: " + ex.Message);
                }

                Console.CursorVisible = false; // Ẩn con trỏ nhấp nháy trên màn hình console.
                ShowTitle(); // Gọi hàm ShowTitle để hiển thị tựa đề game.

                bool showMenu = true;
                while (showMenu)
                {
                    showMenu = MainMenu();
                }
            }
        }

        static bool MainMenu()                                                                  //Giao diện các lựa chọn đầu game
        {
            Console.Clear();                                                                    // Xóa màn hình các hiển thị lúc trước
            Console.OutputEncoding = Encoding.UTF8;                                             // Hiển thị ra màn hình đúng theo tiếng việt
            Console.ForegroundColor = ConsoleColor.DarkRed;                                     // Thiết lập các dòng chữ bên dưới có màu đỏ đậm                                    
            Console.BackgroundColor = ConsoleColor.Gray;                                        // Thiết lập màu nền thành màu 
            Console.SetCursorPosition((Console.WindowWidth - 35) / 2, Console.CursorTop + 10);  // Đặt vị trí con trỏ 
            Console.WriteLine("---- TRÒ CHƠI \"THOÁT KHỎI MẬT THẤT\" ----");                    // Ghi tên trò chơi
            Console.ForegroundColor = ConsoleColor.DarkGreen;                                   // Thiết lập lại dòng chữ bên dưới có màu xanh đậm để nhấn mạnh
            Console.SetCursorPosition((Console.WindowWidth - 14) / 2, Console.CursorTop + 1);
            Console.WriteLine("1. Bắt đầu trò chơi  ");
            Console.ForegroundColor = ConsoleColor.DarkRed;                                     // Thiết lập lại các dòng chữ bên dưới có màu đỏ đậm
            Console.SetCursorPosition((Console.WindowWidth - 14) / 2, Console.CursorTop);
            Console.WriteLine("2. Hướng dẫn chơi    ");
            Console.SetCursorPosition((Console.WindowWidth - 14) / 2, Console.CursorTop);
            Console.WriteLine("3. Lịch sử chơi      ");
            Console.SetCursorPosition((Console.WindowWidth - 14) / 2, Console.CursorTop);
            Console.WriteLine("4. Bật/Tắt âm thanh  ");
            Console.SetCursorPosition((Console.WindowWidth - 14) / 2, Console.CursorTop);
            Console.WriteLine("5. Thoát GAME        ");
            Console.ResetColor();                                                                // Đặt lại màu chữ và màu nền về mặc định
            Console.WriteLine("\nMời bạn chọn từ 1-5");                                          // In ra màn hình Mời chọn các lựa chọn
            Console.CursorVisible = true;                                                        // Hiển thị vị trí con trỏ
            string userInput = Console.ReadLine();                                               // Đọc dữ liệu đầu vào của người dùng                                  
            // Xử lý lựa chọn của người dùng bằng cấu trúc switch-case.
            switch (userInput)                                                                   
            {
                case "1": // Thiết lập lại trò chơi và bắt đầu trò chơi.                                  
                   ResetGame2();
                   StartGame();
                   return true;                                                                  // Tiếp tục hiển thị hàm MainMenu sau khi xử lý lựa chọn của người dùng.
                case "2":// Hiển thị hướng dẫn trò chơi.
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.SetCursorPosition((Console.WindowWidth - 30) / 2, Console.CursorTop + 10);
                    Console.WriteLine("--- Chào mừng bạn đến với trò chơi!!! ---\n");
                    Console.ResetColor();
                    Console.WriteLine("             Chào mừng bạn đến với thế giới phiêu lưu kỳ thú của trò chơi \"THOÁT KHỎI MẬT THẤT\"!\n" +
                       "             Trong trò chơi này, bạn sẽ vào vai một nhà thám hiểm trên đường khám phá bị lạc vào một mật thất vòng vo.\n" +
                       "             Trò chơi bao gồm 2 level, nếu muốn tiếp tục chơi level 2 thì bạn phải vượt qua được level 1.\n" +
                       "             Hãy dùng các phím mũi tên để điều khiển nhân vật của bạn khám phá các ngóc ngách của mật thất.\n" +
                       "\n               Ở level 1:\n" +
                       "             - Điều kỳ lạ là trong mật thất hiện lên những câu hỏi hóc búa buộc bạn phải vắt óc giải mã để tìm thấy\n             lời giải đáp thoát khỏi mật thất.\n" +
                       "             - Thời gian là yếu tố quyết định điểm số của bạn - nhanh chóng thoát khỏi mật thất để nhận điểm số cao hơn.\n" +
                       "\n             Nếu bạn thoát được mật thất ngắn hơn 30 giây thì bạn sẽ được 100 điểm" +
                       "\n                                                    ngắn hơn 1 phút thì bạn sẽ được 50 điểm" +
                       "\n                                                    ngắn hơn 2 phút thì bạn sẽ được 25 điểm" +
                       "\n                                                    nếu lâu hơn 2 phút bạn sẽ chỉ được 10 điểm" +
                       "\n             Tuy nhiên, nếu bạn đã bị chặn hết đường thì bạn có thể gọi bảng menu, sau đó lựa chọn \"Bắt đầu lại game\"\n             hoặc \"Thoát game\" nhé!\n" +
                       "\n               Ở level 2:\n" +
                       "             - Bạn chỉ có 30 giây để thoát khỏi mật thất khi không gian xung quanh trở nên tối tăm và đáng sợ," +
                       "\n             nếu quá 30 giây thì bạn sẽ không vượt qua được level 2." +
                       "\n             - Ban đầu bạn sẽ có điểm cho level này là 100, mỗi giây trôi qua khi bạn còn ở trong mật thất thì\n             bạn sẽ bị trừ 1 điểm.\n" +
                       "\n                   => Nhấn phím \"Enter\" để chuẩn bị đến với trò chơi nào!");
                    Console.ReadLine();
                    return true;
                // Hiển thị lịch sử chơi của người chơi.
                case "3":
                    try
                    {
                        Console.Clear();
                        string showlichsu = File.ReadAllText("lichsu.txt");       // Đọc nội dung từ tệp "lichsu.txt" và lưu vào chuỗi showlichsu.
                        Console.WriteLine(showlichsu);                            // Hiển thị nội dung lịch sử chơi trên màn hình console.
                        Console.WriteLine(showlichsu.GetType().ToString());
                        Console.ReadLine();                                       // Dừng màn hình
                    }
                    catch (FileNotFoundException)                                 // Xử lý ngoại lệ nếu tệp "lichsu.txt" không tồn tại (không có lịch sử chơi).
                    {
                        Console.WriteLine("Không tìm thấy tệp lichsu.txt.");
                        Console.ReadLine();
                    }
                    catch (Exception)                                             // Xử lý nếu xảy ra lỗi khác.
                    {
                        Console.WriteLine("Đã xảy ra lỗi");
                    }
                    return true;
                // Bật hoặc tắt âm thanh.
                case "4":
                    ToggleSound();                                                // Gọi hàm để tắt/bật âm thanh
                    return true;
                // Thoát trò chơi.
                case "5":
                    ExitGame();                                                   // Gọi hàm để thoát 
                    return false;                                                 // Kết thúc việc hiển thị hàm MainMenu 
                // Ấn phím khác ngoài các phím từ 1-5 thì bảng MainMenu vẫn hiện đến khi bấm đúng từ 1-5
                default:
                    return true;
            }
        }

        static bool isSoundPlaying = true;                                         // Biến theo dõi trạng thái âm thanh
        // Hàm ToggleSound được sử dụng để bật hoặc tắt âm thanh.
        static void ToggleSound()
        {
            if (isSoundPlaying)                                // Kiểm tra trạng thái hiện tại của âm thanh
             { 
                SoundPlayer soundPlayer = new SoundPlayer(@"C:\NhacGameSuperMarioFull-VariousArt_3eqzv (1).wav");  // Tạo một đối tượng SoundPlayer để phát âm thanh từ tệp WAV
                soundPlayer.Stop();                                                                      // Dừng phát âm thanh
                isSoundPlaying = false;                                                                  // Cập nhật trạng thái âm thanh thành đã tắt
                Console.WriteLine("Đã tắt âm thanh.");                                                   // In ra màn hình thông báo về việc đã tắt âm thanh
            }
            else
            {
                SoundPlayer soundPlayer = new SoundPlayer(@"C:\NhacGameSuperMarioFull-VariousArt_3eqzv (1).wav");
                soundPlayer.Play();                                                                      // Phát âm thanh
                isSoundPlaying = true;                                                                   // Cập nhật trạng thái âm thanh thành đã bật
                Console.WriteLine("Đã bật âm thanh.");                                                   // In ra màn hình thông báo về việc đã bật âm thanh
            }
        }

        // Hàm ResetGame2 được sử dụng để đặt lại trạng thái của trò chơi.
        static void ResetGame2()
        {
            playerX = 1;                                                                                  // Đặt vị trí của người chơi về vị trí ban đầu (1, 1).
            playerY = 1;
            stopwatch.Reset();                                                                            // Đặt lại đồng hồ đo thời gian về trạng thái ban đầu.
             // Khôi phục mê cung gốc bằng cách sao chép nội dung của mê cung gốc vào mê cung hiện tại.
            for (int i = 0; i < maze.GetLength(0); i++)                                                   // Vòng for để duyệt qua từng vị trí trong mật thất
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    maze[i, j] = originalMaze[i, j];                                                      // Sao chép nội dung của mê cung gốc vào mê cung hiện tại
                }
            }
            Console.Clear();
        }

        static void ShowMenu()
        {
            Console.Clear(); // Xóa màn hình console
            Console.ForegroundColor = ConsoleColor.DarkBlue; // Thiết lập màu chữ là xanh dương đậm
            Console.BackgroundColor = ConsoleColor.Gray; // Thiết lập màu nền là xám
            Console.SetCursorPosition((Console.WindowWidth - 40) / 2, Console.CursorTop + 10); // Đặt con trỏ văn bản đến vị trí trung tâm màn hình và in ra tiêu đề menu
            Console.WriteLine("-------- BẢNG MENU --------"); // In ra các lựa chọn trong menu
            Console.SetCursorPosition((Console.WindowWidth - 40) / 2, Console.CursorTop + 1);
            Console.WriteLine("1. Lịch sử chơi            ");
            Console.SetCursorPosition((Console.WindowWidth - 40) / 2, Console.CursorTop);
            Console.WriteLine("2. Tiếp tục                ");
            Console.SetCursorPosition((Console.WindowWidth - 40) / 2, Console.CursorTop);
            Console.WriteLine("3. Bắt đầu lại game        ");
            Console.SetCursorPosition((Console.WindowWidth - 40) / 2, Console.CursorTop);
            Console.WriteLine("4. Tắt/Bật âm thanh        "); // Thêm lựa chọn tắt/bật âm thanh 
            Console.SetCursorPosition((Console.WindowWidth - 40) / 2, Console.CursorTop);
            Console.WriteLine("5. Thoát game              "); // Thêm lựa chọn khi người chơi bị chặn hết đường hoặc muốn thoát ngay lập tức
            Console.ResetColor(); // Đặt lại màu sắc mặc định và yêu cầu người dùng chọn một tùy chọn từ 1-6
            Console.WriteLine("\nMời bạn chọn từ 1-5");

            string userInput = Console.ReadLine(); // Đọc lựa chọn của người dùng từ bàn phím
            switch (userInput) // Xử lý lựa chọn của người dùng dựa trên giá trị nhập vào
            {
                case "1": // Trường hợp người chơi chọn "1"
                    Console.Clear(); // Xóa màn hình console
                                     // Đọc nội dung từ tệp "lichsu.txt" và in ra màn hình
                    string showlichsu = File.ReadAllText("lichsu.txt");
                    Console.WriteLine(showlichsu);
                    Console.WriteLine(showlichsu.GetType().ToString());
                    Console.ReadLine();
                    break;
                case "2": // Trường hợp người chơi chọn "2"
                    if (isPaused) // Kiểm tra nếu trò chơi đã bị tạm dừng
                    {
                        ResumeGame(); // Gọi phương thức ResumeGame để iếp tục trò chơi
                    }
                    break;
                case "3": // Trường hợp người chơi chọn "3"
                    ResetGame(); // Gọi phương thức ResetGame để đặt lại trạng thái trò chơi về ban đầu
                    break;
                case "4": // Trường hợp người chơi chọn "4"                
                    ToggleSound(); // Thay đổi trạng thái âm thanh (tắt/bật)
                    break;
                case "5": // Trường hợp người chơi chọn "5"
                    ExitGame(); // Gọi phương thức ExitGame để xử lý thoát khỏi trò chơi
                    break;
                default: // Trường hợp người chơi nhập lựa chọn không hợp lệ
                    ShowMenu(); // Hiển thị menu lại
                    break;
            }
        }

        //TẠO HIỆU ỨNG RƠI CHẬM CHO TÊN GAME
        static void PrintCenteredText(string text)                               
        {
            int consoleWidth = Console.WindowWidth; //Lấy chiều rộng hiện tại của cửa sổ Console và lưu giữ vào biến consoleWidth.
            int consoleHeight = Console.WindowHeight;//Lấy chiều cao hiện tại của cửa sổ Console và lưu giữ vào biến consoleHeight.
            int leftMargin = (consoleWidth - text.Length) / 2;//căn lề trái
            int topMargin = (consoleHeight - 1) / 2 - 8;//căn lề phải

            Console.SetCursorPosition(leftMargin, topMargin);//Đặt con trỏ tại vị trí (leftMargin, topMargin) để bắt đầu hiển thị văn bản từ vị trí này.

            for (int i = 0; i < text.Length; i++)
            {
                Console.SetCursorPosition(leftMargin + i, Console.CursorTop);// Đặt con trỏ tại vị trí (leftMargin + i, Console.CursorTop). Con trỏ văn bản di chuyển dọc theo dòng và ngang theo từng ký tự trong vòng lặp.
                Console.Write(text[i]);//Ghi ký tự hiện tại vào tại vị trí con trỏ văn bản hiện tại
                System.Threading.Thread.Sleep(50);   // 50 tích tắc giữa mỗi ký tự
            }
            // using để đảm bảo rằng file lichsu được giải phóng sau khi không còn dùng.
            // Tạo mới và Mở tệp "lichsu.txt" để ghi thêm nội dung vào cuối tệp nếu tệp đã tồn tại hoặc tạo mới nếu chưa tồn tại, cuối cùng là cho phép quyền ghi vào tệp
            using (FileStream lichsu = new FileStream("lichsu.txt", FileMode.Append, FileAccess.Write))
            {
                Console.InputEncoding = Encoding.UTF8;
                Console.OutputEncoding = Encoding.UTF8;
                Console.SetCursorPosition(leftMargin, topMargin + 1);
                Console.WriteLine();
                Console.WriteLine("Hãy nhập tên của bạn: ");
                StreamWriter Lichsuchoi = new StreamWriter(lichsu);                                       // Sử dụng StreamWriter để ghi dữ liệu vào tệp tin "lichsu.txt".                                      
                Lichsuchoi.Write("--------------------------------" + "\n" + Console.ReadLine() + "\n");  // Ghi "----...--", đọc và ghi tên người chơi nhập vào tệp "lichsu.txt" 
                Lichsuchoi.Flush();                                                                       // Đảm bảo rằng dữ liệu đã được ghi vào tệp
            }
        }

        static bool isPaused = false; // Biến để kiểm tra trạng thái dừng tạm thời.
        static TimeSpan pausedTime = TimeSpan.Zero; // Biến để lưu thời gian đã dừng tạm thời.
        static char[,] savedMaze; // Mê cung đã lưu trước khi dừng tạm thời.
        static int savedPlayerRow; // Vị trí hàng đã lưu trước khi dừng tạm thời.
        static int savedPlayerCol; // Vị trí cột đã lưu trước khi dừng tạm thời. 

        static void StartGame()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            Console.Clear();

            //TÊN GAME
            string gameName = "GAME THOÁT KHỎI MẬT THẤT";                                                   // TẠO TÊN CHO GAME
            Console.ForegroundColor = ConsoleColor.DarkRed;                                                 // Đặt màu chữ là màu 
            Console.BackgroundColor = ConsoleColor.Gray;                                                    // Đặt màu nền là màu 
            PrintCenteredText("------ " + gameName + " ------");
            Console.ResetColor();                                                                           // Đặt lại màu chữ và màu nền mặc định

            // THỜI GIAN
            DateTime startTime = DateTime.Now;                                                              // Lấy thời điểm bắt đầu chơi game

            // Hiển thị ngày và giờ bắt đầu chơi game
            Console.WriteLine("\n" + "--------------------------------");
            Console.WriteLine("day: " + startTime.ToShortDateString());
            Console.WriteLine("time: " + startTime.ToString("HH\\:mm\\:ss"));

            Console.CursorVisible = true; // Hiển thị con trỏ nhấp nháy trong console
            Console.ReadKey(); // Chờ người dùng nhấn một phím bất kỳ để bắt đầu trò chơi
            Console.WriteLine(); // Xuống dòng để tạo khoảng trống giữa thông báo và mê cung
            stopwatch = new Stopwatch();// Khởi tạo đối tượng Stopwatch để đếm thời gian
            stopwatch.Start(); // Bắt đầu đồng hồ đếm thời gian
            int score = 0; // Khởi tạo điểm số ban đầu
            while (true) // Vòng lặp chính của trò chơi
            {
                Console.Clear(); // Xóa màn hình console để vẽ lại các phần tử mới
                DrawMenu(); // Vẽ menu
                DrawMaze(); // Vẽ mê cung
                ConsoleKey key = Console.ReadKey().Key; // Đọc phím người dùng nhập vào

                if (key == ConsoleKey.M) // Kiểm tra phím được nhấn // Trường hợp người chơi nhấn phím "M" để hiển thị menu
                {
                    isPaused = true;
                    stopwatch.Stop(); // Tạm dừng đồng hồ
                    pausedTime = stopwatch.Elapsed; // Lưu thời gian đã dừng tạm thời 
                    // Sao chép mê cung hiện tại vào savedMaze
                    int rows = maze.GetLength(0);
                    int cols = maze.GetLength(1);
                    savedMaze = new char[rows, cols];
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            savedMaze[i, j] = maze[i, j];
                        }
                    }
                    // Lưu vị trí mới nhất của nhân vật
                    savedPlayerRow = playerX;
                    savedPlayerCol = playerY;
                    ShowMenu();
                }
                else // Trường hợp người chơi nhấn một phím di chuyển
                {
                    MovePlayer(key); // Di chuyển người chơi trong mê cung dựa trên phím nhấn
                }
                if (!isPaused)
                {
                    if (CheckWin())
                    {
                        Console.Clear();
                        DrawMaze();
                        Console.WriteLine("Chúc mừng bạn đã thắng level 1");
                        Console.WriteLine("Hãy nhấn phím 'Enter' để xem điểm và thời gian chơi của bạn");
                        stopwatch.Stop();
                        Console.ReadKey(true);
                        TimeSpan elapsedTime = stopwatch.Elapsed;
                        Console.WriteLine("Thời gian chơi: " + elapsedTime.ToString("mm\\:ss"));
                        if (elapsedTime < TimeSpan.FromSeconds(30))
                        { score += 100; }
                        else if (elapsedTime < TimeSpan.FromMinutes(1))
                        { score += 50; }
                        else if (elapsedTime < TimeSpan.FromMinutes(2))
                        { score += 25; }
                        else { score += 10; }
                        Console.WriteLine("Điểm của bạn là: " + score);
                        // xử lý ngoại lệ (trường hợp đặc biệt của try-catch-family) để lưu dữ liệu vào file (đã được giải thích tương tự ở trên)
                        using (FileStream lichsu = new FileStream("lichsu.txt", FileMode.Append, FileAccess.Write))           
                        {
                            Console.InputEncoding = Encoding.UTF8;
                            Console.OutputEncoding = Encoding.UTF8;
                            StreamWriter Lichsuchoi = new StreamWriter(lichsu);
                            Lichsuchoi.Write("Bạn đã vượt qua level 1 với số điểm:" + score + "\n");
                            Lichsuchoi.WriteLine("Thời gian chơi của bạn là: " + elapsedTime.ToString("mm\\:ss"));
                            Lichsuchoi.Write("Thời gian bắt đầu: " + startTime.ToString("HH\\:mm\\:ss") + ", " + startTime.ToShortDateString() + "\n");
                            Lichsuchoi.Flush();
                        }
                        Console.WriteLine("Bạn có muốn tiếp tục trò chơi không^^");
                        Console.WriteLine("1. Tiếp tục đến với level 2");
                        Console.WriteLine("2. Dừng lại tại đây thôi!");
                        Console.WriteLine("Mời nhập lựa chọn của bạn: ");
                        while (true)
                        {
                            string choice = Console.ReadLine();

                            if (choice == "1")
                            {
                                // Code để qua màn 2
                                Console.WriteLine("Bạn đã qua level 2");
                                Console.Clear();
                                Console.WriteLine("Để tiếp tục nhấn 'Enter'");

                                //vẽ maze
                                bool[][] mazeLayout =
                                    {
                                new [] {true, true, true, true, true, true, true, true, false, true, true, true, true},
                                new [] {true, false, false, false, true, false, true, true, false, false, true, true, true},
                                new [] {true, false, true, false, true, false, true, true, true, false, true, true, true},
                                new [] {true, false, true, false, true, false, true, true, true, false, false, false, true},
                                new [] {true, false, true, false, true, false, true, true, true, false, true, true, true},
                                new [] {true, false, true, true, true, false, true, true, true, false, false, false, true},
                                new [] {true, false, true, false, false, false, true, true, true, false, true, true, true},
                                new [] {true, false, true, false, true, false, true, true, true, false, true, false, true},
                                new [] {true, false, true, false, true, false, true, true, true, false, true, false, true},
                                new [] {true, false, true, false, true, false, false, false, false, false, true, false, true},
                                new [] {true, false, true, false, true, true, true, true, true, true, true, false, true},
                                new [] {true, false, true, false, false, false, false, false, false, false, false, false, true},
                                new [] {true, false, true, true, true, true, true, true, true, true, true, false, true},
                                new [] {true, false, false, false, false, false, false, false, false, false, false, false, true},
                                new [] {true, true, true, true, true, true, true, true, true, false, true, true, true}
                                };
                                // Đặt các biến
                                const char wallChar = '\u2588'; // Ký tự đại diện cho tường trong mê cung
                                const char mazeChar = '\u0020'; // Ký tự đại diện cho không gian trong mê cung
                                int exitRow = 0;                // Hàng của lối thoát trong mê cung
                                int exitColumn = 8;             // Cột của lối thoát trong mê cung

                                // In mê cung
                                for (int i = 0; i < mazeLayout.Length; i++)
                                {
                                    for (int j = 0; j < mazeLayout[i].Length; j++)
                                    {
                                        if (mazeLayout[i][j])
                                        {
                                            Console.Write(wallChar);
                                        }
                                        else
                                        {
                                            if (i == exitRow && j == exitColumn)
                                            {
                                                Console.Write("E"); // In ký tự đại diện cho lối thoát
                                            }
                                            else
                                            {
                                                Console.Write(mazeChar);
                                            }
                                        }
                                    }
                                    Console.WriteLine(); // Xuống dòng sau khi in một hàng
                                }

                                // Đặt biến
                                int playerRow = 14;                          // Hàng hiện tại của người chơi
                                int playerColumn = 9;                        // Cột hiện tại của người chơi
                                TimeSpan maxTime = TimeSpan.FromSeconds(30); // Thời gian tối đa để hoàn thành mê cung
                                stopwatch = new Stopwatch();                 // Khởi tạo đối tượng Stopwatch để đếm thời gian
                                stopwatch.Start();                           // Bắt đầu đếm thời gian
                                while (true)
                                {
                                    PrintMazeWithVisibilityMap();
                                    // Kiểm tra thời gian đã vượt quá thời gian tối đa hay chưa
                                    if (stopwatch.Elapsed > maxTime)
                                    {
                                        // Thông báo cho người chơi rằng thời gian đã hết
                                        Console.SetCursorPosition((Console.WindowWidth - 40) / 2, Console.CursorTop + 1);
                                        Console.WriteLine("Đã hết 30 giây. Bạn không tìm thấy vị trí exit.");
                                        Thread.Sleep(300);
                                        Console.SetCursorPosition((Console.WindowWidth - 40) / 2, Console.CursorTop + 1);
                                        elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(30));
                                        Console.WriteLine("Tổng thời gian chơi: " + elapsedTime.ToString("mm\\:ss"));
                                        Thread.Sleep(300);
                                        Console.SetCursorPosition((Console.WindowWidth - 40) / 2, Console.CursorTop + 1);
                                        Console.WriteLine("Tổng điểm của bạn là: " + score);
                                        Thread.Sleep(300);
                                        Console.SetCursorPosition((Console.WindowWidth - 40) / 2, Console.CursorTop + 1);
                                        Console.WriteLine("Nhấn Enter để quay về màn hình chính ");
                                        ConsoleKeyInfo keyInfo;

                                        do
                                        {
                                            keyInfo = Console.ReadKey();
                                        } while (keyInfo.Key != ConsoleKey.Enter);

                                        string filePath = "lichsu.txt";                                                // Đặt tên của tệp tin vào biến filePath
                                        int linesToDelete = 3;
                                        string[] lines = File.ReadAllLines(filePath);                                  // Đọc nội dung của tệp tin được chỉ định bởi filePath, lưu trữ nó trong một mảng các chuỗi lines        
                                        if (lines.Length <= linesToDelete)                                             // Kiểm tra xem có đủ số dòng để xóa hay không
                                        {
                                            Console.WriteLine("Không đủ dòng để xóa.");                                // Nếu không đủ dòng để xóa, in ra thông báo "Không đủ dòng"
                                            return;
                                        }
                                        string[] remainingLines = lines.Take(lines.Length - linesToDelete).ToArray();  // Giữ lại các dòng từ đầu cho đến n - linesToDelete
                                        File.WriteAllLines(filePath, remainingLines);                                  // Ghi nội dung còn lại vào file, ghi đè nội dung cũ
                                        // xử lý ngoại lệ (trường hợp đặc biệt của try-catch-family) để lưu dữ liệu vào file (đã được giải thích tương tự ở trên)
                                        using (FileStream lichsu = new FileStream("lichsu.txt", FileMode.Append, FileAccess.Write))           
                                        {
                                            Console.InputEncoding = Encoding.UTF8;
                                            Console.OutputEncoding = Encoding.UTF8;
                                            StreamWriter Lichsuchoi = new StreamWriter(lichsu);
                                            Lichsuchoi.Write("Bạn chưa vượt qua 2 level\n" + "Tổng điểm:" + score + "\n");
                                            Lichsuchoi.WriteLine("Thời gian chơi của bạn là (bao gồm 30s ở level 2): " + elapsedTime.ToString("mm\\:ss"));
                                            Lichsuchoi.Write("Thời gian bắt đầu: " + startTime.ToString("HH\\:mm\\:ss") + ", " + startTime.ToShortDateString() + "\n");
                                            Lichsuchoi.Flush();
                                        }
                                        break;
                                    }

                                    // Di chuyển của nhân vật
                                    switch (Console.ReadKey(true).Key)
                                    {
                                        case ConsoleKey.UpArrow:
                                            if (IsValidMove(playerRow - 1, playerColumn))
                                            {
                                                playerRow--; // Di chuyển người chơi lên
                                            }
                                            break;
                                        case ConsoleKey.DownArrow:
                                            if (IsValidMove(playerRow + 1, playerColumn))
                                            {
                                                playerRow++; // Di chuyển người chơi xuống
                                            }
                                            break;
                                        case ConsoleKey.LeftArrow:
                                            if (IsValidMove(playerRow, playerColumn - 1))
                                            {
                                                playerColumn--; // Di chuyển người chơi qua trái
                                            }
                                            break;
                                        case ConsoleKey.RightArrow:
                                            if (IsValidMove(playerRow, playerColumn + 1))
                                            {
                                                playerColumn++; // Di chuyển người chơi qua phải
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    Console.Clear(); // Xóa màn hình để vẽ lại mê cung sau khi di chuyển người chơi
                                    if (playerRow == exitRow && playerColumn == exitColumn)
                                    {
                                        stopwatch.Stop();
                                        TimeSpan elapsedTimess = stopwatch.Elapsed;
                                        TimeSpan sumelapsedTime = elapsedTimess + elapsedTime;
                                        Console.WriteLine("Thời gian chơi: " + elapsedTimess.ToString("mm\\:ss"));
                                        Console.WriteLine("Hãy nhấn phím 'Enter' để xem điểm và thời gian chơi của bạn");
                                        Console.ReadKey();
                                        int secondsPlayed = (int)elapsedTimess.TotalSeconds;
                                        int totalScore = 100 - secondsPlayed;

                                        Console.WriteLine("Chúc mừng bạn đã tìm thấy lối ra cuối cùng!");
                                        Console.WriteLine("Điểm level 2 của bạn là: " + totalScore);
                                        int totalscore = totalScore + score;
                                        Console.WriteLine("Điểm tổng của bạn là: " + totalscore);
                                        Console.WriteLine("Nhấn Enter để quay về màn hình chính ");
                                        Console.ReadKey();

                                        // Các dòng lệnh dưới tương tự được giải thích ở trên, có tác dụng xóa 3 dòng cuối ở tệp "lichsu.txxt" hiện có nếu đủ dòng xóa
                                        string filePath = "lichsu.txt";
                                        int linesToDelete = 3;
                                        string[] lines = File.ReadAllLines(filePath);
                                        if (lines.Length <= linesToDelete)
                                        {
                                            Console.WriteLine("Không đủ dòng để xóa.");
                                            return;
                                        }
                                        string[] remainingLines = lines.Take(lines.Length - linesToDelete).ToArray();
                                        File.WriteAllLines(filePath, remainingLines);
                                        // xử lý ngoại lệ (trường hợp đặc biệt của try-catch-family) để lưu dữ liệu vào file (đã được giải thích tương tự ở trên)
                                        using (FileStream lichsu = new FileStream("lichsu.txt", FileMode.Append, FileAccess.Write))
                                        {
                                            Console.InputEncoding = Encoding.UTF8;
                                            Console.OutputEncoding = Encoding.UTF8;
                                            StreamWriter Lichsuchoi = new StreamWriter(lichsu);
                                            Lichsuchoi.Write("Bạn đã vượt qua 2 level\n" + "Tổng điểm:" + totalscore + "\n");
                                            Lichsuchoi.WriteLine("Thời gian chơi của bạn là: " + sumelapsedTime.ToString("mm\\:ss"));
                                            Lichsuchoi.Write("Thời gian bắt đầu: " + startTime.ToString("HH\\:mm\\:ss") + ", " + startTime.ToShortDateString() + "\n");
                                            Lichsuchoi.Flush();
                                        }
                                        break;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < mazeLayout.Length; i++)
                                        {
                                            for (int j = 0; j < mazeLayout[i].Length; j++)
                                            {
                                                if (mazeLayout[i][j])
                                                {
                                                    Console.Write(wallChar);
                                                }
                                                else if (i == playerRow && j == playerColumn)
                                                {
                                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                                    Console.Write("☺");
                                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                                }
                                                else if (i == exitRow && j == exitColumn)
                                                {
                                                    Console.Write("E");
                                                }
                                                else
                                                {
                                                    Console.Write(mazeChar);

                                                }
                                            }
                                            Console.WriteLine();
                                        }

                                    }
                                }

                                bool IsValidMove(int row, int column)
                                {
                                    if (row >= 0 && row < mazeLayout.Length && column >= 0 && column < mazeLayout[row].Length && !mazeLayout[row][column])
                                    {
                                        return true;
                                    }
                                    return false;
                                }

                                void UpdateVisibilityMap()
                                {
                                    bool[][] visibilityMap = new bool[mazeLayout.Length][];                          // Khai báo 1 mảng 2 chiều visibilityMap để biểu thị khả năng nhìn thấy của từng ô trong mê cung
                                    for (int i = 0; i < mazeLayout.Length; i++)                                      // Khởi tạo mảng visibilityMap với kích thước tương ứng với kích thước của mazeLayout
                                    {
                                        visibilityMap[i] = new bool[mazeLayout[i].Length];
                                    }

                                    for (int i = Math.Max(0, playerRow - 2); i <= Math.Min(mazeLayout.Length - 1, playerRow + 2); i++)               // Duyệt qua các ô xung quanh vị trí của người chơi (trong phạm vi 2 ô) và đánh dấu các ô tương ứng trong visibilityMap là true 
                                    {
                                        for (int j = Math.Max(0, playerColumn - 2); j <= Math.Min(mazeLayout[i].Length - 1, playerColumn + 2); j++)
                                        {
                                            visibilityMap[i][j] = true;
                                        }
                                    }

                                    visibilityMap[playerRow][playerColumn] = true;                                                                   // Đánh dấu vị trí của người chơi trong visibilityMap là true

                                    for (int i = 0; i < mazeLayout.Length; i++)                                                                      // Duyệt qua mazeLayout và in ra các ô dựa trên giá trị của visibilityMap
                                    {
                                        for (int j = 0; j < mazeLayout[i].Length; j++)
                                        {
                                            if (visibilityMap[i][j])
                                            {
                                                Console.SetCursorPosition(j, i);
                                                if (i == playerRow && j == playerColumn)
                                                {
                                                    Console.Write('☺');
                                                }
                                                else if (i == exitRow && j == exitColumn)
                                                {
                                                    Console.Write('E');
                                                }
                                                else
                                                {
                                                    Console.Write(mazeLayout[i][j] ? wallChar : mazeChar);
                                                }
                                            }
                                        }
                                    }

                                    Console.SetCursorPosition(playerColumn, playerRow);                                                             // Đặt vị trí con trỏ in của Console tại vị trí của người chơi
                                }

                                void PrintMazeWithVisibilityMap()                                                                                   // Xóa màn hình và gọi hàm UpdateVisibilityMap để cập nhật và in ra bản đồ mê cung với bản đồ hiển thị khả năng nhìn thấy 
                                {
                                    Console.Clear();
                                    UpdateVisibilityMap();
                                }

                                break;
                            }
                            else if (choice == "2")
                            {
                                // Code để thoát game
                                MainMenu();
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Lựa chọn không hợp lệ. Vui lòng chọn lại.");
                            }
                        }

                        break;
                    }
                }
            }
        }

        static void ShowTitle() // Phương thức dùng để vẽ tựa game cùng với khung sao hình chữ nhật bao quanh
        {
            Console.ForegroundColor = ConsoleColor.Cyan; // Đặt màu sắc chữ trong console thành màu xanh lam
            Console.OutputEncoding = Encoding.UTF8; // Đặt bảng mã console thành UTF-8 để hỗ trợ các ký tự đặc biệt
            string title = " THOÁT KHỎI MẬT THẤT "; // Chuỗi tiêu đề

            // Tính toán lề trái và lề trên để đặt tiêu đề trong cửa sổ console
            int leftMargin = (Console.WindowWidth - title.Length) - 30;
            int topMargin = Console.WindowHeight - 12;
            Console.SetCursorPosition(leftMargin, topMargin); // Đặt con trỏ văn bản tới vị trí để in tiêu đề
            Console.WriteLine(title); // Bắt đầu in tiêu đề

            Console.ForegroundColor = ConsoleColor.Yellow; // Đặt màu sắc cho sao nhấp nháy thành màu vàng
            DrawBlinkingStars(leftMargin - 2, topMargin - 3, title.Length + 4); // Gọi phương thức DrawBlinkingStars để vẽ khung sao nhấp nháy xung quanh tựa game
        }

        static void DrawBlinkingStars(int x, int y, int width) // Phương thức dùng để xử lý khung sao nhấp nháy
        /* Tham số x đại diện cho tọa độ x (hoành độ) của góc trái trên của khung sao.
        Tham số y đại diện cho tọa độ y (tung độ) của góc trái trên của khung sao.
        Tham số width đại diện cho độ dài của khung sao. */
        {
            Random random = new Random(); // Đối tượng Random để tạo số ngẫu nhiên
            bool blinking = true; // Biến để kiểm soát trạng thái nhấp nháy - true (đang nháy lên)

            while (true) // Vòng lặp vô hạn để tạo hiệu ứng nhấp nháy
            {
                Console.SetCursorPosition(x, y); // Đặt con trỏ văn bản tới vị trí bắt đầu vẽ dòng sao nhấp nháy trên cùng
                                                 // Vẽ dòng sao nhấp nháy trên cùng
                for (int i = 0; i < width; i++)
                {
                    // Random số 0 hoặc 1 để quyết định in sao hoặc khoảng trắng
                    if (random.Next(2) == 0) // Nếu random ra số 0 thì vẽ dấu sao
                        Console.Write("*");
                    else // Nếu ra 1 thì vẽ khoảng trắng
                        Console.Write(" ");
                }

                Console.SetCursorPosition(x, y + 6); // Đặt con trỏ văn bản tới vị trí bắt đầu vẽ dòng sao nhấp nháy phía dưới
                                                     // Vẽ dòng sao nhấp nháy phía dưới
                for (int i = 0; i < width; i++)
                {
                    // Random số 0 hoặc 1 để quyết định in sao hoặc khoảng trắng
                    if (random.Next(2) == 0) // Nếu random ra số 0 thì vẽ dấu sao
                        Console.Write("*");
                    else // Nếu ra 1 thì vẽ khoảng trắng
                        Console.Write(" ");
                }
                Thread.Sleep(200); // Ngừng chương trình trong 200 miliseconds để tạo hiệu ứng nhấp nháy

                // Vẽ các hàng sao nhấp nháy khác
                for (int i = 0; i < 7; i++)
                {
                    Console.SetCursorPosition(x, y + i);

                    if (i == 1 || i == 3 || i == 5)
                    {
                        if (blinking)
                        {
                            Console.Write("*");
                            Console.SetCursorPosition(x + width - 1, y + i);
                            Console.Write("*");
                        }
                        else
                        {
                            Console.Write(" ");
                            Console.SetCursorPosition(x + width - 1, y + i);
                            Console.Write(" ");
                        }
                    }
                    else if (i == 1 || i == 4 || i == 7)
                    {
                        if (blinking)
                        {
                            Console.Write("*");
                            Console.SetCursorPosition(x + width - 1, y + i);
                            Console.Write("*");
                        }
                        else
                        {
                            Console.Write(" ");
                            Console.SetCursorPosition(x + width - 1, y + i);
                            Console.Write(" ");
                        }
                    }
                    else
                    {
                        Console.Write("*");
                        for (int j = 1; j < width - 1; j++)
                        {
                            Console.Write(" ");
                        }
                        Console.Write("*");
                    }
                }
                Thread.Sleep(200); // Ngừng vòng for thứ 3 trong 200 miliseconds để tạo hiệu ứng nhấp nháy

                if (Console.KeyAvailable) // Kiểm tra xem có phím nào được nhấn trên bàn phím không
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true); // Đọc thông tin về phím được nhấn
                    if (keyInfo.Key == ConsoleKey.Enter) // Kiểm tra xem phím Enter có được nhấn không
                    {
                        break; // Thoát khỏi vòng lặp nếu phím Enter được nhấn
                    }
                }
                blinking = !blinking; // Chuyển đổi trạng thái nhấp nháy, dấu sao chuyển từ sáng sang tối
            }
        }

        static void DrawMenu()
        {
            int menuX = 30; // Vị trí X của menu
            int menuY = 0; // Vị trí Y của menu

            Console.SetCursorPosition(menuX, menuY); // Đặt vị trí con trỏ hiển thị trên màn hình console tại tọa độ (menuX, menuY)
            Console.ForegroundColor = ConsoleColor.White; // Đặt màu chữ trong console thành màu trắng
            Console.BackgroundColor = ConsoleColor.DarkBlue; // Đặt màu nền trong console thành màu xanh đậm
            Console.WriteLine("MENU (Nhấn 'M' hoặc 'm' để mở)"); // Hiển thị dòng chữ "MENU (Nhấn 'M' hoặc 'm' để mở)" trên màn hình console
            Console.ResetColor(); // Đặt lại màu chữ và màu nền về mặc định của console
        }

        static char[,] originalMaze = new char[,]  // Mảng hai chiều đại diện cho mê cung gốc.
        {
            { '╔', '═', '═', '═', '═', '═', '═', '═', '═', '╗' },
            { '║', '.', '.', '!', '.', '#', '.', '.', '.', '║' },
            { '║', '.', '!', '.', '.', '!', '.', '.', '#', '║' },
            { '║', '!', '.', '#', '#', '.', '.', '!', '.', '║' },
            { '║', '.', '.', '!', '.', '!', '#', '.', '#', '║' },
            { '║', '!', '#', '#', '#', '.', '.', '!', '.', '║' },
            { '║', '.', '!', '.', '.', '!', '#', '.', '!', '║' },
            { '║', '#', '.', '#', '!', '.', '.', '!', '.', '║' },
            { '║', '.', '.', '.', '#', '.', '!', '.', '.', '║' },
            { '╚', '═', '═', '═', '═', '═', '═', '═', '═', '╝' }
        };

        static void ResetGame()
        {
            playerX = 1; // Đặt vị trí người chơi X về 1
            playerY = 1; // Đặt vị trí người chơi Y về 1
            stopwatch.Reset(); // Đặt lại đồng hồ đếm thời gian về 0
            pausedTime = TimeSpan.Zero; // Đặt thời gian tạm dừng về 0
            Console.Clear(); // Xóa nội dung hiển thị trên màn hình console

            // Khôi phục mê cung gốc
            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    maze[i, j] = originalMaze[i, j]; // Gán giá trị của mê cung gốc cho mê cung hiện tại
                }
            }

            Console.WriteLine("Game đã được reset. Bắt đầu lại từ đầu."); // Hiển thị dòng chữ "Game đã được reset. Bắt đầu lại từ đầu." trên màn hình console
            Thread.Sleep(2000); // Dừng thực thi trong 2 giây để người chơi có thời gian đọc thông báo
            stopwatch.Start(); // Bắt đầu đếm thời gian
            isPaused = false; // Đặt trạng thái tạm dừng về false để tiếp tục chơi game
        }

        static void ResumeGame()
        {
            isPaused = false; // Đặt trạng thái tạm dừng về false để tiếp tục chơi game
            Array.Copy(savedMaze, maze, savedMaze.Length); // Sao chép các phần tử của savedMaze vào mảng maze
            playerX = savedPlayerRow; // Khôi phục vị trí hàng của người chơi
            playerY = savedPlayerCol; // Khôi phục vị trí cột của người chơi
            stopwatch.Start(); // Bắt đầu đồng hồ đếm thời gian lại
        }

        static void ExitGame()
        {
            Console.Clear(); // Xóa nội dung hiển thị trên màn hình console
            string message = "Cảm ơn bạn đã chơi game";
            int consoleWidth = Console.WindowWidth; // Lấy chiều rộng của cửa sổ console
            int consoleHeight = Console.WindowHeight; // Lấy chiều cao của cửa sổ console
            int leftMargin = (consoleWidth - message.Length) / 2; // Tính toán lề trái để căn giữa dòng thông báo
            int topMargin = consoleHeight / 2; // Tính toán lề trên để căn giữa dòng thông báo
            Console.SetCursorPosition(leftMargin, topMargin); // Đặt vị trí con trỏ hiển thị trên màn hình console để in dòng thông báo
            Console.WriteLine(message); // In dòng thông báo "Cảm ơn bạn đã chơi game" trên màn hình console
            Thread.Sleep(2000); // Dừng thực thi trong 2 giây để người dùng có thời gian đọc thông báo
            Environment.Exit(0); // Kết thúc chương trình
        }
    }
}
