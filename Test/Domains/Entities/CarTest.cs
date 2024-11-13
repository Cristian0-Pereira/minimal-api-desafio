using MinimalApi.Domains.Entities;

namespace Test.Domains.Entities;

[TestClass]
public class CarTest
{
    [TestMethod]
    public void TestarGetSetPropriedades()
    {
        // Arrange
        var car = new Car
        {
            // Act
            Id = 1,
            Name = "VW",
            Model = "Fusca",
            Year = 2000
        };

        // Assert

        Assert.AreEqual(1, car.Id);
        Assert.AreEqual("VW", car.Name);
        Assert.AreEqual("Fusca", car.Model);
        Assert.AreEqual(2000, car.Year);
    }
}