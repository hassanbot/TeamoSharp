﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TeamoSharp.Services;
using static TeamoSharp.ErrorHandling.DiscordPoster;

namespace TeamoSharp.Commands
{
    public class TeamoCommands : BaseCommandModule
    {
        private readonly ILogger _logger;
        private readonly IMainService _playService;

        public TeamoCommands(
            ILogger<TeamoCommands> logger, IMainService playService)
        {
            _logger = logger;
            _playService = playService;
        }

        [Command("create")]
        [Description("Create a new teamo")]
        public async Task Create(CommandContext ctx)
        {
            DateTime endDate = DateTime.Now + new TimeSpan(0, 0, 20);
            string game = "League of LoL";
            int maxPlayers = 5;
            var channel = ctx.Channel;
            try
            {
                await _playService.CreateAsync(endDate, maxPlayers, game, channel.Id, ctx.Client);
            }
            catch (Exception e)
            {
                await PostExceptionMessageAsync(channel, _logger, e, "Could not create a new teamo :(");
                return;
            }
        }


        [Command("change")]
        [Aliases("edit")]
        [Description("Edit an aspect of a teamo")]
        public async Task Change(CommandContext ctx, int postId, string property, [RemainingText] string args)
        {
            // TODO: Custom command handler
            try
            {
                var propLower = property.ToLower();
                if (propLower == "date" || propLower == "time")
                {
                    // TODO: Better parsing
                    if (DateTime.TryParse(args, out DateTime date))
                    {
                        await _playService.EditDateAsync(date, postId, ctx.Channel);
                    }
                    else
                    {
                        await PostExceptionMessageAsync(ctx.Channel, _logger, s: $"Could not parse {args} as date and/or time");
                        return;
                    }
                }
                else if (propLower == "players" || propLower == "maxplayers" || propLower == "numplayers")
                {
                    if (int.TryParse(args, out int maxPlayers))
                    {
                        await _playService.EditNumPlayersAsync(maxPlayers, postId, ctx.Channel);
                    }
                    else
                    {
                        await PostExceptionMessageAsync(ctx.Channel, _logger, s: $"Could not parse {args} as integer");
                        return;
                    }
                }
                else if (propLower == "game")
                {
                    await _playService.EditGameAsync(args, postId, ctx.Channel);
                }
                else
                {
                    await PostExceptionMessageAsync(ctx.Channel, _logger, s: $"Unknown teamo property: {property}. Select either \"date\", \"players\" or \"game\"");
                }
            } catch (System.Exception e)
            {
                await PostExceptionMessageAsync(ctx.Channel, _logger, e, "Could not edit teamo!");
            }
        }


        [Command("delete")]
        [Aliases("stop")]
        [Description("Create a new teamo")]
        public async Task Delete(CommandContext ctx, [RemainingText] string postIdString)
        {
            if (!int.TryParse(postIdString, out int postId))
            {
                _logger.LogInformation($"Delete command argument \"{postIdString}\"could not be converted to int");
                return;
            }

            try
            {
                await _playService.DeleteAsync(postId, ctx.Client);
            }
            catch (Exception e)
            {
                await PostExceptionMessageAsync(ctx.Channel, _logger, e, "Could not delete teamo");
                return;
            }
        }
    }
}
