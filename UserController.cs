using [custom];
using [custom].Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace [custom]
{
    [Authorize(Policy = Constants.[custom]SecurityPolicy)]
    public class BLGUserAdminController : Controller
    {
        private readonly IMediator _mediator; // MediatR library routes data from this controller to AddUser.cs

        public BLGUserAdminController(IMediator mediator)
        {
            _mediator = mediator; //mediatr injected
        }

        public async Task<IActionResult> GetUsers(GetUsers.Query query)
        {
            var model = await _mediator.Send(query);
            return View(model);
        }
        
        public async Task<IActionResult> Add(Add.Query query)
        {
            var model = await _mediator.Send(query);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Add.Command command)
        {
            var result = await _mediator.Send(command);

            if (!result)
                return this.CommandResultFailureJson(result);

            return this.RedirectToActionJson(nameof(Index));
        }


    }
}
