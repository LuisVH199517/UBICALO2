﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UBICALO.ViewModel;
using UBICALO.Models;
using System.IO;

using System.Drawing;
using System.Drawing.Imaging;

using UBICALO.Helpers;

namespace UBICALO.Controllers
{
    public class HomeController : Controller
    {
        UbicaloEntities _context = new UbicaloEntities();
        const int productsPerPageInTable = 9;
        const int productsPerPageInDiv = 8;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About(int? id)
        {
            var page = id ?? 0;

            if (Request.IsAjaxRequest())
            {
                return PartialView("_ProductsInDiv", GetPaginatedProducts(page, Session["filtro"].ToString()));
            }
            VmEstablecimientoInfo2 vm = new VmEstablecimientoInfo2();
            vm.productos = _context.Producto.Take(productsPerPageInTable);
            Session["filtro"] = "";
            return View(vm);
        }

        [HttpPost]
        public ActionResult About(VmEstablecimientoInfo2 vm)
        {
            Session["filtro"] = vm.filtro;
            //if (Request.IsAjaxRequest())
            //{
            //    return PartialView("_Products", GetPaginatedProducts(vm.page, Session["filtro"].ToString()));
            //}

            vm.productos = GetPaginatedProducts(0, vm.filtro);
            return View(vm);
        }


        public ActionResult Contact(int? id)
        {
            var page = id ?? 0;
            
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ProductsInTable", GetPaginatedProducts(page, Session["filtro"].ToString()));
            }
            VmListarProductos2 vm = new VmListarProductos2();
            vm.productos = _context.Producto.Take(productsPerPageInTable);
            Session["filtro"] = "";
            return View(vm);
        }

        [HttpPost]
        public ActionResult Contact(VmListarProductos2 vm)
        {
            Session["filtro"] = vm.filtro;
            //if (Request.IsAjaxRequest())
            //{
            //    return PartialView("_Products", GetPaginatedProducts(vm.page, Session["filtro"].ToString()));
            //}

            vm.productos = GetPaginatedProducts(0,vm.filtro);
            return View(vm);
        }


        private List<Producto> GetPaginatedProducts(int page = 1, String filtro = "")
        {
            var skipRecords = page * productsPerPageInTable;

            var partialProducts = _context.Producto;

            if (!(String.IsNullOrEmpty(filtro)||filtro==""))
            {
                return partialProducts.
                    Where(x => x.Nombre.Contains(filtro.ToUpper())).
                    OrderBy(x => x.ProductoID).
                    Skip(skipRecords).
                    Take(productsPerPageInTable).ToList();
            }

            return partialProducts.
                OrderBy(x => x.ProductoID).
                Skip(skipRecords).
                Take(productsPerPageInTable).ToList();

        }
        
        public ActionResult login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult login(VmLogin vm)
        {
            try
            {
                Cliente cliente = _context.Cliente.FirstOrDefault(x => x.Usuario == vm.usuario && x.Clave == vm.clave);
                if (cliente != null)
                {
                    if (cliente.IDApi != null)
                        return RedirectToAction("login");
                    Session["objUsuario"] = cliente;
                    Session["rol"] = "C";
                    return RedirectToAction("verMapa");
                }

                Asociado asociado = _context.Asociado.FirstOrDefault(x => x.Usuario == vm.usuario && x.Clave == vm.clave);
                if (asociado != null)
                {
                    Session["objUsuario"] = asociado;
                    Session["rol"] = "A";
                    return RedirectToAction("estadoCuenta");
                }

                Administrador administrador = _context.Administrador.FirstOrDefault(x => x.Usuario == vm.usuario && x.Clave == vm.clave);
                if (administrador != null)
                {
                    Session["objUsuario"] = administrador;
                    Session["rol"] = "D";
                    return RedirectToAction("listarAsociados");
                }

                return View(vm);
            }
            catch (Exception)
            {
                return View(vm);
            }
        }


        [HttpPost]
        public ActionResult registrarUsuario(VmLogin vm)
        {
            try
            {
                Cliente cliente = new Cliente();
                cliente.Usuario = vm.usuario;
                cliente.Clave = vm.clave;
                cliente.Correo = vm.correo;
                cliente.IDApi = null;
                cliente.Foto = "user.png";
                //string imageFile = System.Web.Http_context.Current.Server.MapPath("~/Content/images/user.png");
                //var srcImage = Image.FromFile(imageFile);
                //var stream = new MemoryStream();
                //srcImage.Save(stream, ImageFormat.Png);
                //cliente.Foto= stream.ToArray();
                //cliente.Rol = "User";
                
                _context.Cliente.Add(cliente);
                _context.SaveChanges();
                Session["objUsuario"] = cliente;
                Session["rol"] = "C";
                return RedirectToAction("verMapa");
            }
            catch (Exception)
            {
                return RedirectToAction("login");
            }
        }


