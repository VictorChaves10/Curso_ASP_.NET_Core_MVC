﻿using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TopBurgers.Models;
using TopBurgers.Repositories.Interfaces;

namespace TopBurgers.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly CarrinhoCompra _carrinhoCompra;

        public PedidoController(IPedidoRepository pedidoRepository, CarrinhoCompra carrinhoCompra)
        {
            _pedidoRepository = pedidoRepository;
            _carrinhoCompra = carrinhoCompra;
        }

        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Checkout(Pedido pedido)
        {
            int totalItensPedido = 0;
            decimal precoTotalPedido = 0.0m;

            List<CarrinhoCompraItem> items = _carrinhoCompra.GetCarrinhoCompraItems();
            _carrinhoCompra.CarrinhoCompraItems = items;

            if (_carrinhoCompra.CarrinhoCompraItems.Count > 0)
            {

                ModelState.AddModelError("", "Seu carrinho está vazio");

            }

            foreach (var item in items)
            {
                totalItensPedido += item.Quantidade;
                precoTotalPedido += (item.Lanche.Preco * item.Quantidade);
            }

            pedido.TotalItensPedido = totalItensPedido;
            pedido.PedidoTotal = precoTotalPedido;

            if (ModelState.IsValid)
            {
                _pedidoRepository.CriarPedido(pedido);

                ViewBag.CheckoutCompleto = "Obrigado pelo seu pedido";
                ViewBag.TotalPedido = _carrinhoCompra.GetCarrinhoCompraTotal();

                _carrinhoCompra.LimparCarrinho();

                return View("~/Views/Pedido/CheckoutCompleto.cshtml", pedido);

            }

            return View(pedido);

        }

    }
}
