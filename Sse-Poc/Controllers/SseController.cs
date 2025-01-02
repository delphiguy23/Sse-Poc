using Sse_Poc.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Channels;
using Sse_Poc.Models;

namespace Sse_Poc.Controllers;

public class SseController(SseService sseService, PulseService pulseService, ILogger<SseController> logger)
    : ControllerBase
{
    [HttpGet("stream/{channelName}")]
    public async Task StreamEvents(string channelName)
    {
        Response.Headers.Add("Content-Type", "text/event-stream");
        Response.Headers.Add("Cache-Control", "no-cache");
        Response.Headers.Add("Connection", "keep-alive");

        var messageChannel = Channel.CreateUnbounded<string>();

        var subscriberTask = sseService.SubscribeToChannelAsync(
            channelName,
            messageChannel.Writer,
            HttpContext.RequestAborted
        );

        try
        {
            await using var streamWriter = new StreamWriter(Response.Body);
            
            await foreach (var message in messageChannel.Reader.ReadAllAsync(HttpContext.RequestAborted))
            {
                await streamWriter.WriteAsync($"data: {message}\n\n");
                await streamWriter.FlushAsync();
            }
        }
        catch (Exception ex)
        {
            //
        }
    }

    [HttpPost("publish/{channel}")]
    public IActionResult PublishEvent(string channel, [FromBody] MyEvent myEvent)
    {
        sseService.PublishEvent(channel, myEvent);
        return Ok();
    }

    [HttpPost("pulse")]
    public IActionResult RecordPulse([FromBody] string pulseValue)
    {
        logger.LogInformation("Recording pulse: {PulseValue}", pulseValue);
        var id = pulseService.StorePulse(pulseValue);
        return Ok(new { id });
    }

    [HttpGet("pulses")]
    public IActionResult GetPulses()
    {
        logger.LogInformation("Getting all pulses");
        return Ok(pulseService.GetAllPulses());
    }

    [HttpDelete("killSession")]
    public IActionResult KillSession([FromBody] string sessionMessage)
    {
        logger.LogInformation("Kill session requested: {SessionMessage}", sessionMessage);
        sseService.PublishEvent("killSession", sessionMessage);
        return Ok();
    }

    [HttpDelete("killall")]
    public IActionResult KillAll()
    {
        logger.LogInformation("Kill all pulses");
        var pulses = pulseService.GetAllPulses();

        foreach (var group in pulses)
        {
            sseService.PublishEvent("killSession", group.TabId);

        }
        return Ok();
    }

    [HttpGet("stream/killSession")]
    public async Task StreamKillSession()
    {
        Response.Headers.Add("Content-Type", "text/event-stream");
        Response.Headers.Add("Cache-Control", "no-cache");
        Response.Headers.Add("Connection", "keep-alive");

        var messageChannel = Channel.CreateUnbounded<string>();

        var subscriberTask = sseService.SubscribeToChannelAsync(
            "killSession", 
            messageChannel.Writer, 
            HttpContext.RequestAborted
        );

        try 
        {
            await using var streamWriter = new StreamWriter(Response.Body);
            
            await foreach (var message in messageChannel.Reader.ReadAllAsync(HttpContext.RequestAborted))
            {
                await streamWriter.WriteAsync($"data: {message}\n\n");
                await streamWriter.FlushAsync();
            }
        }
        catch (Exception ex)
        {
            //
        }
    }
}