        [HttpPost]
        public ActionResult loginFB(VmLogin vm)
        {
            try
            {
                Cliente cliente = _context.Cliente.FirstOrDefault(x => x.IDApi == vm.FbID);

                if (cliente == null)
                {
                    cliente = new Cliente();
                    cliente.Usuario = vm.usuario;
                    cliente.Clave = "";
                    if (vm.correo != null && vm.correo != "undefined")
                        cliente.Correo = vm.correo;
                    else
                        cliente.Correo = "";
                    cliente.IDApi = vm.FbID;
                    cliente.Foto = vm.FbID; //vm.imagen;

                    //string imageFile = System.Web.Http_context.Current.Server.MapPath("~/Content/images/user.png");
                    //var srcImage = Image.FromFile(imageFile);
                    //var stream = new MemoryStream();
                    //srcImage.Save(stream, ImageFormat.Png);
                    //cliente.Foto= stream.ToArray();
                    //cliente.Rol = "User";

                    _context.Cliente.Add(cliente);
                    _context.SaveChanges();

                    Session["objUsuario"] = cliente;
                    Session["rol"] = "C";

                    return RedirectToAction("verMapa");
                }

                Session["objUsuario"] = cliente;
                Session["rol"] = "C";
                return RedirectToAction("verMapa");
            }
            catch (Exception)
            {
                return RedirectToAction("login");
            }
        }

        public ActionResult configurarCuenta()
        {
            return View();
        }


        public ActionResult cerrarSesion()
        {
            Session.Clear();
            return RedirectToAction("login");
        }



        // Cliente START

        public ActionResult verMapa()
        {
            VmEstaMapa vm = new VmEstaMapa();
            vm.fill();

            return View(vm);
        }


        public ActionResult establecimientoBusqueda()
        {
            VmEstablecimientoBusqueda vm = new VmEstablecimientoBusqueda();
            vm.fill();

            return View(vm);
        }

        [HttpPost]
        public ActionResult establecimientoBusqueda(VmEstablecimientoBusqueda vm)
        {
            vm.fill();

            return View(vm);
        }


        public ActionResult establecimientoInfo(int establecimientoID)
        {                            
            VmEstablecimientoInfo vm = new VmEstablecimientoInfo();
            vm.establecimientoID = establecimientoID;
            vm.fill();

            return View(vm);
        }



        public ActionResult compraProducto(int productoID)
        {
            VmCompraProducto vm = new VmCompraProducto();

            vm.productoID = productoID;
            vm.fill();
            return View(vm);
        }

        [HttpPost]
        public ActionResult compraProducto(VmCompraProducto vm)
        {
            try
            {
                Compra compra = new Compra();
                compra.ClienteID = ((Cliente)Session["objUsuario"]).ClienteID;
                compra.ProductoID = vm.productoID;
                

                compra.QR = "codigo";

                _context.Compra.Add(compra);
                _context.SaveChanges();

                return RedirectToAction("productosAdquiridos");
            }
            catch (Exception)
            {
                return View();
            }

        }


        public ActionResult productosAdquiridos()
        {
            VmProductosAdquiridos vm = new VmProductosAdquiridos();
            vm.fill(((Cliente)Session["objUsuario"]).ClienteID);

            return View(vm);
        }

        
        public ActionResult getQr(int id)
        {
            Bitmap bmp = QRHelper.GenerateQrCode(id.ToString());
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            byte[] bt = ms.ToArray();
            return File(bt, "image/png");
        }

        // Cliente END

        
        // Asociado START

        public ActionResult listarProductos()
        {

            VmListarProductos vm = new VmListarProductos();
            vm.fill(((Asociado)Session["objUsuario"]).EstablecimientoID);
            return View(vm);
        }

        [HttpPost]
        public ActionResult listarProductos(VmListarProductos vm)
        {
            vm.fill(((Asociado)Session["objUsuario"]).EstablecimientoID);
            return View(vm);
        }
        
        public ActionResult registrarProducto(int? productoID)
        {
            UbicaloEntities _context = new UbicaloEntities();
            VmRegistrarProducto vm = new VmRegistrarProducto();
            vm.fill(_context, productoID);            

            return View(vm);
        }

        [HttpPost]
        public ActionResult registrarProducto(VmRegistrarProducto vm, HttpPostedFileBase file)
        {
            try
            {                
                Producto producto = null;

                String categoriaNombre = _context.Categoria.FirstOrDefault(x => x.CategoriaID == vm.categoria).Nombre;


                if (vm.productoID.HasValue)
                {                    
                    producto = _context.Producto.FirstOrDefault(x => x.ProductoID == vm.productoID);

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(categoriaNombre+"-"+vm.nombre + ".jpg");
                        var path = Path.Combine(Server.MapPath("~/Content/images/productos"), fileName);
                        file.SaveAs(path);
                        producto.imagen = fileName;
                    }
                }
                else
                {
                    producto = new Producto();
                    _context.Producto.Add(producto);

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(categoriaNombre + "-" + vm.nombre+".jpg");
                        var path = Path.Combine(Server.MapPath("~/Content/images/productos"), fileName);
                        file.SaveAs(path);
                        producto.imagen = fileName;
                    }
                    else
                    {
                        producto.imagen = "default_product.gif";
                    }

                }

