﻿using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using BusinessLogic.Repo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace FinCore.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("/api/[controller]")]
    public class WalletsController : BaseController
    {
        [HttpGet]
        [AcceptVerbs("GET")]
        public IEnumerable<Wallet> Get()
        {
            try
            {
                return MainService.GetWalletsState(DateTime.MaxValue);
            }
            catch (Exception e)
            {
                log.Error(e.ToString());
            }

            return null;
        }

        [HttpGet]
        [AcceptVerbs("GET")]
        [Route("[action]/{id}")]
        public Wallet Get(int id)
        {
            try
            {
                var wb = MainService.GetWalletsState(DateTime.MaxValue);
                return wb.Where(d => d.Id.Equals(id)).FirstOrDefault();
            }
            catch (Exception e)
            {
                log.Error(e.ToString());
            }

            return null;
        }

        [HttpGet]
        [Route("[action]")]
        [AcceptVerbs("GET")]
        public List<Wallet> GetRange([FromQuery]int id, [FromQuery]DateTime fromDate, [FromQuery]DateTime toDate)
        {
            try
            {
                var w = MainService.GetWalletBalanceRange(id, fromDate, toDate);
                return w;
            }
            catch (Exception e)
            {
                log.Error(e.ToString());
            }

            return null;
        }

        // Warning: month is Zero based - January equal 0
        [HttpGet]
        [AcceptVerbs("GET")]
        [Route("[action]")]
        public List<TimeStat> Performance([FromQuery]int month, [FromQuery]int period)
        {
            try
            {
                var ds = MainService.Container.Resolve<DataService>();
                if (ds == null)
                    return null;
                return ds.Performance(month, (TimePeriod) period);
            }
            catch (Exception e)
            {
                log.Error(e.ToString());
            }

            return null;
        }

        [HttpGet]
        [AcceptVerbs("GET")]
        [Route("[action]")]
        public List<Asset> AssetsDistribution([FromQuery]int type)
        {
            try
            {
                List<Asset> result;
                var ds = MainService.Container.Resolve<DataService>();
                if (ds == null)
                    return null;
                result = ds.AssetsDistribution(type);
                return result;
            }
            catch (Exception e)
            {
                log.Error(e.ToString());
            }
            return null;
        }

        [HttpPut]
        [AcceptVerbs("PUT")]
        [Route("[action]")]
        public ActionResult Put([FromBody]AccountState state)
        {
            try
            {
                if (state == null)
                    return Problem("Empty state passed to Put method!", "Error", StatusCodes.Status500InternalServerError);

                bool bres = MainService.UpdateAccountState(state);
                if (bres)
                    return Ok();
                return Problem("Failed to Update Account State", "Error", StatusCodes.Status417ExpectationFailed);
            }
            catch (Exception e)
            {
                log.Error(e.ToString());
                return Problem(e.ToString(), "Error", StatusCodes.Status500InternalServerError);
            }
        }

    }
}
