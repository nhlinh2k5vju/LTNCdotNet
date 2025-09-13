using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleApp1
{
    internal class Bai5
    {
        public static void Run()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            StudentManager manager = new StudentManager("students.txt");
            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\n==============================================");
                Console.WriteLine("==        Bài 5: Danh sách sinh viên          ==");
                Console.WriteLine("==============================================");
                Console.WriteLine("1. Thêm sinh viên mới");
                Console.WriteLine("2. Tìm kiếm sinh viên theo tên");
                Console.WriteLine("3. Cập nhật thông tin");
                Console.WriteLine("4. Xóa bản ghi");
                Console.WriteLine("5. Thống kê số lần đăng ký");
                Console.WriteLine("6. Lưu thay đổi và thoát");
                Console.WriteLine("7. Hiển thị tất cả sinh viên");
                Console.WriteLine("0. Thoát không lưu");
                Console.WriteLine("----------------------------------------------");
                Console.Write("Chọn chức năng: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        manager.AddStudent();
                        break;
                    case "2":
                        Console.Write("Nhập tên cần tìm: ");
                        string nameToSearch = Console.ReadLine();
                        var results = manager.FindStudentsByName(nameToSearch);
                        if (results.Count == 0)
                        {
                            Console.WriteLine("Không có kết quả.");
                        }
                        else
                        {
                            Console.WriteLine("\n--- KẾT QUẢ TÌM KIẾM ---");
                            foreach (var s in results)
                            {
                                Console.WriteLine(s);
                            }
                        }
                        break;
                    case "3":
                        manager.UpdateStudent();
                        break;
                    case "4":
                        manager.DeleteStudent();
                        break;
                    case "5":
                        manager.GenerateCourseReport();
                        break;
                    case "6":
                        manager.SaveChangesToFile();
                        isRunning = false;
                        break;
                    case "7":
                        manager.ShowAllStudents();
                        break;
                    case "0":
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ.");
                        break;
                }

                if (isRunning)
                {
                    Console.WriteLine("\nNhấn phím bất kỳ để tiếp tục...");
                    Console.ReadKey();
                }
            }
        }
    }

    // Class Sinh viên
    public class Student
    {
        public string Name { get; set; }
        public int Semester { get; set; }
        public string CourseName { get; set; }

        public Student(string name, int semester, string courseName)
        {
            Name = name;
            Semester = semester;
            CourseName = courseName;
        }

        public override string ToString()
        {
            return $"Name: {Name,-20} | Semester: {Semester} | Course: {CourseName}";
        }
    }

    // Class quản lý
    public class StudentManager
    {
        private List<Student> _students;
        private readonly string _filePath;
        private readonly HashSet<string> _validCourses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        { "Java", ".Net", "C/C++", "C++" };

        public StudentManager(string fileName)
        {
            string projectDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
            _filePath = Path.Combine(projectDir, fileName);

            _students = new List<Student>();
            LoadFromFile();
        }

        private void LoadFromFile()
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine($"File '{_filePath}' chưa tồn tại. Khi lưu sẽ tự tạo.");
                return;
            }

            string[] lines = File.ReadAllLines(_filePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                if (parts.Length == 3)
                {
                    string name = parts[0].Trim();
                    if (int.TryParse(parts[1].Trim(), out int semester) && IsValidCourse(parts[2].Trim()))
                    {
                        _students.Add(new Student(name, semester, NormalizeCourse(parts[2].Trim())));
                    }
                }
            }
            Console.WriteLine($"Đã tải {_students.Count} bản ghi.");
        }

        public void SaveChangesToFile()
        {
            List<string> lines = new List<string>();
            foreach (Student s in _students)
            {
                lines.Add($"{s.Name},{s.Semester},{s.CourseName}");
            }
            File.WriteAllLines(_filePath, lines);
            Console.WriteLine("Lưu thành công.");
        }

        public void AddStudent()
        {
            Console.Write("Nhập tên sinh viên: ");
            string name = Console.ReadLine();

            int semester;
            Console.Write("Nhập học kỳ: ");
            while (!int.TryParse(Console.ReadLine(), out semester) || semester <= 0)
            {
                Console.Write("Học kỳ không hợp lệ, nhập lại: ");
            }

            string course;
            Console.Write("Nhập khóa học (Java, .Net, C/C++): ");
            do
            {
                course = Console.ReadLine();
            } while (!IsValidCourse(course));

            course = NormalizeCourse(course);

            _students.Add(new Student(name, semester, course));
            Console.WriteLine("Thêm sinh viên thành công!");
        }

        public List<Student> FindStudentsByName(string searchName)
        {
            List<Student> result = new List<Student>();
            foreach (Student s in _students)
            {
                if (s.Name.ToLower().Contains(searchName.ToLower()))
                {
                    result.Add(s);
                }
            }
            return result;
        }

        public void UpdateStudent()
        {
            Console.Write("Nhập tên sinh viên cần sửa: ");
            string name = Console.ReadLine();
            List<Student> found = FindStudentsByName(name);

            if (found.Count == 0)
            {
                Console.WriteLine("Không tìm thấy.");
                return;
            }

            for (int i = 0; i < found.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] {found[i]}");
            }

            Console.Write("Chọn số thứ tự để sửa (0 = hủy): ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice <= 0 || choice > found.Count) return;

            Student sv = found[choice - 1];

            Console.Write($"Tên mới (Enter để giữ '{sv.Name}'): ");
            string newName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newName)) sv.Name = newName;

            Console.Write($"Học kỳ mới (Enter để giữ {sv.Semester}): ");
            string newSemesterStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newSemesterStr) && int.TryParse(newSemesterStr, out int newSemester))
                sv.Semester = newSemester;

            Console.Write($"Khóa học mới (Enter để giữ '{sv.CourseName}'): ");
            string newCourse = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newCourse) && IsValidCourse(newCourse))
                sv.CourseName = NormalizeCourse(newCourse);

            Console.WriteLine("Cập nhật thành công!");
        }

        public void DeleteStudent()
        {
            Console.Write("Nhập tên sinh viên cần xóa: ");
            string name = Console.ReadLine();
            List<Student> found = FindStudentsByName(name);

            if (found.Count == 0)
            {
                Console.WriteLine("Không tìm thấy.");
                return;
            }

            for (int i = 0; i < found.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] {found[i]}");
            }

            Console.Write("Chọn số thứ tự để xóa (0 = hủy): ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice <= 0 || choice > found.Count) return;

            _students.Remove(found[choice - 1]);
            Console.WriteLine("Đã xóa thành công!");
        }
        public void GenerateCourseReport()
        {
            if (_students.Count == 0)
            {
                Console.WriteLine("Không có dữ liệu.");
                return;
            }

            Dictionary<string, int> report = new Dictionary<string, int>();

            foreach (Student s in _students)
            {
                string key = s.Name + "|" + s.CourseName;
                if (report.ContainsKey(key))
                {
                    report[key]++;
                }
                else
                {
                    report[key] = 1;
                }
            }

            Console.WriteLine("\n--- BÁO CÁO ---");
            Console.WriteLine($"{"Student Name",-20} | {"Course",-10} | Total");
            Console.WriteLine("--------------------------------------------");

            foreach (var kvp in report)
            {
                string[] parts = kvp.Key.Split('|');
                string name = parts[0];
                string course = parts[1];
                int total = kvp.Value;

                Console.WriteLine($"{name,-20} | {course,-10} | {total}");
            }
        }

        // Hiển thị tất cả sinh viên
        public void ShowAllStudents()
        {
            if (_students.Count == 0)
            {
                Console.WriteLine("Danh sách trống.");
                return;
            }

            Console.WriteLine("\n--- DANH SÁCH SINH VIÊN ---");
            foreach (Student s in _students)
            {
                Console.WriteLine(s);
            }
        }

        private bool IsValidCourse(string courseName)
        {
            return _validCourses.Contains(courseName.Trim());
        }

        private string NormalizeCourse(string courseName)
        {
            string c = courseName.Trim().ToLower();
            if (c == "c++") return "C/C++";
            foreach (string valid in _validCourses)
            {
                if (valid.Equals(courseName.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return valid;
                }
            }
            return courseName;
        }
    }
}
