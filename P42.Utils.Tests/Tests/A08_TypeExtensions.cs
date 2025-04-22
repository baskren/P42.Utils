using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace P42.Utils.Tests;

[TestClass]
public class A08_TypeExtensions
{
    public class Person
    {
        public int Id { get; set; } = -1;
        public string Name { get; set; } = string.Empty;

        public Person(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class Student : Person
    {
        public int Year { get; set; }

        public Student(int id, string name, int year) : base(id, name)
        {
            Year = year;
        }
    }

    [TestMethod]
    public void A00_IsCastableTo()
    {
        var studentType = typeof(Student);
        var personType = typeof(Person);
        studentType.IsCastableTo(personType).ShouldBe(true);
        personType.IsCastableTo(studentType).ShouldBe(false);

        studentType.IsCastableTo<Person>().ShouldBe(true);
        personType.IsCastableTo<Student>().ShouldBe(false);
    }


}
