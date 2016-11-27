using System;
using NUnit.Framework;
using UBICALO.Models;
using UBICALO.ViewModel;

namespace RegistrarAsociadoTest
{
    [TestFixture]
    public class RegistrarAsociadoTest
    {
        [Test]
        public void FlujoRegistrarAsociado()
        {
            Asociado asociado1 = new Asociado();
            asociado1.AsociadoID = 5;
            asociado1.Clave = "asociado1";
            asociado1.Usuario = "asociado1";
            asociado1.Foto = "3432.jpg";
            asociado1.EstablecimientoID = 1;

            VmRegistrarAsociado vmRA = new VmRegistrarAsociado();
            vmRA.fill(1);
            Assert.IsNotNull(vmRA);
        }
    }
}
