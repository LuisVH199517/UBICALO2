using System;
using NUnit.Framework;
using UBICALO.Models;
using UBICALO.ViewModel;

namespace UnitTestProject1
{
    [TestFixture]
    public class ListarProductos1Test
    {
        [Test]
        public void FlujoListarAsociados()
        {
            VmListarAsociados vmLA = new VmListarAsociados();
            vmLA.fill();
           
        }
    }
}
