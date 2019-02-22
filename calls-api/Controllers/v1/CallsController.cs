using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

[ApiController]
[Produces("application/json")]
[Route("api/v1/[controller]")]
public class CallsController : ControllerBase
{
    private readonly CallAnalyzerContext _context;

    public CallsController(CallAnalyzerContext context)
    {
        _context = context;
    }

    // GET api/v1/calls
    [HttpGet]
    public ActionResult GetCalls(string number = null, string device = null, string cause = null, DateTime? start = null, DateTime? end = null, int take = 25, int skip = 0)
    {
        var query = _context.Calls.AsQueryable();

        if (start == null)
        {
            start = DateTime.Now.Date;
        }

        if (end == null)
        {
            end = DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        }

        if (number != null)
            query = query.Where(l =>
                l.CallingPartyNumber.ToLower().Contains(number.ToLower()) ||
                l.OriginalCalledPartyNumber.ToLower().Contains(number.ToLower()) ||
                l.FinalCalledPartyNumber.ToLower().Contains(number.ToLower())
            );

        if (device != null)
            query = query.Where(l =>
                l.OrigDeviceName.ToLower().Contains(device.ToLower()) ||
                l.DestDeviceName.ToLower().Contains(device.ToLower())
            );

        if (cause != null)
            query = query.Where(l =>
                l.OrigCauseValue.ToLower().Contains(cause.ToLower()) ||
                l.DestCauseValue.ToLower().Contains(cause.ToLower())
            );

        if (start != null)
            query = query.Where(l => l.DateTimeDisconnect >= start);

        if (end != null)
            query = query.Where(l => l.DateTimeDisconnect <= end);

        var results = query
            .OrderByDescending(l => l.DateTimeDisconnect)
            .Take(take)
            .Skip(skip)
            .ToList();

        return Ok(results);
    }

}