﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BusinessLogic;
using DataBase.Repository;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Mvc;
using ValidationException = BusinessLogic.ValidationException;

namespace PresentationLayer.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class HomeController : Controller
    {
        IOrderService orderService;


        public HomeController(IOrderService serv)
        {
            orderService = serv;
        }

        [HttpGet]
        public IEnumerable<GoodView> GetGoodViews()
        {
            try{
                IEnumerable<GoodDTO> goodDtos = orderService.GetGoods();
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<GoodDTO, GoodView>()).CreateMapper();
            var goods = mapper.Map<IEnumerable<GoodDTO>, List<GoodView>>(goodDtos);
            return goods;
            } catch (ValidationException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
                return new List<GoodView>();
            }
        } 
        [HttpGet("{goodId:int?}",Name = "~/MakeOrder")]
        public OrderView MakeOrder(int? id)
        {
            try
            {
                GoodDTO Good = orderService.GetGood(id);
                int Id = orderService.GetNewOrderId();
                var order = new OrderView {Id = Id,GoodId = Good.Id};

                return order;
            }
            catch (ValidationException ex)
            {
                
                return new OrderView();
            }
        }

        [HttpPost]
        public string CreateOrder(OrderView order)
        {
            try
            {
                var orderDto = new OrderDTO
                    {Id = order.Id,GoodId = order.GoodId, Address = order.Address, GoodQuantity = order.GoodQuantity};
                orderService.MakeOrder(orderDto);
                return $"<h2>Ваш заказ {orderDto.Id} успешно отгружен</h2> ";
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
                return 
                    $"<h2>{ex.Message}</h2>\n Чтобы проверить статус заказа - вернитесь на главную страницу";
            }

        }
        
        /*public ActionResult CheckOrder()
        {
            try
            {
                var checkOrder = new OrderCheckView(); 
                
                return View(checkOrder);
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
                return Content("");
            }

        }*/
        [HttpGet("{orderId}/checkOrder")]
        public OrderView CheckOrder(int id)
        {
            try
            {
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<OrderDTO, OrderView>()).CreateMapper();
                var order = mapper.Map<OrderDTO, OrderView>(orderService.GetOrder(id));
                return order;
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
                return new OrderView{Id = id};
            }

        }
    }
}