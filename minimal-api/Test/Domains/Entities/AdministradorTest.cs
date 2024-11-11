using minimal_api.Domains.Entities;

namespace Test.Domains.Entities;

[TestClass]
public class AdministradorTest
{
    [TestMethod]
    public void TestarGetSetPropriedades()
    {
        // Arrange
        var adm = new Administrador
        {
            // Act
            Id = 1,
            Email = "teste@teste.com",
            Senha = "teste",
            Perfil = "Adm"
        };

        // Assert

        Assert.AreEqual(1, adm.Id);
        Assert.AreEqual("teste@teste.com", adm.Email);
        Assert.AreEqual("teste", adm.Senha);
        Assert.AreEqual("Adm", adm.Perfil);
    }
}