                producto.Nombre = vm.nombre;
                producto.Descripcion = vm.descripcion;
                producto.Costo = vm.costo;
                
                producto.EstablecimientoID = ((Asociado)Session["objUsuario"]).EstablecimientoID;
                producto.CategoriaID = vm.categoria;

                _context.SaveChanges();

                return RedirectToAction("listarProductos");
            }
            catch (Exception)
            {
                vm.fill(_context, null);
                TryUpdateModel(vm);
                return View(vm);
            }
        }

        

        public ActionResult estadoCuenta()
        {
            VmEstadoCuenta vmEstadoCuenta = new VmEstadoCuenta();
            vmEstadoCuenta.fill(((Asociado)Session["objUsuario"]).EstablecimientoID);
            return View(vmEstadoCuenta);
        }

        // Asociado END

        // Administrador START

        public ActionResult listarAsociados()
        {
            VmListarAsociados vmListarAsociados = new VmListarAsociados();
            vmListarAsociados.fill();
            return View(vmListarAsociados);
        }

        
        [HttpPost]
        public ActionResult listarAsociados(VmListarAsociados vm)
        {
            vm.fill();
            return View(vm);
        }

        public ActionResult registrarAsociado(int? asociadoID)
        {
            VmRegistrarAsociado vm = new VmRegistrarAsociado();
            vm.fill(asociadoID);
            return View(vm);
        }

        [HttpPost]
        public ActionResult registrarAsociado(VmRegistrarAsociado vm, HttpPostedFileBase file)
        {
            try
            {
                Asociado asociado = null;
                if (vm.asociadoID.HasValue)
                {
                    asociado = _context.Asociado.FirstOrDefault(x => x.AsociadoID == vm.asociadoID);

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = asociado.Usuario+".jpg";// Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/images/users"), fileName);
                        file.SaveAs(path);
                        asociado.Foto = fileName;
                    }
                    //else
                    //{

                    //    obj.imagen = "portfolio5.jpg";
                    //}
                }
                else
                {
                    asociado = new Asociado();
                    _context.Asociado.Add(asociado);

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = vm.usuario+".jpg";// Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/images/users"), fileName);
                        file.SaveAs(path);
                        asociado.Foto = fileName;
                    }
                    else
                    {
                        asociado.Foto = "user.png";
                    }
                    
                }

                asociado.Usuario = vm.usuario;
                asociado.Clave = vm.clave;
                asociado.EstablecimientoID = vm.establecimientoId;
                
                _context.SaveChanges();

                return RedirectToAction("listarAsociados");
            }
            catch (Exception)
            {
                vm.fill(null);
                TryUpdateModel(vm);
                return View(vm);
            }
        }




        public ActionResult listarEstablecimiento()
        {
            VmEstablecimientoBusqueda vm = new VmEstablecimientoBusqueda();
            vm.fill();

            return View(vm);
        }

        public ActionResult registrarEstablecimiento(int? establecimientoID)
        {
            VmRegistrarEstablecimiento vm = new VmRegistrarEstablecimiento();
            vm.fill(establecimientoID);
            return View(vm);
        }

        [HttpPost]
        public ActionResult agregarEstablecimiento(VmRegistrarEstablecimiento vm, HttpPostedFileBase file)
        {
            try
            {
                Establecimiento establecimiento = null;

                if (vm.establecimientoID.HasValue)
                {
                    establecimiento = _context.Establecimiento.FirstOrDefault(x => x.EstablecimientoID == vm.establecimientoID);

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                        file.SaveAs(path);
                        establecimiento.imagen = "~/Content/images/" + fileName;
                    }
                    //else
                    //{

                    //    obj.imagen = "portfolio5.jpg";
                    //}
                }
                else
                {
                    establecimiento = new Establecimiento();
                    _context.Establecimiento.Add(establecimiento);

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                        file.SaveAs(path);
                        establecimiento.imagen = "~/Content/images/" + fileName;
                    }
                    else
                    {
                        establecimiento.imagen = "~/Content/images/4.jpg";
                    }
                }

                establecimiento.Nombre = vm.nombre;
                establecimiento.Direccion = vm.direccion;
                establecimiento.RUC = vm.ruc;
                establecimiento.Latitud = vm.latitud;
                establecimiento.Longitud = vm.longitud;
                establecimiento.Portal = vm.portal;

                _context.SaveChanges();

                return RedirectToAction("listarEstablecimiento");
            }
            catch (Exception)
            {
                vm.fill(null);
                TryUpdateModel(vm);
                return View(vm);
            }

        }



        // Administrador END


    }
}