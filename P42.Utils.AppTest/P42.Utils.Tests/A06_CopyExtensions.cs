using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace P42.Utils.AppTest;

[TestClass]
public class A06_CopyExtensions
{
    public class Person : ICopiable<Person>
    {
        public int Id { get; set; } = -1;
        public string Name { get; set; } = string.Empty;

        public void PropertiesFrom(Person other)
        {
            Id = other.Id;
            Name = other.Name;
        }

        public Person()
        { }

        public Person(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    static List<Person> Persons => new List<Person>( )
    {
        new Person(1, "Ann"),
        new Person(2, "Bob"),
        new Person(3, "Carl")
    };

    [TestMethod]
    public void A00_ReferenceCopy()
    {
        var persons = Persons;
        var alt = persons.ToList();
        ReferenceEquals(alt, persons).ShouldBeFalse();
        for (int i = 0; i < persons.Count; i++)
            ReferenceEquals(alt[i],persons[i]).ShouldBeTrue();

        var dennis = new Person(4, "Dennis");
        alt.Add(dennis);

        persons.Count.ShouldBe(3);
        alt.Count.ShouldBe(4);

        alt[2].Name = "Charlie";
        alt[2].Name.ShouldBe(persons[2].Name);

    }

    [TestMethod]
    public void A01_ValueCopy()
    {
        var persons = Persons;
        var alt = persons.ValueCopy();
        ReferenceEquals(alt, persons).ShouldBeFalse();
        for (int i = 0; i < persons.Count; i++)
        {
            ReferenceEquals(alt[i], persons[i]).ShouldBeFalse();
            alt[i].Id.ShouldBe(persons[i].Id);
            alt[i].Name.ShouldBe(persons[i].Name);
        }

        var dennis = new Person(4, "Dennis");
        alt.Add(dennis);

        persons.Count.ShouldBe(3);
        alt.Count.ShouldBe(4);

        alt[2].Name = "Charlie";
        alt[2].Name.ShouldNotBe(persons[2].Name);
    }
}
