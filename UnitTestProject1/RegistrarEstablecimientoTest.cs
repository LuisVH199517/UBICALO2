using System;
using NUnit.Framework;
using UBICALO.Models;
using UBICALO.ViewModel;


namespace UnitTestProject1
{
    [TestFixture]
    public class RegistrarEstablecimientoTest
    {
        [Test]
        public void FlujoRegistraEstablecimiento()
        {
            Establecimiento establecimiento1 = new Establecimiento();
            establecimiento1.EstablecimientoID = 6;
            establecimiento1.Nombre = "La casa del saber";
            establecimiento1.Direccion = "Jr. Casetas 146";
            establecimiento1.RUC = "122111";
            establecimiento1.Latitud = "122111";
            establecimiento1.Longitud = "122111";
            establecimiento1.Portal = "www.casota.com.pe";
            establecimiento1.imagen = "casita1.jpg";
            establecimiento1.Tipo = 1;

            VmRegistrarEstablecimiento vmRE = new VmRegistrarEstablecimiento();
            vmRE.fill(3);
            Assert.IsNotNull(vmRE);
        }
    }
}
