using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== CHỌN BÀI TẬP ===");
            Console.WriteLine("1. Bài 1");
            Console.WriteLine("2. Bài 2");
            Console.WriteLine("3. Bài 3");
            Console.WriteLine("4. Bài 4");
            Console.WriteLine("5. Bài 5 (Quản lý sinh viên)");
            Console.Write("Nhập lựa chọn: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Bai1.Run();
                    break;
                case "2":
                    Bai2.Run();
                    break;
                case "3":
                    Bai3.Run();
                    break;
                case "4":
                    Bai4.Run();
                    break;
                case "5":
                    Bai5.Run();
                    break;
                default:
                    Console.WriteLine("❌ Lựa chọn không hợp lệ!");
                    break;
            }

            Console.WriteLine("\nNhấn phím bất kỳ để thoát...");
            Console.ReadKey();
        }
    }
}
