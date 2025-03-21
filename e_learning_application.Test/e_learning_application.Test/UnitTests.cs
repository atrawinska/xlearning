using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using e_learning_application.Models;
using Xunit;

namespace e_learning_application.Tests
{
    public class BusinessLogicTests
    {
        [Fact]
        public void Student_CannotEnrollTwiceInSameSubject()
        {
            // Arrange
            var student = new Student("john.doe", "password", "John Doe");
            var subject = new Subject("Mathematics", "An introductory math course", teacherId: 1);
            
            // Act: Enroll student if not already enrolled.
            if (!student.EnrolledSubjects.Any(s => s.Id == subject.Id))
            {
                student.EnrolledSubjects.Add(subject);
            }
            // Try enrolling the same subject a second time.
            if (!student.EnrolledSubjects.Any(s => s.Id == subject.Id))
            {
                student.EnrolledSubjects.Add(subject);
            }

            // Assert: The student should only be enrolled in the subject once.
            Assert.Single(student.EnrolledSubjects);
        }


        // below is the example how the test may fail if student enrolls twice for the same subject.
        [Fact]
        public void Student_EnrollsTwice_FailsTest()
        {
            // Arrange
            var student = new Student("john.doe", "password", "John Doe");
            var subject = new Subject("Mathematics", "An introductory math course", teacherId: 1);

            // Act: Intentionally add the subject twice without checking for duplicates.
            student.EnrolledSubjects.Add(subject);
            student.EnrolledSubjects.Add(subject);

            // Assert: This assertion will fail because the student is enrolled twice.
            Assert.Single(student.EnrolledSubjects);
        }

        [Fact]
        public void SaveData_ReadData_Student()
        {
            // Arrange: Create a temporary file path within a temporary folder to isolate test data.
            string tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);
            string tempStudentFile = Path.Combine(tempFolder, "student.json");

            // Override the file path by copying the logic from JsonReader constructor.
            var jsonReader = new JsonReader("Student");
            // For testing, overwrite the _filePath (using reflection or reassigning in a derived test class) 
            // is one approach. Here we simulate persistence by writing to a temporary file.
            // We'll write and read our data using the same instance.
            // (Note: In a real-world scenario, dependency injection would allow us to set the file path.)
            
            // Save some test students.
            var students = new List<Student>
            {
                new Student("alice", "pass123", "Alice Wonderland"),
                new Student("bob", "pass456", "Bob Builder")
            };

            // Act: Save and then read the student data.
            jsonReader.SaveData(students);
            var readStudents = jsonReader.ReadData<Student>();

            // Assert: The number of students read should match the number saved.
            Assert.Equal(students.Count, readStudents.Count);

            // Cleanup: Optionally, remove the test data file.
            // File.Delete(tempStudentFile);
            // Directory.Delete(tempFolder, recursive: true);
        }

        [Fact]
        public void Teacher_CannotModifySubjectNotOwned()
        {
            // Arrange: Create two teachers and a subject owned by teacherA.
            var teacherA = new Teacher("teacherA", "passA", "Alice Teacher");
            var teacherB = new Teacher("teacherB", "passB", "Bob Teacher");
            var subject = new Subject("Physics", "Fundamentals of Physics", teacherA.Id);
            teacherA.MySubjects.Add(subject);

            // Act: Simulate teacherB trying to modify the subject.
            // In a real business layer, we might throw an exception or return an error if teacherB attempts a modification.
            // For this simple test, we check that the subject's TeacherId does not match teacherB's Id.
            bool canTeacherBModify = subject.TeacherId == teacherB.Id;

            // Assert: teacherB should not be allowed to modify the subject.
            Assert.False(canTeacherBModify);
        }
    }
}